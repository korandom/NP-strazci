
workspace "Planovac� kalend��"  {

    model {
        strazce = person "Str�ce" 
        vedouciObvodu = person "Vedouc� str�n�ho obvoudu"
        vedouciStraze = person "Vedouc� odd�len� str�e a ochrany p��rody"
        kalendar = softwareSystem "Aplikace pro str�ce NP"  {
            Webapp = container "Webov� aplikace" "Poskytne front-end webov� aplikace klientovi"
            WebFrontend = container "Single-page Front-end" "" "React + TSX" "Web Front-End" {
                BrowserRouter = component "Browser Router" "��d� navigaci mezi str�nkami"
                LoginPage = component "P�ihla�ovac� str�nka" "" "" "Str�nka"
                PlansPage = component "Str�nka Pl�nov�n�" "Umo��uje pl�nov�n� slu�by" "" "Str�nka"
                SourcePage = component "Str�nka spr�vy zdroj�" "pouze pro vedouc� str�n�ho obvodu" "" "Str�nka"
                DailyPlan = component "Str�nka denn�ho pl�nu" "Zobrazuje denn� pl�n slu�by obvodu" "" "Str�nka"
                WeeklyPlan = component "Str�nka v�cedenn�ho pl�nu" "Zobrazuje pl�n slu�by v tabulce na 14 dn�" "" "Str�nka"
                GeneratingPreview = component "Str�nka generov�n� pl�nu tras"  "" "" "Str�nka"
                Services = component "Services" 
                AuthProvider = component "Provider autorizace a autentikace" "Poskytuje glob�ln� kontext p�ihla�ov�n� a u�ivatele" "" "Provider"
                DistrictDataProvider = component "Provider dat obvodu" "Poskytuje glob�ln� kontext dat obvodu - informace o zdroj�ch" "" "Provider"
                PlanDataProvider = component "Provider dat pl�nov�n�" "Poskytuje glob�ln� kontext dat pl�nov�n�" "" "Provider"
                
                // browser pages
                BrowserRouter -> LoginPage
                BrowserRouter -> PlansPage
                BrowserRouter -> SourcePage
                PlansPage -> DailyPlan
                PlansPage -> WeeklyPlan
                WeeklyPlan -> GeneratingPreview
                
                
                // services
                GeneratingPreview -> Services "vol� funkci pro z�sk�n� generovan�ho pl�nu"
                DistrictDataProvider -> Services "vol� funkce pro z�sk�n� nebo zm�nu dat obvodu"
                PlanDataProvider -> Services "vol� funkce pro z�sk�n� nebo zm�nu pl�n�"
                AuthProvider -> Services "vol� funkce pro autentikaci a autorizaci u�ivatel�"
                
            }
            Database = container "Datab�ze" "" "MariaDB" "Datab�ze"
            AuthDatabase = container "Auth Datab�ze" "" "MariaDB" "Datab�ze"
            Backend = container "Backend aplikace" "" "ASP .NET Core"{
                // Repa
                UnitOfWork = component "Unit Of Work" "souhr repozit���, umo��uje jednotn� ulo�en� zm�n"
                GeneralRepo = component "General Repository" "�ablona - pro Str�ce, Trasy, Dopr. prost�edky, Obvody"
                PlanRepo = component "Repository pl�n�" 
                AttRepo = component "Repository doch�zky"
                
                
                // Controllery
                // ObvodController = component "Controller Obvod�"
                // DochazkaController = component "Controller Doch�zky"
                // StrazceController = component "Controller Str�c�"
                // TrasaController = component "Controller Tras"
                UzivatelController = component "Controller U�ivatel�"
                PlanovaniController = component "Controller Pl�nov�n�"
                // ProstredkyController = component "Controller Dopr. Prost�edk�"
                // LockController = component "Controller Z�mk�"
                OstatniController = component "Placeholder - ostatn� controllery" "Obvod�, Doch�zky, Str�c�, Tras, Dopravn�ch prost�edk�, Z�mk�"
                
                // Huby
                ObvodHub = component "Hub Obvod" 
                PlanHub = component "Hub Pl�ny"
                
                //Auth
                AuthService = component "Autorizace a Autentikace"
                
                Generator = component "Modul generov�n� pl�nu tras"
                
                // Controller + Model vztahy
                //ObvodController -> UnitOfWork 
                // StrazceController -> UnitOfWork              
                // TrasaController -> UnitOfWork 
                 PlanovaniController -> UnitOfWork "p�istupuje k repozit���m a ukl�d� zm�ny"
                //ProstredkyController -> UnitOfWork 
                //LockController -> UnitOfWork
                //DochazkaController -> UnitOfWork
                 OstatniController -> UnitOfWork "p�istupuj� k repozit���m a ukl�daj� zm�ny"
                
                UzivatelController -> AuthService "vol�"
                PlanovaniController -> Generator "generuje pl�n tras"
                UnitOfWork -> PlanRepo "spr�va dat pl�n�"
                UnitOfWork -> GeneralRepo "spr�va dat model� zdroj�"
                UnitOfWork -> AttRepo "spr�va dat doch�zky"
                
                
                
                
                // vztahy s datab�z�
                GeneralRepo -> Database "zapisuje, �te" "SQL"
                PlanRepo -> Database "zapisuje, �te" "SQL"
                AttRepo -> Database "zapisuje, �te" "SQL"
                AuthService -> AuthDatabase "zapisuje, �te" "SQL"
                
                // vztahy s Frontendem
                //Services -> ObvodController "vol� API a p�ij�m� data" "JSON/HTTPS"
                // Services -> StrazceController "vol� API a p�ij�m� data" "JSON/HTTPS"
                // Services -> TrasaController "vol� API a p�ij�m� data" "JSON/HTTPS"
                Services -> UzivatelController "vol� API a p�ij�m� data" "JSON/HTTPS"
                // Services -> ProstredkyController "vol� API a p�ij�m� data" "JSON/HTTPS"
                Services -> PlanovaniController "vol� API a p�ij�m� data" "JSON/HTTPS"
                //Services -> LockController "vol� API a p�ij�m� data" "JSON/HTTPS"
                //Services -> DochazkaController "vol� API a p�ij�m� data" "JSON/HTTPS"
                Services ->  OstatniController "vol� API a p�ij�m� data" "JSON/HTTPS"
                PlanDataProvider -> PlanHub "pos�l� a p�ij�m� zm�ny v pl�nech slu�by" "SignalR"
                DistrictDataProvider -> ObvodHub "pos�l� a p�ij�m� �pravy zdroj� obvodu" "SignalR"
                 
            }
            
            
            Webapp -> WebFrontend 
            strazce -> BrowserRouter "Zobrazuje pl�n slu�by a pl�nuje trasy"
            vedouciStraze -> BrowserRouter "Zobrazuje pl�n slu�by"
            vedouciObvodu -> BrowserRouter "Zobrazuje a vytv��� pl�n slu�by, spravuje objekty"
            strazce -> WebApp "Na�ten� frontendu aplikace" "HTTPS" "internal"
            vedouciObvodu -> WebApp "Na�ten� frontendu aplikace" "HTTPS" "internal"
            vedouciStraze -> WebApp "Na�ten� frontendu aplikace" "HTTPS" "internal"

            
        }
    }
    

    configuration {
        scope softwaresystem
    }
        views {
            systemContext kalendar "kalendarSystemContext" "Diagram syst�mu" {
                include *
                //autoLayout lr
            }
            
            filtered "kalendarSystemContext" exclude "internal"
            
            container kalendar "kalendarContainer" "Diagram kontejner�" {
                include *
                //autoLayout lr
            }
            component WebFrontend "WebFrontend"{
                include *
            }
            component Backend "Backend" {
                include *
                //autoLayout lr
            }
            theme default

        styles {
            element "External" {
                background #999999
                color #ffffff
            }

            element "Web Front-End" {
                shape WebBrowser
            }

            element "Datab�ze" {
                shape Cylinder
            }

            element "Software System" {
                background #1168bd
                color #ffffff
            }

            element "U�ivatel" {
                shape person
                background #08427b
                color #ffffff
            }
            
            element "Provider" {
                background #a8dba8
            }
            
            element "Str�nka" {
                background #c6a2f3
            }
            

        }
    }

}