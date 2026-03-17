# Couverture des tests unitaires

## Résumé (dernier run)

| Métrique   | Couvert | Total | Pourcentage |
|-----------|---------|--------|-------------|
| **Lignes**   | 437 | 477 | **~91,6 %** |
| **Branches** | 99 | 132 | **~75 %** |

La couverture porte sur **Hexagonal.Domain** et **Hexagonal.Application** (pas l’API ni l’Infrastructure, non référencés par les tests). Objectif 80 % dépassé.

---

## Ce qui est couvert

- **Domaine** : `Customer`, `Order`, `OrderItem`, `Product`, `Invoice`, `Money`, `Email`, `Address`, exceptions métier.
- **Application** : tous les use cases (CreateCustomer, GetCustomerById, CreateProduct, CreateOrder, GetOrderById, ConfirmOrder, GenerateInvoice), cas de succès et d’échec (validation, non trouvé, doublon, etc.), `Result` / `Result<T>`, validateurs FluentValidation (CreateCustomer, CreateProduct, CreateOrder).

---

## Lancer les tests avec couverture

### Option 1 : Ligne de commande

```bash
dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj --collect:"XPlat Code Coverage" --results-directory ./TestResults --settings test/Hexagonal.UnitTests/coverlet.runsettings
```

Les fichiers de couverture (Cobertura, OpenCover) sont dans `TestResults/<guid>/`.

### Option 2 : Script PowerShell

```powershell
.\scripts\test-coverage.ps1
```

Le script affiche le résumé (lignes / branches) et, si **ReportGenerator** est installé, génère un rapport HTML dans `CoverageReport/`.

### Rapport HTML (optionnel)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Puis après un run avec couverture :

```bash
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:CoverageReport" "-reporttypes:Html;HtmlSummary"
```

Ouvrir `CoverageReport/index.html` dans un navigateur.

---

## Relancer uniquement les tests (sans couverture)

```bash
dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj
```
