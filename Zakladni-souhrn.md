### Pl�novac� kalend�� pro str�ce NP �umava
Mobiln� aplikace na z�znam pracovn� doby a napl�novan�ch tras pro str�ce. 
Jako str�ce si mohu napl�novat trasy (trasy jsou reprezentov�ny ��seln�mi k�dy, tedy mno�ina ��seln�ch k�d�)  na ur�it� den a zaznamenat pracovn� dobu. Jako str�ce m�m mo�nost na konci t�dne zhodnotit t�den ve form� pozn�mky. 
Jako nad��zen� str�ce mohu str�c�m upravit napl�novan� trasy a zamknout zm�ny tak, aby s�m str�ce ji� do pl�nu nezasahoval, ale j� s�m mohu z�mek odemknout a pl�n zm�nit pokud je t�eba. Jako nad��zen� mohu tak� generovat statistiky pro jednotliv� trasy za ur�it� obdob�, neboli filtrovat podle tras. Mohu tak� kontrolovat odpracovan� doby pro jednotliv� str�ce v r�mci m�s�ce. V�echny data by m�li b�t ulo�en� zp�sobem, aby se dali exportovat ve form�tu excelov� tabulky. 

___
[Design pro mobiln� aplikaci](Figma-mobile.pdf)

---
#### Dom�nov� model
[Dom�nov� model](prilohy/domenovy_model.png)
```plantuml
@startuml
class "Nad��zen� str�ce"
class Str�ce {
  jm�no
  p��jmen�
}
class "Pl�n Tras"
class Pozn�mka {
  text
}
class Trasa{
  jm�no
  ��slo
  bool kontroln�
}
class Kontrola{
  �as
  m�sto
}
class "P�i�azen� auta" 
class Auto {
  id
  model
}
class Den  

"Nad��zen� str�ce" <|-U-  Str�ce

"Nad��zen� str�ce" "1" -- "0..*"  "P�i�azen� auta" : > za��dit
"Nad��zen� str�ce" "1" -- "0..*"  "Pl�n Tras" : > zm�nit/zamknout

Den "0..*" -- "1"  Pozn�mka  : < na
Den "1" -- "0..*"  "Pl�n Tras" : < na

"Pl�n Tras" "0..*" -- "0..*" Trasa : > 
"P�i�azen� auta" "0..*" -- "1"  Den : > na
"P�i�azen� auta" "0..*" -- "1"  Auto : >
Trasa "1" -- "0..1" Kontrola : > na

Str�ce "1" -- "0..*"  Pozn�mka : > editovat
Str�ce "1" -- "0..*"  "Pl�n Tras" : > zm�nit
Str�ce "1" -- "0..*"  "Pl�n Tras" : < p�i�azen
@enduml
```