# Exécute les tests unitaires avec couverture de code et affiche le résumé.
# Optionnel : génère un rapport HTML si reportgenerator est installé (dotnet tool install -g dotnet-reportgenerator-globaltool).

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir
$resultsDir = Join-Path $rootDir "TestResults"
$reportDir = Join-Path $rootDir "CoverageReport"

Push-Location $rootDir

# Nettoyer les anciens résultats
if (Test-Path $resultsDir) { Remove-Item $resultsDir -Recurse -Force }
if (Test-Path $reportDir) { Remove-Item $reportDir -Recurse -Force }

Write-Host "Execution des tests avec couverture..." -ForegroundColor Cyan
$testResult = dotnet test test/Hexagonal.UnitTests/Hexagonal.UnitTests.csproj `
    --collect:"XPlat Code Coverage" `
    --results-directory $resultsDir `
    --settings test/Hexagonal.UnitTests/coverlet.runsettings `
    --no-build `
    --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Pop-Location
    exit $LASTEXITCODE
}

# Trouver le dernier fichier cobertura
$coberturaFile = Get-ChildItem -Path $resultsDir -Recurse -Filter "coverage.cobertura.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $coberturaFile) {
    Write-Host "Fichier de couverture non trouve." -ForegroundColor Yellow
    Pop-Location
    exit 1
}

# Lire et afficher le résumé
[xml]$cov = Get-Content $coberturaFile.FullName
$lineRate = [double]$cov.coverage.'line-rate'
$branchRate = [double]$cov.coverage.'branch-rate'
$linesCovered = [int]$cov.coverage.'lines-covered'
$linesValid = [int]$cov.coverage.'lines-valid'
$branchesCovered = [int]$cov.coverage.'branches-covered'
$branchesValid = [int]$cov.coverage.'branches-valid'

Write-Host ""
Write-Host "========== Couverture des tests ==========" -ForegroundColor Green
Write-Host "  Lignes   : $linesCovered / $linesValid ($([math]::Round($lineRate * 100, 1))%)"
Write-Host "  Branches : $branchesCovered / $branchesValid ($([math]::Round($branchRate * 100, 1))%)"
Write-Host "==========================================" -ForegroundColor Green
Write-Host ""

# Rapport HTML si reportgenerator est disponible
$reportGen = Get-Command reportgenerator -ErrorAction SilentlyContinue
if ($reportGen) {
    Write-Host "Generation du rapport HTML..." -ForegroundColor Cyan
    reportgenerator "-reports:$($coberturaFile.FullName)" "-targetdir:$reportDir" "-reporttypes:Html;HtmlSummary"
    Write-Host "Rapport : $reportDir\index.html" -ForegroundColor Green
} else {
    Write-Host "Pour un rapport HTML : dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Gray
}

Pop-Location
