# Base de données et API avec Docker / Podman Compose

> **Note** : ce projet est un **POC**. La configuration Docker/Podman ci‑dessous est à des fins de démo et de développement, pas pour un déploiement production.

Le projet propose deux services dans `docker-compose.yml` :

- **db** : SQL Server 2022 (port 1433)
- **api** : API ASP.NET Core (port 8080), dépend de `db`

---

## Lancer la BDD et l’API ensemble (recommandé)

Depuis la **racine du projet** (où se trouve `docker-compose.yml`) :

### Avec Podman

```bash
podman compose up -d --build
```

### Avec Docker

```bash
docker compose up -d --build
```

- **API** : http://localhost:8080 — Swagger : http://localhost:8080/swagger  
- **Base** : `localhost:1433` (user `sa`, mot de passe par défaut ci‑dessous)

`--build` reconstruit l’image de l’API si le code ou le Dockerfile a changé.

### Voir les logs

```bash
podman compose logs -f
# ou
docker compose logs -f
```

### Arrêter

```bash
podman compose down
# ou
docker compose down
```

Avec suppression des données (volume) :

```bash
podman compose down -v
```

---

## Lancer uniquement la base de données

Si vous voulez faire tourner **uniquement la BDD** en conteneur et l’API en local (Visual Studio ou `dotnet run`) :

```bash
podman compose up -d db
# ou
docker compose up -d db
```

Puis lancer le projet **Hexagonal.API** en mode **Development**. La chaîne de connexion est dans `appsettings.Development.json` (localhost,1433).

```bash
dotnet run --project Hexagonal.API
```

Les migrations EF s’appliquent au démarrage en Development.

---

## Configuration

- **Mot de passe SA** : `YourStrong@Passw0rd` par défaut.  
  Pour le changer : fichier `.env` à la racine (voir `.env.example`) :
  ```env
  MSSQL_SA_PASSWORD=VotreMotDePasse
  ```
- **Base** : `HexagonalOrderManagement` (créée par les migrations au premier run de l’API).
- **Connexion depuis l’API en conteneur** : `Server=db,1433;...` (nom du service Compose).  
- **Connexion depuis l’hôte** : `Server=localhost,1433;...`.

---

## Dépannage

- **Build API échoue** : s’assurer d’exécuter `compose` depuis la **racine de la solution** (contexte = `.`, Dockerfile dans `Hexagonal.API/Dockerfile`).
- **Podman** : utiliser `podman compose` (Podman 4.1+). Sinon, installer `podman-compose` ou configurer l’alias Docker → Podman.
