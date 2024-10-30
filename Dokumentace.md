# Webová aplikace na plánování služby strážců národního parku
## Obsah
1. [Základní souhrn](#základní-souhrn)
2. [Uživatelé](#uživatelé)
3. [Funkční požadavky](#funkční-požadavky)
    - [Plánování tras](#plánování-tras) 
    - [Správa objektů a zdrojů](#správa-objektů-a-zdrojů)
4. [Požadavky kvality](#požadavky-kvality)
5. [Případné rozšíření](#případné-rozšíření)
6. [Doménový model](#doménový-model)

## Základní souhrn
Webová aplikace na plánování tras, správu a přidělování zdrojů pro strážce.
1. **Mobilní rozhraní**
    - Primárně určeno pro strážce.
    Každý strážce si může trasy plánovat, zobrazit plán všech strážců a přidat poznámku dne.
    - [Design1](https://www.figma.com/file/B96k04ObDK4yaycN5ulT1I/NP-strazci?node-id=0%3A1&t=VeApoPs8S3MXuRlP-1)
2. **Webové rozhraní**
    - Určeno pro vedoucí i strážce.
    Poskytuje nadmnožinu funkcí mobilního rozhraní.
    Umožňuje vedoucímu zobrazovat, uzamykat, měnit plán tras.
    Umožňuje vedoucímu spravovat objekty a jejich atributy (trasy, dopravní prostředky, strážci).


## Uživatelé 
1. Strážce
    - Strážce je zaměstnanec národního parku, který má za úkol chránit přírodní území národního parku.
    - Má povinnost účastnit se plánování tras a předávat informace z terénu pomocí denní poznámky.
1. Vedoucí strážního obvodu
   - Vedoucí strážního obvodu je strážce, který má povinnost vytvářet plán tras, přiřazovat dopravní prostředky a reagovat na denní poznámky.
1. Vedoucí strážní služby
   - Vedoucí strážní služby je zaměstnanec národního parku, který má přehled o práci strážců ze všech obvodů, má přístup k datům, ale nezasahuje do nich.
1. Náměstek ředitele
   - Náměstek ředitele má stejné možnosti používání aplikace jako vedoucí strážní služby.
1. Ředitel
   - Ředitel má stejné možnosti používání aplikace jako vedoucí strážní služby.

------------------------
## Funkční požadavky

### Plánování tras

 - Jako **strážce** si chci zobrazit plán tras na nadcházející den, či týden, abych měl přehled. ([issue #18](https://github.com/korandom/NP-strazci/issues/18))
 
 - Jako **strážce** chci, aby se v utvořeném plánu zobrazili pouze strážci, kteří v daný den nebo týden jdou do práce, aby byl plán přehlednější. ([issue #18](https://github.com/korandom/NP-strazci/issues/18))

 - Jako **strážce** si chci na určitý den naplánovat množinu tras, kterou projdu, aby můj vedoucí věděl, kam mám v plánu jít a aby si moji kolegové nenaplánovali stejné trasy jako já. ([issue #4](https://github.com/korandom/NP-strazci/issues/4))

 - Jako **strážce** chci vidět naplánované trasy mých kolegů, protože si nechci naplánovat stejné trasy. ([issue #4](https://github.com/korandom/NP-strazci/issues/4))

 - Jako **vedoucí strážního obvodu** chci mít možnost upravit naplánované trasy strážců a uzamknout změny tak, aby strážci do změn nemohli zasahovat, protože pokud nejsem spokojen s naplánovými trasami, chci, aby moje změny byli následovány. ([issue #6](https://github.com/korandom/NP-strazci/issues/6))

 - Jako **vedoucí strážního obvodu** chci mít možnost zámek na plánech odemknout a provést další změny, protože může nastat nečekaná situace, při které to bude potřeba. ([issue #6](https://github.com/korandom/NP-strazci/issues/6))
 
 - Jako **vedoucí strážního obvodu** chci mít možnost přiřadit nějakému strážci na den nějaký dopravní prostředek, protože jich je omezené množství a musím je rozdělovat podle potřeby. ([issue #14](https://github.com/korandom/NP-strazci/issues/14))

 - Jako **vedoucí strážní služby**, **náměstek ředitele**, **ředitel** chci mít možnost vybrat si obvod a zobrazit jeho informace, abych si mohl zobrazit plán tras. ([issue #19](https://github.com/korandom/NP-strazci/issues/19))

 ### Správa objektů a zdrojů 

 - Jako **vedoucí strážního obvodu** chci dát informaci strážcům o důležitosti tras, protože důležitost tras se mění a pokud strážci ví, jaké trasy jsou důležité, naplánují si trasy správněji. ([issue #7](https://github.com/korandom/NP-strazci/issues/7))
 
 - Jako **vedoucí strážního obvodu** chci mít možnost přidat nebo smazat dopravní prostředek z kolekce, protože aktuální počet nebo jejich druh se může změnit. ([issue #16](https://github.com/korandom/NP-strazci/issues/16))
 
 - Jako **vedoucí strážního obvodu** chci mít možnost přidat nebo smazat nebo editovat informace okrsků, nastavit barvy, aby informace o okrskách byli aktuální a barvy byli podle mých představ. ([issue #21](https://github.com/korandom/NP-strazci/issues/21))

 -  Jako **vedoucí strážního obvodu** chci mít možnost přidávat a mazat strážce z mého obvodu. ([issue #20](https://github.com/korandom/NP-strazci/issues/20))

 - Jako **vedoucí strážního obvodu** chci mít možnost ve výjmečné situaci určit a změnit, která trasa je kontrolní a v jakém čase probíhá kontrola, protože požadavky na kontrolní místa se mohou změnit. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))

 ### Automatické plánování s preferencemi 

 - Vedoucí strážního obvodu může automaticky generovat plány, které se řídí podle preferencí a priorit tras.
 - Vedoucí strážního obvodu může plány měnit, pokud je to potřeba.
 - Strážce si může nastavit a editovat preference na trasy.

 ## Požadavky kvality

 - Mobilní uživatelské rozhraní musí být intuitivní, s dostatečně velkým textem, aby se dalo ovládat s minimálním zaučením i pro netechnicky založené lidi. 
 - Zabezpečení mobilní aplikace - základní požadavky: šifrovaná a zabezpečená komunikace s WS backendem, ostatní zabezpečení na úrovni zabezpečení telefonu (Výhradně OS Android 11+)
 - Zabezpečení webové aplikace - Výhradně on-premise řešení, šifrování, poddoména "npsumava.cz", autorizace a autentizace uživatelů přes LDAP organizace, mfa 
 -----------------------------

 ## Případné rozšíření

 ### Vizualizace tras na mapě
 - Zobrazení strážcemi pokrytých míst na mapě v konkréntí den.

 ### Použité vybavení strážce na den
 - Mezi vybavení patří kolo, běžecké lyže, sněžníce, skialpy, záznamové zařízení (osobní kamera), možnost přidat další.
 - Střážce může vybrat, které z vybavení v daný den použil, součástí docházky.
 - Vedoucí strážního obvodu si může zobrazit četnost užití určitého vybavení v určitém časovém úseku.
 - Slouží pro posouzení potřeby a užití vybavení.

 ### Docházka 
 - Jako **strážce** si chci zaznamenat počet odpracovaných hodin na určitý den, protože musím tuto informaci předat vedoucímu. ([issue #1](https://github.com/korandom/NP-strazci/issues/1))
 
 - Jako **strážce** chci zaznamenat, jestli mám odpracované nějaké hodiny na den v rámci JPO - Jednotky požární ochrany, protože je to separátní činnost a tyto hodiny se počítají odděleně. ([issue #12](https://github.com/korandom/NP-strazci/issues/12))
 
 - Jako **strážce** chci, aby se odpracované hodiny zaznamenané o víkendech nebo svátcích započítaly do náhradního volna, protože chci vědět, kolik hodin náhradního volna mohu využít v budoucnu. ([issue #2](https://github.com/korandom/NP-strazci/issues/2))

 - Jako **strážce** chci, abych si mohl zaznamenat hodiny přesčasů, a abych si je mohl také vybírat, pokud potřeba, protože chci mít přehled o mých přesčasech a chci si sám flexibilně plánovat pracovní dobu. ([issue #13](https://github.com/korandom/NP-strazci/issues/13))
 
 - Jako **strážce** si chci naplánovat náhradní volno nebo dovolenou, protože potřebuji schválení od vedoucího. ([issue #3](https://github.com/korandom/NP-strazci/issues/3))

 - Jako **strážce** chci mít možnost ohodnotit den ve formě poznámky, protože chci poznamenat události, které se staly a předat zajímavé informace vedoucímu. ([issue #5](https://github.com/korandom/NP-strazci/issues/5))

 - Jako **vedoucí strážního obvodu** chci vidět denní poznámky strážců, abych se dozvěděl potřebné informace a mohl na ně reagovat. ([issue #17](https://github.com/korandom/NP-strazci/issues/17))

 - Jako **vedoucí strážního obvodu** chci vidět počet odpracovaných hodin jednotlivých strážců v rámci měsíce i v jednotlivé dny, protože musím mít přehled o mých podřízených. ([issue #9](https://github.com/korandom/NP-strazci/issues/9))

  ### Statistiky tras ([issue #8](https://github.com/korandom/NP-strazci/issues/8))
 
 - Jako **vedoucí strážního obvodu** chci zobrazit vizualizaci (graf) počtu průchodů tras v rámci určitého období, protože chci vědět frekvenci procházení tras.
 
 - Jako **vedoucí strážního obvodu** chci zobrazit vizualizaci (graf) - kolikrát byla určitá trasa navštívena jakými strážci v rámci určitého období, protože chci vědět, jestli je trasa plánovaná pro strážce přiměřeně rovnoměrně.

 - Jako **vedoucí strážního obvodu** chci zobrazit vizualizaci (graf) - kolikrát prošel jaké trasy určitý strážce v rámci určitého období, protože chci vědět, jaké trasy jsou pro určitého strážce plánované nejvíce a nejméně často.

 ## Doménový model
 - [Doménový model verze](prilohy/domenovy_model.svg)


