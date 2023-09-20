Jsme st�tn� p��sp�vkovou organizac�, kter� podl�h� v tomto oboru 3 z�kladn�m 
z�kon�m/na��zen�m, a to:
1. Z�kon �. 181/2014 Sb. - Z�kon o kybernetick� bezpe�nosti a o zm�n� souvisej�c�ch z�kon� 
(z�kon o kybernetick� bezpe�nosti)
2. Z�kon �. 365/2000 Sb. - Z�kon o informa�n�ch syst�mech ve�ejn� spr�vy a o zm�n� 
n�kter�ch dal��ch z�kon�
3. Obecn� na��zen� o ochran� osobn�ch �daj� (GDPR)

Posouzen�m Va�eho projektu jsem vybral n�kolik nutn�ch po�adavk� pro v�voj a provoz SW.

a) P�edimplementa�n� anal�za (PIA), alespo� z�kladn�ho rozsahu, kter� bude 
definovat/popisovat nejen zpracovan� procesy (t�eba Archimate3 nebo jen od ruky), ale i 
z�kladn� postupy v�voje a provozu SW (mobiln� i webov� aplikace, v�etn� DB). Nutn� 
akceptace p�ed zah�jen�m programov�n�.
b) ��zen� dodavatel� ze ZoKB, kde je nutn� definovat parametry a �rove� poskytovan�ch 
zdrojov�ch k�d� a �rove� jejich dokumentace, v�etn� dokumentace datov�ch struktur 
(Technick� dokumentace, v�etn� p��ru�ek). V pr�b�hu programov�n� a p�i akceptaci p�ed�n� 
do u��v�n�.
c) Definice rozsahu sb�ru a uchov�v�n� osobn�ch �daj� (po�adavky GDPR) v PIA.
d) Zabezpe�en� mobiln� aplikace - z�kladn� po�adavky: �ifrovan� a zabezpe�en� komunikace 
s WS backendem, ostatn� zabezpe�en� na �rovni zabezpe�en� telefonu (V�hradn� OS Android 
11+)
e) Zabezpe�en� webov� aplikace - V�hradn� on-premise �e�en�, �ifrov�n�, poddom�na 
"npsumava.cz", autorizace a autentizace u�ivatel� p�es LDAP organizace, mfa povinn�. 
(Doporu�en� PHP8/MariaDB)
f) Vylou�en� ve�ker�ch extern�ch cloudov�ch slu�eb, v�etn� souborov�ch.

Je upozor�uji, �e od p��t�ho roku budeme nasazovat pro organizaci centr�ln� evidenci 
doch�zky. Nyn� je pod�n projekt na IROP.