# Návod spuštění
Aplikace byla vyvíjena ve Visual Studiu a testována na Windows. 

## Požadavky
- **.NET SDK** (verze 8.0) - dostupné z https://dotnet.microsoft.com/en-us/download
- **MariaDB** (verze 11.x) - dostupné z https://mariadb.org/download/
- **Node.js** (verze 20.11) a **npm** 

## Konfigurace databáze
V souboru `.\App.Server\appsettings.json` je potřeba aktualizovat 
"connections stringy" pro databáze a přihlašovací údaje administrátora.

- Samotné databáze není třeba vytvářet, o to se postaráme později.
- Stačí upravit přihlašovací údaje pro MariaDB server.
- Údaje admina zahrnuje:
    - Email : validní emailová adresa
    - Heslo : musí odpovídat konvencím pro hesla - alespoň jedno velké a malé písmeno, číslo, znak etc.

## Visual Studio
Ve Visual Studiu je možné nakonfigurovat "startup project" tak, aby spouštěl více projektů současně (frontend i backend).

## Backend
Spuštění backendu ze složky  `.\App.Server`:
```shell
dotnet run 
```
Po spuštění backendu se automaticky připojí frontendová aplikace a otevře se Vite terminál s odkazem na localhost adresu,
na které bude aplikace k dispozici:[https://localhost:5173/planovani](https://localhost:5173/planovani).

Pro spuštění testů, spouštět uvnitř složky `.\Tests` 
```shell
dotnet test
```

## Frontend
Pro vývoj a sestavování frontendu aplikace je použit nástroj Vite.

Obnovení instalace balíčků:
```shell
npm install
```
Spustit frontend samostatně -  v rootu složky `.\app.client`:
```shell
npm run dev
```

Pro spuštění testů frontendu:
```shell
npm run test
```

## Setup Databáze 
Pro vytvoření databáze je nutné spustit následující příkaz,
v Package Manager Console (Visual Studio):
```shell
    Update-Database -context PlannerNP
    Update-Database -context ApplicationIdentity
```
nebo v terminálu s instalovaným EF:
```shell
    dotnet ef database update --context PlannerNP
    dotnet ef database update --context ApplicationIdentity
```


## Build a Release

Pro sestavení frontendu použijte následující příkaz v root složce .\app.client:
```shell
    npm run build
```
Tento příkaz vytvoří statickou verzi aplikace do složky .\app.client\dist.
Obsah této složky pak může být nasazen na backend server do složky wwwroot, nebo na externí server.

Poté můžeme spustit release build aplikace z `.\App.Server\`:
```shell
    dotnet publish -c Release -o ./publish
```
