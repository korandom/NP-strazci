# Pl�nov�n� slu�by str�c�
Tento dokument obsahuje popis architektury webov� aplikace pro pl�nov�n� slu�by str�c�.

## Obsah
- [Pl�nov�n� slu�by str�c�](#planovani-sluzby-strazcu)
    - [Obsah](#obsah)
    - [�vod](#uvod)
    - [Z�kladn� informace o syst�mu](#zakladni-informace-o-systemu)
    - [Front-end webov� aplikace](#front-end-webove-aplikace)
        - [Browser router](#browser-router)
        - [Services](#Services)
        - [Poskytovatel Dat Pl�n�](#poskytovatel-dat-planu)
        - [Poskytovatel Dat Obvodu](#poskytovatel-dat-obvodu)
        - [Poskytovatel Autentizace a autorizace](#poskytovatel-autentizace-a-autorizace)
        - [P�ihla�ovac� str�nka](#prihlasovaci-stranka)
        - [Menu](#menu)
        - [Str�nka pl�nov�n�](#stranka-planovani)
        - [Str�nka spr�vy zdroj�](#stranka-spravy-zdroju)
        - [Domovsk� str�nka](#domovska-stranka)
    - [Backend aplikace](#backend-aplikace)
        - [Kontrolery](#kontrolery)
        - [Unit of Work](#unit-of-work)
        - [Repozit��e](#repozitare)
        - [Huby](#huby)
        - [Autentikace a autorizace](#autentikace-a-autorizace)



## �vod
Aplikace Pl�novac� kalend�� je ur�ena pro str�ce a vedouc� str�n�ch obvod� n�rodn�ho parku. 
Str�c�m umo��uje zobrazit si pl�n slu�by a napl�novat si trasy.
Vedouc�m str�n�ch obvod� umo��uje vytv��et pl�n slu�by a spravovat objekty v obvodu.

![C1 model](./prilohy/architektura/C1_model.png)
*C4 diagram Level 1: Pl�novac� kalend��*

## Z�kladn� informace o syst�mu
Pl�novac� kalend�� je webov� aplikace, kter� slou�� k pl�nov�n� slu�by str�c� v re�ln�m �ase a zobrazov�n� pl�n� na den.
Aplikace poskytne klientovi front-end, kter� umo��uje u�ivateli interagovat se syst�mem a m�nit pl�n slu�by.
Front-end webov� aplikace vyu��v� knihovnu React s Typescriptem. 
U�ivatelsk� rozhran� zobrazuje a pracuje s daty, kter� jsou na�ten� z backendu ASP.NET Core prost�ednict�m vol�n� API kontroler�. 
Aby byla data aktu�ln� p�es v�echna za��zen�, je backend vyu�it tak� na pos�l�n� informac� o proveden�ch zm�n�ch.
Backend �te a zapisuje data do dvou datab�z� MariaDB: jedna obsahuje aplika�n� data o objektech (trasy, str�ci, dopravn� prost�edky atd.) a druh� je ur�ena pro autentika�n� data o identit�ch a rol�ch u�ivatel�.

![C2 model](./prilohy/architektura/C2_model.png)
*C4 diagram Level 2: Kontejnery Pl�novac�ho kalend��e*
### Front-end webov� aplikace
Front-end webov� aplikace poskytuje aplika�n� logiku, jako nap��klad zobrazen� pl�nu slu�by, poskytnut� funkcionality pl�nov�n� a spravov�n� zdroj�. Vyu��v� API backendu pro z�sk�n� a updatov�n� dat.
Aby m�l u�ivatel p��stup k dat�m aplikace je autentizov�n. 

V diagramu front-end webov� aplikace nejsou zaznamen�ny vztahy vedouc� z poskytovatel� za ��elem p�ehlednosti. Poskytuj� kontext aplikace, kter� je prim�rn� dostupn� v�em str�nk�m.

![C3 model](./prilohy/architektura/C3_model_frontend.png)
*C4 diagram Level 2: Front-end webov� aplikace*

#### Browser router
Jedn� se o react-router, kter� je odpov�dn� za navigaci mezi str�nkami.

#### Services
Services obsahuje mno�inu fetch funkc�, kter� volaj� API backendu. 
Jsou uspo��dan� do slo�ek podle objekt�, kter�ch se t�kaj�.

#### Poskytovatel Dat Pl�n�
Poskytovatel Dat Pl�n� je odpov�dn� za centr�ln� spravov�n� pl�n� v r�mci vybran�ho a pr�v� zobrazovan�ho m�s�ce. 
Tyto pl�ny poskytuje v r�mci kontextu domovsk� str�nce a str�nce pl�nov�n�. 
P�i zm�n� zobrazovan�ho m�s�ce je odpov�dn� za z�sk�n� nov�ch pl�n� pomoc� Services a pokud je provedena v pl�nech zm�na, notifikuje o tom pomoc� Hub Pl�n� ostatn� klienty.

#### Poskytovatel Dat Obvodu
Poskytovatel Dat Obvodu je odpov�dn� za centr�ln� spravov�n� zdroj� obvodu (tras, dopravn�ch prost�edk�, str�c�).
Tyto data poskytuje v r�mci kontextu domovsk� str�nce, str�nce pl�nov�n� a str�nce spr�vy zdroj�. 
Pokud je provedena v zdroj�ch zm�na, notifikuje o zm�n� ostatn� klienty pou�it�m Hub Obvod-zdroje na backendu.

#### Poskytovatel Autentizace a autorizace
Poskytovatel autentizace a autorizace spravuje stav p�ihl�en� a ov��uje roli u�ivatele, tento stav v r�mci kontextu poskytuje v�em str�nk�m.
Vol� funkce z Services spojen� s p�ihla�ov�n�m a odhla�ov�n�m.

#### P�ihla�ovac� str�nka
P�ihla�ovac� str�nka je odpov�dn� za zobrazen� p�ihla�ovac�ho formul��e a navigace na hlavn� str�nku p�i �sp�n�m p�ihl�en�.
Vyu��v� funkci Poskytovatele Autentizace a autorizace pro p�ihl�en� u�ivatele.

#### Menu
Menu poskytuje odkazy na str�nky a funkci odhl�en�
Je odpov�dn� zobrazit link na str�nku spr�va zdroj� pouze vedouc�m str�n�ho obvodu.

#### Str�nka pl�nov�n�
Str�nka pl�nov�n� je zodpov�dn� za zobrazen� pl�nu slu�by na vybran� m�s�c a umo�nit kontrolovan� editov�n� a zamyk�n� pl�n�.
Vyu��v� Poskytovatele Dat Obvodu, aby zobrazoval aktu�ln� informace o zdroj�ch.
Poskytovatele autentizace a autorizace, aby mohl u�ivatel editovat pouze pro n�j povolen� pl�ny.
Poskytovatele Dat Pl�n� vyu��v� na z�sk�n� a spr�vu zobrazovan�ch pl�n�.

#### Str�nka spr�vy zdroj�
Str�nka spr�vy zdroj� je zodpov�dn� za zobrazen� tras, str�c� a dopravn�ch prost�edk� se v�emi informacemi a je p��stupn� pouze pro vedouc� str�n�ch obvod�.
Umo��uje vedouc�m p�id�vat, mazat a editovat zdroje obvodu. 
Vyu��v� Poskytovatele Dat Obvodu pro ���en� zm�n nap��� str�nkami a do backendu. 

#### Domovsk� str�nka
Domovsk� str�nka je zodpov�dn� za zobrazov�n� pl�nu slu�by ur�it�ho obvodu na ur�it� den.

### Backend aplikace
Backend slou�� na zpracov�n� po�adavk� frontendu a manipulaci s daty v datab�zi.
Poskytuje business logiku aplikace, jako nap��klad p�id�n� trasy do pl�nu str�ce, ov��en� identity u�ivatele nebo z�sk�n� dopravn�ch prost�edk� v dan�m obvodu.

Obsahuje:
- Kontrolery - Implementuj� API, kter� zpracov�vaj� po�adavky frontendu a pos�laj� data.
- Unit of Work - Komponenta, kter� koordinuje p��stup k repozit���m a jednotn� ukl�d� zm�ny do datab�ze.
- Repozit��e - Poskytuj� metody pro p��stup a pr�ci s objekty v datab�zi.
- Huby - Slou�� pro pos�l�n� informac� o proveden�ch zm�n�ch, zaji��uj� komunikaci mezi serverem a klientem v re�ln�m �ase.
- Autoriza�n� a autentika�n� logiku

![C3 model](./prilohy/architektura/C3_model_backend.png)
*C4 diagram Level 3: Backend*

#### Kontrolery
**Kontrolery zdroj�** (trasy, str�ci, obvody a dopravn� prost�edky) obsahuj� prim�rn� pouze HTTP metody pro z�kladn� CRUD operace - tvorba nov�ch, z�sk�n� v�ech v dan�m obvodu, aktualizaci a maz�n� objekt�.
V�jimkou je kontroler str�c�, kter� tak� obsahuje metodu na z�sk�n� str�ce, kter� je p�i�azen� k aktu�ln� p�ihl�en�mu u�ivateli (pokud je u�ivatel str�cem).

**Kontroler Pl�n�** poskytuje API pro p�id�v�n� a odeb�r�n� tras nebo dopravn�ch prost�edk� ze specifick�ch pl�n�, zamknut� a odemknut� pl�n� v ur�it�m dnu a z�sk�n� pl�n� v ur�it�m �asov�m rozmez�.

**Kontroler U�ivatel�** poskytuje API pro p�ihla�ov�n� a odhla�ov�n� u�ivatel�, registraci nov�ch u�ivatel� a spr�vu rol�.

#### Unit of Work
Unit of work je t��da, kter� koordinuje pr�ci repozit��� a zaji��uje, �e v�echny repozit��e pracuj� se stejnou reprezentac� datab�ze a tedy udr�uje konzistenci dat.
Umo��uje ulo�it v�echny zm�ny proveden� v reprezentaci datab�ze do re�ln� datab�ze v jednom atomick�m kroku.

#### Repozit��e
Repozit��e jsou zodpov�dn� za p��stup k dat�m v datab�zi, vyu��vaj� Entity Framework a jeho DbContext pro usnadn�n� pr�ce s datab�z�.
DbContext zaji��uje mapov�n� mezi objekty v aplikaci a tabulkami v datab�zi, co� umo��uje snadn�j�� pr�ci s daty.
General Repository je obecn� �ablona, kter� implementuje rozhran� pro z�sk�v�n�, ukl�d�n�, maz�n� a aktualizaci objekt� v datab�zi.
Je vyu�it pro spr�vu v�ech objekt� krom� pl�n�. 
Pl�ny jsou spravov�ny v Plan Repository, kter� implementuje stejn� rozhran�, ale m� specifickou metodu pro z�sk�v�n� pl�n�.

#### Huby
Huby jsou implementovan� pomoc� SignalR a slou�� k pos�l�n� informac� o proveden�ch zm�n�ch a t�m udr�uj� pl�n slu�by a informace o objektech aktu�ln� nap��� v�emi klienty.

**Hub Pl�ny** slou�� k pos�l�n� notifikac� o zm�n�ch konkr�tn�ho pl�nu skupin� klient�.
Umo��uje klient�m p�idat se do skupiny podle obvodu, ve kter�m jsou, a m�s�ce, kter�ho pl�ny sleduj�. 
D�ky skupin�m jsou notifikace o zm�n�ch pos�l�ny pouze klient�m, kter�ch se zm�ny t�kaj�.

**Hub Obvod-zdroje** slou�� k pos�l�n� notifikac� o zm�n�ch ve zdroj�ch skupin� klient�. 
Umo��uje klinet�m p�idat se do skupiny podle obvodu. 
Notifika�n� metody jsou �ty�i - pro pos�l�n� zm�n o tras�ch, dopravn�ch prost�edk�ch, str�c�ch a z�mc�ch.

#### Autentikace a autorizace
Autentikace a autorizace je zodpov�dn� za p��stup k dat�m v Auth datab�zi a vyu��v� IdentityDbContext pro usnadn�n� autorizace a autentikace. 
Autentika�n� slu�ba implementuje metody p�ihla�ov�n�, odhla�ov�n�, registrov�n� nov�ch u�ivatel� a z�sk�n� informac� o p�ihl�en�m u�viateli.
Autoriza�n� slu�ba implementuje metody pro p�i�azov�n� a ov��ov�n� rol� u�ivatel�.
