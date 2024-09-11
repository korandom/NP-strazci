workspace "Planovací kalendář"  {

    model {
        strazce = person "Strážce" 
        vedouciObvodu = person "Vedoucí strážního obvoudu"
        kalendar = softwareSystem "Plánovací kalendář"  {
            Webapp = container "Webová aplikace" "Poskytne front-end webové aplikace klientovi"
            WebFrontend = container "Single-page Webová aplikace Front-end" "" "React + TSX" "Web Front-End" 
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
                WebFrontend -> ObvodController "volá API a přijímá data"
                WebFrontend -> StrazceController "volá API a přijímá data"
                WebFrontend -> TrasaController "volá API a přijímá data"
                WebFrontend -> UzivatelController "volá API a přijímá data"
                WebFrontend -> ProstredkyController "volá API a přijímá data"
                WebFrontend -> PlanovaniController "volá API"
                PlanovaniController -> WebFrontend "posílá data"
                WebFrontend -> PlanHub "posílá a přijímá změny v plánech"
                WebFrontend -> ObvodHub "posílá a přijímá úpravy zdrojů obvodu"
                 
            }
            
            
            Webapp -> WebFrontend 
            strazce -> WebFrontend "Zobrazuje a plánuje trasy"
            strazce -> WebApp "Načtení frontendu aplikace" "" "internal"
            vedouciObvodu -> WebFrontend "Vytváří plán tras a spravuje objekty"
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