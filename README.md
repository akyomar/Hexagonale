# Hexagonale – Gestion des commandes clients

> **Note** : ce projet est un **POC (proof of concept)** pour illustrer l’architecture hexagonale en .NET. Il n’est pas destiné à la production telle quelle.

Application .NET 8 basée sur l’**architecture hexagonale (Ports & Adapters)** : gestion des **clients**, **produits**, **commandes** et **facturation**.

---

## Structure du projet

```
Hexagonale/
├── Hexagonal.Api/          # Adaptateurs entrants (API REST, Swagger)
├── src/
│   ├── Hexagonal.Domain/   # Entités, Value Objects, règles métier (aucune dépendance)
│   ├── Hexagonal.Application/  # Use cases, ports, DTOs, validation
│   └── Hexagonal.Infrastructure/  # EF Core, repositories, services (adaptateurs sortants)
├── test/
│   └── Hexagonal.UnitTests/
├── docs/                   # Documentation
├── scripts/                # Scripts (ex. couverture)
├── docker-compose.yml      # BDD + API (SQL Server + API conteneurisée)
└── Hexagonale.sln
```

- **Domaine** : `Customer`, `Order`, `OrderItem`, `Product`, `Invoice`, Value Objects (`Money`, `Email`, `Address`).
- **Application** : cas d’usage (CreateCustomer, CreateOrder, GetOrderById, ConfirmOrder, GenerateInvoice, etc.), ports entrants/sortants, FluentValidation.
- **Infrastructure** : SQL Server (EF Core), repositories, génération de numéros de facture.
- **API** : contrôleurs REST, middleware d’erreurs, injection des dépendances.

---

## Prérequis

- .NET 8 SDK  
- SQL Server (local, LocalDB, ou conteneur)  
- **Docker** ou **Podman** (pour lancer BDD et/ou API en conteneurs)

---

## Lancer l’application

### Option 1 : BDD + API en conteneurs (recommandé)

À la racine du projet :

```bash
# Avec Podman
podman compose up -d --build

# Avec Docker
docker compose up -d --build
```

- **API** : http://localhost:8080  
- **Swagger** : http://localhost:8080/swagger  
- **Base** : `localhost:1433`, user `sa`, mot de passe `YourStrong@Passw0rd`

Détails (mot de passe, logs, arrêt) : [docs/docker.md](docs/docker.md).

### Option 2 : BDD en conteneur, API en local

```bash
podman compose up -d db
# ou
docker compose up -d db
```

Puis :

```bash
dotnet run --project Hexagonal.API
```

L’API utilise `appsettings.Development.json` (connexion à `localhost,1433`). Les migrations s’appliquent au démarrage.

---

## Tests

```bash
dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj
```

Couverture (~91 % lignes, ~75 % branches) :

```bash
dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj --collect:"XPlat Code Coverage" --results-directory ./TestResults --settings test/Hexagonal.UnitTests/coverlet.runsettings
```

Rapport HTML (après installation de ReportGenerator) :

```bash
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html"
```

Voir [docs/couverture-tests.md](docs/couverture-tests.md) et le script `scripts/test-coverage.ps1`.

---

## CI/CD (Azure Pipelines)

Le dépôt contient un pipeline Azure DevOps **CI** : **`azure-pipelines.yml`**.

- **Déclenchement** : push et PR sur `main`, `master`, `develop` (hors changements docs/md uniquement).
- **Étapes** : restore → build (Release) → tests unitaires avec couverture → publication des résultats et de la couverture dans Azure DevOps.

Pour l’activer : Pipelines → New pipeline → choisir le repo → « Existing Azure Pipelines YAML file » → sélectionner **`azure-pipelines.yml`**.

Détails : [docs/ci-cd.md](docs/ci-cd.md).

---

## Documentation

| Fichier | Contenu |
|--------|---------|
| [docs/docker.md](docs/docker.md) | Lancer BDD et API avec Docker / Podman Compose |
| [docs/couverture-tests.md](docs/couverture-tests.md) | Tests unitaires et couverture de code |
| [docs/ci-cd.md](docs/ci-cd.md) | Pipeline CI Azure DevOps (build, test, couverture) |
| [docs/architecture-hexagonale-mermaid.md](docs/architecture-hexagonale-mermaid.md) | Schéma Mermaid de l’architecture |

---

## Technologies

- .NET 8, ASP.NET Core Web API  
- Entity Framework Core 8, SQL Server  
- FluentValidation  
- xUnit, Moq, FluentAssertions, Coverlet  
