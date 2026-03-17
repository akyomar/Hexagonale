# CI/CD – Azure Pipelines

> **Note** : ce projet est un **POC**. Les pipelines ci-dessous servent à la démo et à l’intégration continue, pas à un déploiement production.

## Pipeline CI (azure-pipelines.yml)

Un unique pipeline **CI** à la racine du repo :

- **Déclenchement** : push sur `main`, `master`, `develop` et sur les Pull Requests vers ces branches.
- **Exclusions** : les modifications uniquement dans `docs/`, `*.md` ou `.gitignore` ne déclenchent pas le pipeline.
- **Étapes** :
  1. Installation du SDK .NET 8
  2. Restore de la solution
  3. Build en configuration **Release**
  4. **Tests unitaires** sur `test/Hexagonal.UnitTests` avec collecte de couverture (Coverlet)
  5. Publication des **résultats de tests** (rapport VSTest / TRX)
  6. Publication du **rapport de couverture** (pour l’onglet « Code coverage » dans Azure DevOps)

### Configurer le pipeline dans Azure DevOps

1. **Projet** : Azure DevOps → Projet → Pipelines → New pipeline.
2. **Source** : choisir le repo (GitHub, Azure Repos, etc.).
3. **Configuration** : « Existing Azure Pipelines YAML file » → branche par défaut → chemin **`/azure-pipelines.yml`**.
4. **Save** (et optionnellement « Run » pour la première exécution).

Aucune variable secrète n’est requise pour le CI (build + test).

### Résultats disponibles après un run

- **Tests** : onglet « Tests » du run de pipeline (résultats, durée, échecs).
- **Couverture** : onglet « Code coverage » (si les artefacts Coverlet ont été trouvés).

### Exécution locale (équivalent)

Pour reproduire localement les étapes du CI :

```bash
dotnet restore
dotnet build --configuration Release
dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj --no-build --configuration Release --collect:"XPlat Code Coverage" --results-directory ./TestResults --settings test/Hexagonal.UnitTests/coverlet.runsettings
```

---

## Évolutions possibles (hors POC)

- **CD** : déploiement vers un environnement (Azure App Service, AKS, etc.) à partir d’un artefact ou d’une image Docker.
- **Build d’image Docker** : job qui build l’image de l’API (Dockerfile) et la pousse vers un registre (ACR, Docker Hub).
- **Environnements** : approbations, slots (staging/production).
- **Variables / secrets** : chaînes de connexion, clés API, injectées via Variable groups ou Azure Key Vault.

Pour un POC, le pipeline fourni se limite au **build et aux tests** avec publication de la couverture.
