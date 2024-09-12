
workspace "Planovací kalendář"  {

    model {
        strazce = person "Strážce" 
        vedouciObvodu = person "Vedoucí strážního obvoudu"
        kalendar = softwareSystem "Plánovací kalendář"  {
            Webapp = container "Webová aplikace" "Poskytne front-end webové aplikace klientovi"
            WebFrontend = container "Single-page Front-end" "" "React + TSX" "Web Front-End" {
                BrowserRouter = component "Browser Router" "Řídí navigaci mezi stránkami"
                LoginPage = component "Přihlašovací stránka"
                HomePage = component "Domovská stránka" "Zobrazuje denní plán služby"
                PlansPage = component "Stránka Plánování" "Umožňuje plánování služby"
                SourcePage = component "Stránka správy zdrojů" "pouze pro vedoucí strážního obvodu"
                Menu = component "Menu" 
                Services = component "Services"
                AuthProvider = component "Poskytovatel autorizace a autentikace" "Poskytuje kontext všem "
                DistrictDataProvider = component "Poskytovatel dat Obvodu"
                PlanDataProvider = component "Poskytovatel dat plánů"
                
                // browser pages
                BrowserRouter -> LoginPage
                BrowserRouter -> Menu
                Menu -> HomePage
                Menu -> PlansPage
                Menu -> SourcePage
                
                // services
                DistrictDataProvider -> Services "volá funkce pro získání nebo změnu dat obvodu"
                PlanDataProvider -> Services "volá funkce pro získání nebo změnu plánů"
                AuthProvider -> Services "volá funkce pro autentikaci a autorizaci uživatelů"
                
            }
            Database = container "Databáze" "" "MariaDB" "Database"
            AuthDatabase = container "Auth Databáze" "" "MariaDB" "Databáze"
            Backend = container "Backend aplikace" "" "ASP .NET Core"{
                // Repa
                UnitOfWork = component "Unit Of Work" "souhr repozitářů, umožňuje jednotné uložení více transakcí"
                GeneralRepo = component "General Repository" "šablona - pro Strážce, Trasy, Dopr. prostředky"
                PlanRepo = component "Repository plánů"
                
                
                // Controllery
                ObvodController = component "Controller Obvodů"
                StrazceController = component "Controller Strážců"
                TrasaController = component "Controller Tras"
                UzivatelController = component "Controller Uživatelů"
                PlanovaniController = component "Controller Plánování"
                ProstredkyController = component "Controller Dopr. Prostředků"
                
                // Huby
                ObvodHub = component "Hub Obvod-zdroje"
                PlanHub = component "Hub Plány"
                
                //Auth
                AuthService = component "Autorizace a Autentikace"
                
                
                // Controller + Model vztahy
                ObvodController -> UnitOfWork 
                StrazceController -> UnitOfWork              
                TrasaController -> UnitOfWork 
                PlanovaniController -> UnitOfWork 
                ProstredkyController -> UnitOfWork 
                UzivatelController -> AuthService 
                
                UnitOfWork -> PlanRepo "správa dat plánů služby"
                UnitOfWork -> GeneralRepo "správa dat modelů zdrojů"
                
                
                
                
                // vztahy s databází
                GeneralRepo -> Database "zapisuje, čte"
                PlanRepo -> Database "zapisuje, čte"
                AuthService -> AuthDatabase "zapisuje, čte"
                
                // vztahy s Frontendem
                Services -> ObvodController "volá API a přijímá data"
                Services -> StrazceController "volá API a přijímá data"
                Services -> TrasaController "volá API a přijímá data"
                Services -> UzivatelController "volá API a přijímá data"
                Services -> ProstredkyController "volá API a přijímá data"
                Services -> PlanovaniController "volá API a přijímá data"
                PlanDataProvider -> PlanHub "posílá a přijímá změny v plánech"
                DistrictDataProvider -> ObvodHub "posílá a přijímá úpravy zdrojů obvodu"
                 
            }
            
            
            Webapp -> WebFrontend 
            strazce -> WebFrontend "Zobrazuje plán služby a plánuje trasy"
            strazce -> WebApp "Načtení frontendu aplikace" "" "internal"
            vedouciObvodu -> WebFrontend "Vytváří plán služby a spravuje objekty"
            vedouciObvodu -> WebApp "Načtení frontendu aplikace" "" "internal"
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
            
            container kalendar "kalendarContainer" "Diagram kontejnerů" {
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

            element "Database" {
                shape Cylinder
            }

            element "Software System" {
                background #1168bd
                color #ffffff
            }

            element "Person" {
                shape person
                background #08427b
                color #ffffff
            }

        }
    }

}
