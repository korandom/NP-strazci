# N�vod spu�t�n�
Aplikace byla vyv�jena ve Visual Studiu a testov�na na Windows. 

## Po�adavky
- **.NET SDK** (verze 8.0) - dostupn� z https://dotnet.microsoft.com/en-us/download
- **MariaDB** (verze 11.x) - dostupn� z https://mariadb.org/download/
- **Node.js** (verze 20.11) a **npm** 

## Konfigurace datab�ze
V souboru `.\App.Server\appsettings.json` je pot�eba aktualizovat 
"connections stringy" pro datab�ze a p�ihla�ovac� �daje administr�tora.

- Samotn� datab�ze nen� t�eba vytv��et, o to se postar�me pozd�ji.
- Sta�� upravit p�ihla�ovac� �daje pro MariaDB server.
- �daje admina zahrnuje:
    - Email : validn� emailov� adresa
    - Heslo : mus� odpov�dat konvenc�m pro hesla - alespo� jedno velk� a mal� p�smeno, ��slo, znak etc.

## Visual Studio
Ve Visual Studiu je mo�n� nakonfigurovat "startup project" tak, aby spou�t�l v�ce projekt� sou�asn� (frontend i backend).

## Backend
Spu�t�n� backendu ze slo�ky  `.\App.Server`:
```shell
dotnet run 
```
Po spu�t�n� backendu se automaticky p�ipoj� frontendov� aplikace a otev�e se Vite termin�l s odkazem na localhost adresu,
na kter� bude aplikace k dispozici:[https://localhost:5173/planovani](https://localhost:5173/planovani).

Pro spu�t�n� test�, spou�t�t uvnit� slo�ky `.\Tests` 
```shell
dotnet test
```

## Frontend
Pro v�voj a sestavov�n� frontendu aplikace je pou�it n�stroj Vite.

Obnoven� instalace bal��k�:
```shell
npm install
```
Spustit frontend samostatn� -  v rootu slo�ky `.\app.client`:
```shell
npm run dev
```

Pro spu�t�n� test� frontendu:
```shell
npm run test
```

## Setup Datab�ze 
Pro vytvo�en� datab�ze je nutn� spustit n�sleduj�c� p��kaz,
v Package Manager Console (Visual Studio):
```shell
    Update-Database -context PlannerNP
    Update-Database -context ApplicationIdentity
```
nebo v termin�lu s instalovan�m EF:
```shell
    dotnet ef database update --context PlannerNP
    dotnet ef database update --context ApplicationIdentity
```


## Build a Release

Pro sestaven� frontendu pou�ijte n�sleduj�c� p��kaz v root slo�ce .\app.client:
```shell
    npm run build
```
Tento p��kaz vytvo�� statickou verzi aplikace do slo�ky .\app.client\dist.
Obsah t�to slo�ky pak m��e b�t nasazen na backend server do slo�ky wwwroot, nebo na extern� server.

Pot� m��eme spustit release build aplikace z `.\App.Server\`:
```shell
    dotnet publish -c Release -o ./publish
```
