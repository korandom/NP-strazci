# Plánování sluby strácù
Tento dokument obsahuje popis architektury webové aplikace pro plánování sluby strácù.

## Obsah
- [Plánování sluby strácù](#planovani-sluzby-strazcu)
    - [Obsah](#obsah)
    - [Úvod](#uvod)
    - [Základní informace o systému](#zakladni-informace-o-systemu)
    - [Front-end webové aplikace](#front-end-webove-aplikace)
        - [Browser router](#browser-router)
        - [Services](#Services)
        - [Poskytovatel Dat Plánù](#poskytovatel-dat-planu)
        - [Poskytovatel Dat Obvodu](#poskytovatel-dat-obvodu)
        - [Poskytovatel Autentizace a autorizace](#poskytovatel-autentizace-a-autorizace)
        - [Pøihlašovací stránka](#prihlasovaci-stranka)
        - [Menu](#menu)
        - [Stránka plánování](#stranka-planovani)
        - [Stránka správy zdrojù](#stranka-spravy-zdroju)
        - [Domovská stránka](#domovska-stranka)
    - [Backend aplikace](#backend-aplikace)
        - [Kontrolery](#kontrolery)
        - [Unit of Work](#unit-of-work)
        - [Repozitáøe](#repozitare)
        - [Huby](#huby)
        - [Autentikace a autorizace](#autentikace-a-autorizace)



## Úvod
Aplikace Plánovací kalendáø je urèena pro stráce a vedoucí stráních obvodù národního parku. 
Strácùm umoòuje zobrazit si plán sluby a naplánovat si trasy.
Vedoucím stráních obvodù umoòuje vytváøet plán sluby a spravovat objekty v obvodu.

![C1 model](./prilohy/architektura/C1_model.png)
*C4 diagram Level 1: Plánovací kalendáø*

## Základní informace o systému
Plánovací kalendáø je webová aplikace, která slouí k plánování sluby strácù v reálném èase a zobrazování plánù na den.
Aplikace poskytne klientovi front-end, kterı umoòuje uivateli interagovat se systémem a mìnit plán sluby.
Front-end webové aplikace vyuívá knihovnu React s Typescriptem. 
Uivatelské rozhraní zobrazuje a pracuje s daty, které jsou naètené z backendu ASP.NET Core prostøednictím volání API kontrolerù. 
Aby byla data aktuální pøes všechna zaøízení, je backend vyuit také na posílání informací o provedenıch zmìnách.
Backend ète a zapisuje data do dvou databází MariaDB: jedna obsahuje aplikaèní data o objektech (trasy, stráci, dopravní prostøedky atd.) a druhá je urèena pro autentikaèní data o identitách a rolích uivatelù.

![C2 model](./prilohy/architektura/C2_model.png)
*C4 diagram Level 2: Kontejnery Plánovacího kalendáøe*
### Front-end webové aplikace
Front-end webové aplikace poskytuje aplikaèní logiku, jako napøíklad zobrazení plánu sluby, poskytnutí funkcionality plánování a spravování zdrojù. Vyuívá API backendu pro získání a updatování dat.
Aby mìl uivatel pøístup k datùm aplikace je autentizován. 

V diagramu front-end webové aplikace nejsou zaznamenány vztahy vedoucí z poskytovatelù za úèelem pøehlednosti. Poskytují kontext aplikace, kterı je primárnì dostupnı všem stránkám.

![C3 model](./prilohy/architektura/C3_model_frontend.png)
*C4 diagram Level 2: Front-end webové aplikace*

#### Browser router
Jedná se o react-router, kterı je odpovìdnı za navigaci mezi stránkami.

#### Services
Services obsahuje mnoinu fetch funkcí, které volají API backendu. 
Jsou uspoøádané do sloek podle objektù, kterıch se tıkají.

#### Poskytovatel Dat Plánù
Poskytovatel Dat Plánù je odpovìdnı za centrální spravování plánù v rámci vybraného a právì zobrazovaného mìsíce. 
Tyto plány poskytuje v rámci kontextu domovské stránce a stránce plánování. 
Pøi zmìnì zobrazovaného mìsíce je odpovìdnı za získání novıch plánù pomocí Services a pokud je provedena v plánech zmìna, notifikuje o tom pomocí Hub Plánù ostatní klienty.

#### Poskytovatel Dat Obvodu
Poskytovatel Dat Obvodu je odpovìdnı za centrální spravování zdrojù obvodu (tras, dopravních prostøedkù, strácù).
Tyto data poskytuje v rámci kontextu domovské stránce, stránce plánování a stránce správy zdrojù. 
Pokud je provedena v zdrojích zmìna, notifikuje o zmìnì ostatní klienty pouitím Hub Obvod-zdroje na backendu.

#### Poskytovatel Autentizace a autorizace
Poskytovatel autentizace a autorizace spravuje stav pøihlášení a ovìøuje roli uivatele, tento stav v rámci kontextu poskytuje všem stránkám.
Volá funkce z Services spojené s pøihlašováním a odhlašováním.

#### Pøihlašovací stránka
Pøihlašovací stránka je odpovìdná za zobrazení pøihlašovacího formuláøe a navigace na hlavní stránku pøi úspìšném pøihlášení.
Vyuívá funkci Poskytovatele Autentizace a autorizace pro pøihlášení uivatele.

#### Menu
Menu poskytuje odkazy na stránky a funkci odhlášení
Je odpovìdná zobrazit link na stránku správa zdrojù pouze vedoucím stráního obvodu.

#### Stránka plánování
Stránka plánování je zodpovìdná za zobrazení plánu sluby na vybranı mìsíc a umonit kontrolované editování a zamykání plánù.
Vyuívá Poskytovatele Dat Obvodu, aby zobrazoval aktuální informace o zdrojích.
Poskytovatele autentizace a autorizace, aby mohl uivatel editovat pouze pro nìj povolené plány.
Poskytovatele Dat Plánù vyuívá na získání a správu zobrazovanıch plánù.

#### Stránka správy zdrojù
Stránka správy zdrojù je zodpovìdná za zobrazení tras, strácù a dopravních prostøedkù se všemi informacemi a je pøístupná pouze pro vedoucí stráních obvodù.
Umoòuje vedoucím pøidávat, mazat a editovat zdroje obvodu. 
Vyuívá Poskytovatele Dat Obvodu pro šíøení zmìn napøíè stránkami a do backendu. 

#### Domovská stránka
Domovská stránka je zodpovìdná za zobrazování plánu sluby urèitého obvodu na urèitı den.

### Backend aplikace
Backend slouí na zpracování poadavkù frontendu a manipulaci s daty v databázi.
Poskytuje business logiku aplikace, jako napøíklad pøidání trasy do plánu stráce, ovìøení identity uivatele nebo získání dopravních prostøedkù v daném obvodu.

Obsahuje:
- Kontrolery - Implementují API, které zpracovávají poadavky frontendu a posílají data.
- Unit of Work - Komponenta, která koordinuje pøístup k repozitáøùm a jednotnì ukládá zmìny do databáze.
- Repozitáøe - Poskytují metody pro pøístup a práci s objekty v databázi.
- Huby - Slouí pro posílání informací o provedenıch zmìnách, zajišují komunikaci mezi serverem a klientem v reálném èase.
- Autorizaèní a autentikaèní logiku

![C3 model](./prilohy/architektura/C3_model_backend.png)
*C4 diagram Level 3: Backend*

#### Kontrolery
**Kontrolery zdrojù** (trasy, stráci, obvody a dopravní prostøedky) obsahují primárnì pouze HTTP metody pro základní CRUD operace - tvorba novıch, získání všech v daném obvodu, aktualizaci a mazání objektù.
Vıjimkou je kontroler strácù, kterı také obsahuje metodu na získání stráce, kterı je pøiøazenı k aktuálnì pøihlášenému uivateli (pokud je uivatel strácem).

**Kontroler Plánù** poskytuje API pro pøidávání a odebírání tras nebo dopravních prostøedkù ze specifickıch plánù, zamknutí a odemknutí plánù v urèitém dnu a získání plánù v urèitém èasovém rozmezí.

**Kontroler Uivatelù** poskytuje API pro pøihlašování a odhlašování uivatelù, registraci novıch uivatelù a správu rolí.

#### Unit of Work
Unit of work je tøída, která koordinuje práci repozitáøù a zajišuje, e všechny repozitáøe pracují se stejnou reprezentací databáze a tedy udruje konzistenci dat.
Umoòuje uloit všechny zmìny provedené v reprezentaci databáze do reálné databáze v jednom atomickém kroku.

#### Repozitáøe
Repozitáøe jsou zodpovìdné za pøístup k datùm v databázi, vyuívají Entity Framework a jeho DbContext pro usnadnìní práce s databází.
DbContext zajišuje mapování mezi objekty v aplikaci a tabulkami v databázi, co umoòuje snadnìjší práci s daty.
General Repository je obecná šablona, která implementuje rozhraní pro získávání, ukládání, mazání a aktualizaci objektù v databázi.
Je vyuit pro správu všech objektù kromì plánù. 
Plány jsou spravovány v Plan Repository, kterı implementuje stejné rozhraní, ale má specifickou metodu pro získávání plánù.

#### Huby
Huby jsou implementované pomocí SignalR a slouí k posílání informací o provedenıch zmìnách a tím udrují plán sluby a informace o objektech aktuální napøíè všemi klienty.

**Hub Plány** slouí k posílání notifikací o zmìnách konkrétního plánu skupinì klientù.
Umoòuje klientùm pøidat se do skupiny podle obvodu, ve kterém jsou, a mìsíce, kterého plány sledují. 
Díky skupinám jsou notifikace o zmìnách posílány pouze klientùm, kterıch se zmìny tıkají.

**Hub Obvod-zdroje** slouí k posílání notifikací o zmìnách ve zdrojích skupinì klientù. 
Umoòuje klinetùm pøidat se do skupiny podle obvodu. 
Notifikaèní metody jsou ètyøi - pro posílání zmìn o trasách, dopravních prostøedkách, strácích a zámcích.

#### Autentikace a autorizace
Autentikace a autorizace je zodpovìdná za pøístup k datùm v Auth databázi a vyuívá IdentityDbContext pro usnadnìní autorizace a autentikace. 
Autentikaèní sluba implementuje metody pøihlašování, odhlašování, registrování novıch uivatelù a získání informací o pøihlášeném uviateli.
Autorizaèní sluba implementuje metody pro pøiøazování a ovìøování rolí uivatelù.
