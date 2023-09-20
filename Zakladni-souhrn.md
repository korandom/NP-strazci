### Plánovací kalendáø pro strážce NP Šumava
Mobilní aplikace na záznam pracovní doby a naplánovaných tras pro strážce. 
Jako strážce si mohu naplánovat trasy (trasy jsou reprezentovány èíselnými kódy, tedy množina èíselných kódù)  na urèitý den a zaznamenat pracovní dobu. Jako strážce mám možnost na konci týdne zhodnotit týden ve formì poznámky. 
Jako nadøízený strážce mohu strážcùm upravit naplánované trasy a zamknout zmìny tak, aby sám strážce již do plánu nezasahoval, ale já sám mohu zámek odemknout a plán zmìnit pokud je tøeba. Jako nadøízený mohu také generovat statistiky pro jednotlivé trasy za urèité období, neboli filtrovat podle tras. Mohu také kontrolovat odpracované doby pro jednotlivé strážce v rámci mìsíce. Všechny data by mìli být uložené zpùsobem, aby se dali exportovat ve formátu excelové tabulky. 

___
[Design pro mobilní aplikaci](Figma-mobile.pdf)

---
#### Doménový model
[Doménový model](prilohy/domenovy_model.png)
```plantuml
@startuml
class "Nadøízený strážce"
class Strážce {
  jméno
  pøíjmení
}
class "Plán Tras"
class Poznámka {
  text
}
class Trasa{
  jméno
  èíslo
  bool kontrolní
}
class Kontrola{
  èas
  místo
}
class "Pøiøazení auta" 
class Auto {
  id
  model
}
class Den  

"Nadøízený strážce" <|-U-  Strážce

"Nadøízený strážce" "1" -- "0..*"  "Pøiøazení auta" : > zaøídit
"Nadøízený strážce" "1" -- "0..*"  "Plán Tras" : > zmìnit/zamknout

Den "0..*" -- "1"  Poznámka  : < na
Den "1" -- "0..*"  "Plán Tras" : < na

"Plán Tras" "0..*" -- "0..*" Trasa : > 
"Pøiøazení auta" "0..*" -- "1"  Den : > na
"Pøiøazení auta" "0..*" -- "1"  Auto : >
Trasa "1" -- "0..1" Kontrola : > na

Strážce "1" -- "0..*"  Poznámka : > editovat
Strážce "1" -- "0..*"  "Plán Tras" : > zmìnit
Strážce "1" -- "0..*"  "Plán Tras" : < pøiøazen
@enduml
```