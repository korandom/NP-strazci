# Návod spuštìní
Aplikace byla vyvíjena ve Visual Studiu a testována na Windows. 

## Požadavky
- **.NET SDK** (verze 8.0) - dostupné z https://dotnet.microsoft.com/en-us/download
- **MariaDB** (verze 11.x) - dostupné z https://mariadb.org/download/
- **Node.js** (verze 20.11) a **npm** 

## Konfigurace databáze
V souboru `.\App.Server\appsettings.json` je potøeba aktualizovat 
"connections stringy" pro databáze a pøihlašovací údaje administrátora.

- Samotné databáze není tøeba vytváøet, o to se postaráme pozdìji.
- Staèí upravit pøihlašovací údaje pro MariaDB server.
- Údaje admina zahrnuje:
    - Email : validní emailová adresa
    - Heslo : musí odpovídat konvencím pro hesla - alespoò jedno velké a malé písmeno, èíslo, znak etc.

## Visual Studio
Ve Visual Studiu je možné nakonfigurovat "startup project" tak, aby spouštìl více projektù souèasnì (frontend i backend).

## Backend
Spuštìní backendu ze složky  `.\App.Server`:
```shell
dotnet run 
```
Po spuštìní backendu se automaticky pøipojí frontendová aplikace a otevøe se Vite terminál s odkazem na localhost adresu,
na které bude aplikace k dispozici:[https://localhost:5173/planovani](https://localhost:5173/planovani).

Pro spuštìní testù, spouštìt uvnitø složky `.\Tests` 
```shell
dotnet test
```

## Frontend
Pro vývoj a sestavování frontendu aplikace je použit nástroj Vite.

Obnovení instalace balíèkù:
```shell
npm install
```
Spustit frontend samostatnì -  v rootu složky `.\app.client`:
```shell
npm run dev
```

Pro spuštìní testù frontendu:
```shell
npm run test
```

## Setup Databáze 
Pro vytvoøení databáze je nutné spustit následující pøíkaz,
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

Pro sestavení frontendu použijte následující pøíkaz v root složce .\app.client:
```shell
    npm run build
```
Tento pøíkaz vytvoøí statickou verzi aplikace do složky .\app.client\dist.
Obsah této složky pak mùže být nasazen na backend server do složky wwwroot, nebo na externí server.

Poté mùžeme spustit release build aplikace z `.\App.Server\`:
```shell
    dotnet publish -c Release -o ./publish
```
