Jsme státní pøíspìvkovou organizací, které podléhá v tomto oboru 3 základním 
zákonùm/naøízením, a to:
1. Zákon è. 181/2014 Sb. - Zákon o kybernetické bezpeènosti a o zmìnì souvisejících zákonù 
(zákon o kybernetické bezpeènosti)
2. Zákon è. 365/2000 Sb. - Zákon o informaèních systémech veøejné správy a o zmìnì 
nìkterých dalších zákonù
3. Obecné naøízení o ochranì osobních údajù (GDPR)

Posouzením Vašeho projektu jsem vybral nìkolik nutných požadavkù pro vývoj a provoz SW.

a) Pøedimplementaèní analýza (PIA), alespoò základního rozsahu, která bude 
definovat/popisovat nejen zpracované procesy (tøeba Archimate3 nebo jen od ruky), ale i 
základní postupy vývoje a provozu SW (mobilní i webová aplikace, vèetnì DB). Nutná 
akceptace pøed zahájením programování.
b) Øízení dodavatelù ze ZoKB, kde je nutné definovat parametry a úroveò poskytovaných 
zdrojových kódù a úroveò jejich dokumentace, vèetnì dokumentace datových struktur 
(Technická dokumentace, vèetnì pøíruèek). V prùbìhu programování a pøi akceptaci pøedání 
do užívání.
c) Definice rozsahu sbìru a uchovávání osobních údajù (požadavky GDPR) v PIA.
d) Zabezpeèení mobilní aplikace - základní požadavky: šifrovaná a zabezpeèená komunikace 
s WS backendem, ostatní zabezpeèení na úrovni zabezpeèení telefonu (Výhradnì OS Android 
11+)
e) Zabezpeèení webové aplikace - Výhradnì on-premise øešení, šifrování, poddoména 
"npsumava.cz", autorizace a autentizace uživatelù pøes LDAP organizace, mfa povinnì. 
(Doporuèení PHP8/MariaDB)
f) Vylouèení veškerých externích cloudových služeb, vèetnì souborových.

Je upozoròuji, že od pøíštího roku budeme nasazovat pro organizaci centrální evidenci 
docházky. Nyní je podán projekt na IROP.