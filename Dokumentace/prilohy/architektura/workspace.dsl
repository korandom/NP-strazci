
workspace "Planovací kalendáø"  {

    model {
        strazce = person "Strážce" 
        vedouciObvodu = person "Vedoucí strážního obvoudu"
        vedouciStraze = person "Vedoucí oddìlení stráže a ochrany pøírody"
        kalendar = softwareSystem "Aplikace pro strážce NP"  {
            Webapp = container "Webová aplikace" "Poskytne front-end webové aplikace klientovi"
            WebFrontend = container "Single-page Front-end" "" "React + TSX" "Web Front-End" {
                BrowserRouter = component "Browser Router" "Øídí navigaci mezi stránkami"
                LoginPage = component "Pøihlašovací stránka" "" "" "Stránka"
                PlansPage = component "Stránka Plánování" "Umožòuje plánování služby" "" "Stránka"
                SourcePage = component "Stránka správy zdrojù" "pouze pro vedoucí strážního obvodu" "" "Stránka"
                DailyPlan = component "Stránka denního plánu" "Zobrazuje denní plán služby obvodu" "" "Stránka"
                WeeklyPlan = component "Stránka vícedenního plánu" "Zobrazuje plán služby v tabulce na 14 dní" "" "Stránka"
                GeneratingPreview = component "Stránka generování plánu tras"  "" "" "Stránka"
                Services = component "Services" 
                AuthProvider = component "Provider autorizace a autentikace" "Poskytuje globálnì kontext pøihlašování a uživatele" "" "Provider"
                DistrictDataProvider = component "Provider dat obvodu" "Poskytuje globálnì kontext dat obvodu - informace o zdrojích" "" "Provider"
                PlanDataProvider = component "Provider dat plánování" "Poskytuje globálnì kontext dat plánování" "" "Provider"
                
                // browser pages
                BrowserRouter -> LoginPage
                BrowserRouter -> PlansPage
                BrowserRouter -> SourcePage
                PlansPage -> DailyPlan
                PlansPage -> WeeklyPlan
                WeeklyPlan -> GeneratingPreview
                
                
                // services
                GeneratingPreview -> Services "volá funkci pro získání generovaného plánu"
                DistrictDataProvider -> Services "volá funkce pro získání nebo zmìnu dat obvodu"
                PlanDataProvider -> Services "volá funkce pro získání nebo zmìnu plánù"
                AuthProvider -> Services "volá funkce pro autentikaci a autorizaci uživatelù"
                
            }
            Database = container "Databáze" "" "MariaDB" "Databáze"
            AuthDatabase = container "Auth Databáze" "" "MariaDB" "Databáze"
            Backend = container "Backend aplikace" "" "ASP .NET Core"{
                // Repa
                UnitOfWork = component "Unit Of Work" "souhr repozitáøù, umožòuje jednotné uložení zmìn"
                GeneralRepo = component "General Repository" "šablona - pro Strážce, Trasy, Dopr. prostøedky, Obvody"
                PlanRepo = component "Repository plánù" 
                AttRepo = component "Repository docházky"
                
                
                // Controllery
                // ObvodController = component "Controller Obvodù"
                // DochazkaController = component "Controller Docházky"
                // StrazceController = component "Controller Strážcù"
                // TrasaController = component "Controller Tras"
                UzivatelController = component "Controller Uživatelù"
                PlanovaniController = component "Controller Plánování"
                // ProstredkyController = component "Controller Dopr. Prostøedkù"
                // LockController = component "Controller Zámkù"
                OstatniController = component "Placeholder - ostatní controllery" "Obvodù, Docházky, Strážcù, Tras, Dopravních prostøedkù, Zámkù"
                
                // Huby
                ObvodHub = component "Hub Obvod" 
                PlanHub = component "Hub Plány"
                
                //Auth
                AuthService = component "Autorizace a Autentikace"
                
                Generator = component "Modul generování plánu tras"
                
                // Controller + Model vztahy
                //ObvodController -> UnitOfWork 
                // StrazceController -> UnitOfWork              
                // TrasaController -> UnitOfWork 
                 PlanovaniController -> UnitOfWork "pøistupuje k repozitáøùm a ukládá zmìny"
                //ProstredkyController -> UnitOfWork 
                //LockController -> UnitOfWork
                //DochazkaController -> UnitOfWork
                 OstatniController -> UnitOfWork "pøistupují k repozitáøùm a ukládají zmìny"
                
                UzivatelController -> AuthService "volá"
                PlanovaniController -> Generator "generuje plán tras"
                UnitOfWork -> PlanRepo "správa dat plánù"
                UnitOfWork -> GeneralRepo "správa dat modelù zdrojù"
                UnitOfWork -> AttRepo "správa dat docházky"
                
                
                
                
                // vztahy s databází
                GeneralRepo -> Database "zapisuje, ète" "SQL"
                PlanRepo -> Database "zapisuje, ète" "SQL"
                AttRepo -> Database "zapisuje, ète" "SQL"
                AuthService -> AuthDatabase "zapisuje, ète" "SQL"
                
                // vztahy s Frontendem
                //Services -> ObvodController "volá API a pøijímá data" "JSON/HTTPS"
                // Services -> StrazceController "volá API a pøijímá data" "JSON/HTTPS"
                // Services -> TrasaController "volá API a pøijímá data" "JSON/HTTPS"
                Services -> UzivatelController "volá API a pøijímá data" "JSON/HTTPS"
                // Services -> ProstredkyController "volá API a pøijímá data" "JSON/HTTPS"
                Services -> PlanovaniController "volá API a pøijímá data" "JSON/HTTPS"
                //Services -> LockController "volá API a pøijímá data" "JSON/HTTPS"
                //Services -> DochazkaController "volá API a pøijímá data" "JSON/HTTPS"
                Services ->  OstatniController "volá API a pøijímá data" "JSON/HTTPS"
                PlanDataProvider -> PlanHub "posílá a pøijímá zmìny v plánech služby" "SignalR"
                DistrictDataProvider -> ObvodHub "posílá a pøijímá úpravy zdrojù obvodu" "SignalR"
                 
            }
            
            
            Webapp -> WebFrontend 
            strazce -> BrowserRouter "Zobrazuje plán služby a plánuje trasy"
            vedouciStraze -> BrowserRouter "Zobrazuje plán služby"
            vedouciObvodu -> BrowserRouter "Zobrazuje a vytváøí plán služby, spravuje objekty"
            strazce -> WebApp "Naètení frontendu aplikace" "HTTPS" "internal"
            vedouciObvodu -> WebApp "Naètení frontendu aplikace" "HTTPS" "internal"
            vedouciStraze -> WebApp "Naètení frontendu aplikace" "HTTPS" "internal"

            
        }
    }
    

    configuration {
        scope softwaresystem
    }
        views {
            systemContext kalendar "kalendarSystemContext" "Diagram systému" {
                include *
                //autoLayout lr
            }
            
            filtered "kalendarSystemContext" exclude "internal"
            
            container kalendar "kalendarContainer" "Diagram kontejnerù" {
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

            element "Databáze" {
                shape Cylinder
            }

            element "Software System" {
                background #1168bd
                color #ffffff
            }

            element "Uživatel" {
                shape person
                background #08427b
                color #ffffff
            }
            
            element "Provider" {
                background #a8dba8
            }
            
            element "Stránka" {
                background #c6a2f3
            }
            

        }
    }

}