# Webov� aplikace na pl�nov�n� slu�by str�c� n�rodn�ho parku
## Obsah
1. [Z�kladn� souhrn](#z�kladn�-souhrn)
2. [U�ivatel�](#u�ivatel�)
3. [Funk�n� po�adavky](#funk�n�-po�adavky)
    - [Pl�nov�n� tras](#pl�nov�n�-tras) 
    - [Spr�va objekt� a zdroj�](#spr�va-objekt�-a-zdroj�)
4. [Po�adavky kvality](#po�adavky-kvality)
5. [P��padn� roz���en�](#p��padn�-roz���en�)
6. [Dom�nov� model](#dom�nov�-model)

## Z�kladn� souhrn

Webov� aplikace na pl�nov�n� tras, spr�vu a p�id�lov�n� zdroj� pro str�ce.
1. **Mobiln� rozhran�**
    - Prim�rn� ur�eno pro str�ce.
    Ka�d� str�ce si m��e pl�novat doch�zku, zobrazovat pl�n v�ech str�c� v obvodu a p�idat pozn�mku dne.
    - [Design1](https://www.figma.com/file/B96k04ObDK4yaycN5ulT1I/NP-strazci?node-id=0%3A1&t=VeApoPs8S3MXuRlP-1)
2. **Webov� rozhran�**
    - Ur�eno pro vedouc� i str�ce.
    Poskytuje nadmno�inu funkc� mobiln�ho rozhran�.
    Umo��uje vedouc�mu zobrazovat, uzamykat, m�nit doch�zku str�c� pod jeho dohledem.
    Umo��uje vedouc�mu generovat pl�n tras podle jejich priorit a vypln�n� doch�zky str�c� pod jeho dohledem, a tento pl�n manu�ln� m�nit.
    Umo��uje vedouc�mu obvodu spravovat objekty a jejich atributy (trasy, dopravn� prost�edky, str�ci).


## U�ivatel� 
1. Str�ce
    - Str�ce je zam�stnanec n�rodn�ho parku, kter� m� za �kol chr�nit p��rodn� �zem� n�rodn�ho parku.
    - M� povinnost ��astnit se pl�nov�n� a p�ed�vat informace z ter�nu pomoc� denn� pozn�mky.
1. Vedouc� str�n�ho okrsku
   - Vedouc� str�n�ho okrsku je str�ce, kter� m� povinnost vytv��et pl�n tras, p�i�azovat dopravn� prost�edky a reagovat na denn� pozn�mky str�c� okrsku. 
1. Vedouc� str�n�ho obvodu
   - Vedouc� str�n�ho obvodu je str�ce, kter� m� mo�nost vytv��et pl�n tras, p�i�azovat dopravn� prost�edky a reagovat na denn� pozn�mky str�c� obvodu.
1. Vedouc� str�n� slu�by
   - Vedouc� str�n� slu�by je zam�stnanec n�rodn�ho parku, kter� m� p�ehled o pr�ci str�c� ze v�ech obvod�, m� p��stup k dat�m, ale nezasahuje do nich.
1. N�m�stek �editele
   - N�m�stek �editele m� stejn� mo�nosti pou��v�n� aplikace jako vedouc� str�n� slu�by.
1. �editel
   - �editel m� stejn� mo�nosti pou��v�n� aplikace jako vedouc� str�n� slu�by.

------------------------
## Funk�n� po�adavky

### Pl�nov�n� tras

Jako **str�ce** si chci zobrazit pl�n tras, abych m�l p�ehled. ([issue #18](https://github.com/korandom/NP-strazci/issues/18))
- Jako **str�ce** chci m�t rychl� p��stup k pl�nu tras str�c� z okrsku, abych mohl snadno zjistit um�st�n� m�ch koleg� a ov��it si m�j pl�n.
- Jako **str�ce** chci m�t mo�nost roz���it pl�n na str�ce z obvodu, proto�e v krizov� situaci mohu pot�ebovat informaci o pozici v�ech str�c� v obvodu. 
- Jako **str�ce** chci, aby se na mobiln� obrazovce zobrazoval pl�n v r�mci jednoho vybran�ho dne a pouze str�c�, kte�� jsou v pr�ci, proto�e je pro m� d�le�it� p�ehlednost a �itelnost pl�nu.
- Jako **str�ce** chci, aby se na po��ta�ov� obrazovce zobrazoval pl�n na vybran�ch 14 dn�, proto�e to je rozsah pl�nov�n� do budoucna a na velk� obrazovce chci m�t �ir�� p�ehled.

Jako **vedouc� str�n�ho obvodu/okrsku** chci generovat t�denn� pl�n tras, kter� odpov�d� doch�zce str�c�, priorit�m tras a spravedliv�mu rozd�len� tras mezi str�ce v r�mci m�s�ce, proto�e str�ci musej� b�t s rozd�len�m tras spokojen� a po�adavky na �etnost pr�chod� tras v r�mci priorit napln�ny.  ([issue #6](https://github.com/korandom/NP-strazci/issues/6))
- Jako **vedouc� str�n�ho obvodu/okrsku** chci vygenerovan� pl�n kdykoliv upravit, proto�e m��e nastat neo�ek�van� situace a pl�n mus� b�t aktu�ln�.
 
Jako **vedouc� str�n�ho obvodu/okrsku** chci m�t mo�nost p�i�adit n�jak�mu str�ci na den n�jak� dopravn� prost�edek, proto�e jich je omezen� mno�stv� a mus�m je rozd�lovat podle pot�eby. ([issue #14](https://github.com/korandom/NP-strazci/issues/14))

Jako **vedouc� str�n� slu�by**, **n�m�stek �editele**, **�editel** chci m�t mo�nost vybrat si obvod a zobrazit jeho informace, abych si mohl zobrazit pl�n slu�by a m�l v�eobecn� p�ehled. ([issue #19](https://github.com/korandom/NP-strazci/issues/19))

 ### Spr�va zdroj� obvodu
Jako **vedouc� str�n�ho obvodu** chci m�t mo�nost p�idat, upravit a smazat trasu z kolekce, proto�e informace tras se mohou m�nit a data mus� b�t aktu�ln� pro automatizovan� generov�n� i pro zobrazov�n� pl�nu tras. ([issue #7](https://github.com/korandom/NP-strazci/issues/7))
 - Jako **vedouc� str�n�ho obvodu** chci m�t mo�nost ur�it a zm�nit, kter� trasa je kontroln� a v jak�m �ase prob�h� kontrola, proto�e po�adavky na kontroln� m�sta se mohou zm�nit. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))
 
Jako **vedouc� str�n�ho obvodu** chci m�t mo�nost p�idat nebo smazat dopravn� prost�edek z kolekce, proto�e aktu�ln� po�et nebo jejich druh se m��e zm�nit. ([issue #16](https://github.com/korandom/NP-strazci/issues/16))
 
Jako **vedouc� str�n�ho obvodu** chci m�t mo�nost p�idat nebo smazat nebo editovat informace okrsk�, aby informace o okrsk�ch byli aktu�ln�. ([issue #21](https://github.com/korandom/NP-strazci/issues/21))

Jako **vedouc� str�n�ho obvodu** chci m�t mo�nost p�id�vat a mazat str�ce z m�ho obvodu, aby se v pl�nu zm�ny prom�tali. ([issue #20](https://github.com/korandom/NP-strazci/issues/20))

### Doch�zka
 Jako **str�ce** si chci napl�novat, kter� dny v jak� �as budu slou�it, aby mi vedouc� mohl napl�novat trasy a pl�n odpov�dal m�m pot�eb�m. ([issue #1](https://github.com/korandom/NP-strazci/issues/1))
 - Jako **str�ce** chci p�idat d�vod nep��tomnosti, pokud se pl�nov�n� v dan� den nebudu ��astnit, proto�e pot�ebuji schv�len� od vedouc�ho. ([issue #3](https://github.com/korandom/NP-strazci/issues/3))
 
 Jako **vedouc� str�n�ho obvodu/okrsku** chci zm�nit napl�novanou doch�zku str�c�, aby doch�zka odpov�dala m�m po�adavk�m a zm�n�m. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))
 - Jako **vedouc� str�n�ho obvodu/okrsku** chci napl�novanou doch�zku str�c� uzamknout, aby po m�m schv�len� nemohli str�ci zasahovat a dal�� zm�ny museli b�t provedeny pouze mnou.

### Pozn�mka ke dni
 Jako **str�ce** chci m�t mo�nost ohodnotit den ve form� pozn�mky, proto�e chci poznamenat ud�losti, kter� se staly a p�edat zaj�mav� informace vedouc�mu. ([issue #5](https://github.com/korandom/NP-strazci/issues/5))

 Jako **str�ce** chci vid�t denn� pozn�mky ostatn�ch str�c�, abych se dozv�d�l pot�ebn� informace a mohl na n� reagovat. ([issue #17](https://github.com/korandom/NP-strazci/issues/17))

 
 ## Po�adavky kvality

 - Mobiln� u�ivatelsk� rozhran� mus� b�t intuitivn�, s dostate�n� velk�m textem, aby se dalo ovl�dat s minim�ln�m zau�en�m i pro netechnicky zalo�en� lidi. 
 
 -----------------------------

 ## Dom�nov� model
 - [Dom�nov� model verze](prilohy/domenovy_model.svg)


