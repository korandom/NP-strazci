# Webová aplikace na plánování služby strážcù národního parku
## Obsah
1. [Základní souhrn](#základní-souhrn)
2. [Uživatelé](#uživatelé)
3. [Funkèní požadavky](#funkèní-požadavky)
    - [Plánování tras](#plánování-tras) 
    - [Správa objektù a zdrojù](#správa-objektù-a-zdrojù)
4. [Požadavky kvality](#požadavky-kvality)
5. [Pøípadné rozšíøení](#pøípadné-rozšíøení)
6. [Doménový model](#doménový-model)

## Základní souhrn

Webová aplikace na plánování tras, správu a pøidìlování zdrojù pro strážce.
1. **Mobilní rozhraní**
    - Primárnì urèeno pro strážce.
    Každý strážce si mùže plánovat docházku, zobrazovat plán všech strážcù v obvodu a pøidat poznámku dne.
    - [Design1](https://www.figma.com/file/B96k04ObDK4yaycN5ulT1I/NP-strazci?node-id=0%3A1&t=VeApoPs8S3MXuRlP-1)
2. **Webové rozhraní**
    - Urèeno pro vedoucí i strážce.
    Poskytuje nadmnožinu funkcí mobilního rozhraní.
    Umožòuje vedoucímu zobrazovat, uzamykat, mìnit docházku strážcù pod jeho dohledem.
    Umožòuje vedoucímu generovat plán tras podle jejich priorit a vyplnìné docházky strážcù pod jeho dohledem, a tento plán manuálnì mìnit.
    Umožòuje vedoucímu obvodu spravovat objekty a jejich atributy (trasy, dopravní prostøedky, strážci).


## Uživatelé 
1. Strážce
    - Strážce je zamìstnanec národního parku, který má za úkol chránit pøírodní území národního parku.
    - Má povinnost úèastnit se plánování a pøedávat informace z terénu pomocí denní poznámky.
1. Vedoucí strážního okrsku
   - Vedoucí strážního okrsku je strážce, který má povinnost vytváøet plán tras, pøiøazovat dopravní prostøedky a reagovat na denní poznámky strážcù okrsku. 
1. Vedoucí strážního obvodu
   - Vedoucí strážního obvodu je strážce, který má možnost vytváøet plán tras, pøiøazovat dopravní prostøedky a reagovat na denní poznámky strážcù obvodu.
1. Vedoucí strážní služby
   - Vedoucí strážní služby je zamìstnanec národního parku, který má pøehled o práci strážcù ze všech obvodù, má pøístup k datùm, ale nezasahuje do nich.
1. Námìstek øeditele
   - Námìstek øeditele má stejné možnosti používání aplikace jako vedoucí strážní služby.
1. Øeditel
   - Øeditel má stejné možnosti používání aplikace jako vedoucí strážní služby.

------------------------
## Funkèní požadavky

### Plánování tras

Jako **strážce** si chci zobrazit plán tras, abych mìl pøehled. ([issue #18](https://github.com/korandom/NP-strazci/issues/18))
- Jako **strážce** chci mít rychlý pøístup k plánu tras strážcù z okrsku, abych mohl snadno zjistit umístìní mých kolegù a ovìøit si mùj plán.
- Jako **strážce** chci mít možnost rozšíøit plán na strážce z obvodu, protože v krizové situaci mohu potøebovat informaci o pozici všech strážcù v obvodu. 
- Jako **strážce** chci, aby se na mobilní obrazovce zobrazoval plán v rámci jednoho vybraného dne a pouze strážcù, kteøí jsou v práci, protože je pro mì dùležitá pøehlednost a èitelnost plánu.
- Jako **strážce** chci, aby se na poèítaèové obrazovce zobrazoval plán na vybraných 14 dní, protože to je rozsah plánování do budoucna a na velké obrazovce chci mít širší pøehled.

Jako **vedoucí strážního obvodu/okrsku** chci generovat týdenní plán tras, který odpovídá docházce strážcù, prioritám tras a spravedlivému rozdìlení tras mezi strážce v rámci mìsíce, protože strážci musejí být s rozdìlením tras spokojení a požadavky na èetnost prùchodù tras v rámci priorit naplnìny.  ([issue #6](https://github.com/korandom/NP-strazci/issues/6))
- Jako **vedoucí strážního obvodu/okrsku** chci vygenerovaný plán kdykoliv upravit, protože mùže nastat neoèekávaná situace a plán musí být aktuální.
 
Jako **vedoucí strážního obvodu/okrsku** chci mít možnost pøiøadit nìjakému strážci na den nìjaký dopravní prostøedek, protože jich je omezené množství a musím je rozdìlovat podle potøeby. ([issue #14](https://github.com/korandom/NP-strazci/issues/14))

Jako **vedoucí strážní služby**, **námìstek øeditele**, **øeditel** chci mít možnost vybrat si obvod a zobrazit jeho informace, abych si mohl zobrazit plán služby a mìl všeobecný pøehled. ([issue #19](https://github.com/korandom/NP-strazci/issues/19))

 ### Správa zdrojù obvodu
Jako **vedoucí strážního obvodu** chci mít možnost pøidat, upravit a smazat trasu z kolekce, protože informace tras se mohou mìnit a data musí být aktuální pro automatizované generování i pro zobrazování plánu tras. ([issue #7](https://github.com/korandom/NP-strazci/issues/7))
 - Jako **vedoucí strážního obvodu** chci mít možnost urèit a zmìnit, která trasa je kontrolní a v jakém èase probíhá kontrola, protože požadavky na kontrolní místa se mohou zmìnit. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))
 
Jako **vedoucí strážního obvodu** chci mít možnost pøidat nebo smazat dopravní prostøedek z kolekce, protože aktuální poèet nebo jejich druh se mùže zmìnit. ([issue #16](https://github.com/korandom/NP-strazci/issues/16))
 
Jako **vedoucí strážního obvodu** chci mít možnost pøidat nebo smazat nebo editovat informace okrskù, aby informace o okrskách byli aktuální. ([issue #21](https://github.com/korandom/NP-strazci/issues/21))

Jako **vedoucí strážního obvodu** chci mít možnost pøidávat a mazat strážce z mého obvodu, aby se v plánu zmìny promítali. ([issue #20](https://github.com/korandom/NP-strazci/issues/20))

### Docházka
 Jako **strážce** si chci naplánovat, které dny v jaký èas budu sloužit, aby mi vedoucí mohl naplánovat trasy a plán odpovídal mým potøebám. ([issue #1](https://github.com/korandom/NP-strazci/issues/1))
 - Jako **strážce** chci pøidat dùvod nepøítomnosti, pokud se plánování v daný den nebudu úèastnit, protože potøebuji schválení od vedoucího. ([issue #3](https://github.com/korandom/NP-strazci/issues/3))
 
 Jako **vedoucí strážního obvodu/okrsku** chci zmìnit naplánovanou docházku strážcù, aby docházka odpovídala mým požadavkùm a zmìnám. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))
 - Jako **vedoucí strážního obvodu/okrsku** chci naplánovanou docházku strážcù uzamknout, aby po mém schválení nemohli strážci zasahovat a další zmìny museli být provedeny pouze mnou.

### Poznámka ke dni
 Jako **strážce** chci mít možnost ohodnotit den ve formì poznámky, protože chci poznamenat události, které se staly a pøedat zajímavé informace vedoucímu. ([issue #5](https://github.com/korandom/NP-strazci/issues/5))

 Jako **strážce** chci vidìt denní poznámky ostatních strážcù, abych se dozvìdìl potøebné informace a mohl na nì reagovat. ([issue #17](https://github.com/korandom/NP-strazci/issues/17))

 
 ## Požadavky kvality

 - Mobilní uživatelské rozhraní musí být intuitivní, s dostateènì velkým textem, aby se dalo ovládat s minimálním zauèením i pro netechnicky založené lidi. 
 
 -----------------------------

 ## Doménový model
 - [Doménový model verze](prilohy/domenovy_model.svg)


