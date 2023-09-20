### Plánovací kalendář pro strážce NP Šumava
Mobilní aplikace na záznam pracovní doby a naplánovaných tras pro strážce. 
Jako strážce si mohu naplánovat trasy (trasy jsou reprezentovány číselnými kódy, tedy množina číselných kódů)  na určitý den a zaznamenat pracovní dobu. Jako strážce mám možnost na konci týdne zhodnotit týden ve formě poznámky. 
Jako nadřízený strážce mohu strážcům upravit naplánované trasy a zamknout změny tak, aby sám strážce již do plánu nezasahoval, ale já sám mohu zámek odemknout a plán změnit pokud je třeba. Jako nadřízený mohu také generovat statistiky pro jednotlivé trasy za určité období, neboli filtrovat podle tras. Mohu také kontrolovat odpracované doby pro jednotlivé strážce v rámci měsíce. Všechny data by měli být uložené způsobem, aby se dali exportovat ve formátu excelové tabulky. 

___
[Design pro mobilní aplikaci](https://www.figma.com/file/B96k04ObDK4yaycN5ulT1I/NP-strazci?node-id=0%3A1&t=VeApoPs8S3MXuRlP-1)

---
#### Doménový model
[Doménový model](prilohy/domenovy_model.png)
```plantuml
@startuml
class "Nadřízený strážce"
class Strážce {
  jméno
  příjmení
}
class "Plán Tras"
class Poznámka {
  text
}
class Trasa{
  jméno
  číslo
  bool kontrolní
}
class Kontrola{
  čas
  místo
}
class "Přiřazení auta" 
class Auto {
  id
  model
}
class Den  

"Nadřízený strážce" <|-U-  Strážce

"Nadřízený strážce" "1" -- "0..*"  "Přiřazení auta" : > zařídit
"Nadřízený strážce" "1" -- "0..*"  "Plán Tras" : > změnit/zamknout

Den "0..*" -- "1"  Poznámka  : < na
Den "1" -- "0..*"  "Plán Tras" : < na

"Plán Tras" "0..*" -- "0..*" Trasa : > 
"Přiřazení auta" "0..*" -- "1"  Den : > na
"Přiřazení auta" "0..*" -- "1"  Auto : >
Trasa "1" -- "0..1" Kontrola : > na

Strážce "1" -- "0..*"  Poznámka : > editovat
Strážce "1" -- "0..*"  "Plán Tras" : > změnit
Strážce "1" -- "0..*"  "Plán Tras" : < přiřazen
@enduml
```