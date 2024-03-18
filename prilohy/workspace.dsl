workspace "Planovací kalendář"  {

    model {
        strazce = person "Strážce" 
        vedouciObvodu = person "Vedoucí strážního obvoudu"
        kalendar = softwareSystem "Plánovací kalendář"  {
            Webapp = container "Webová aplikace" "Poskytne front-end webové aplikace klientovi"
            WebFrontend = container "Single-page Webová aplikace Front-end" "" "" "Web Front-End" 
            MobileApp = container "Mobilní aplikace" 
            Database = container "Databáze" "" "" "Database"
            Backend = container "Backend aplikace" {
                StrazceModel = component "Model Strážců" 
                TrasaModel = component "Model Tras a okrsků"
                DopravaModel = component "Model Dopravních prostředků"
                PlanovaniModel = component "Model Plánů"
                SpravaController = component "Controller Správy zdrojů"
                PlanovaniController = component "Controller Plánování tras"
                
                // Controller + Model vztahy
                SpravaController -> StrazceModel "reprezentace dat strážců"
                SpravaController -> TrasaModel "reprezentace dat tras a okrsků"
                SpravaController -> DopravaModel "reprezentace dat dopravních prostředků"
                PlanovaniController -> PlanovaniModel "reprezentace dat plánů tras"
                
                // vztahy s databází
                StrazceModel -> Database "zapisuje, čte"
                TrasaModel -> Database "zapisuje, čte"
                DopravaModel -> Database "zapisuje, čte"
                PlanovaniModel -> Database "zapisuje, čte"
                
                // vztahy s Frontendem
                 WebFrontend -> SpravaController "volá API"
                 SpravaController -> WebFrontend "posílá data"
                 WebFrontend -> PlanovaniController "volá API"
                 PlanovaniController -> WebFrontend "posílá data"
            }
            
            
            Webapp -> WebFrontend 
            Backend -> MobileApp "posílá data"
            MobileApp -> Backend "volá API"
            
            strazce -> MobileApp "Zobrazuje a plánuje trasy"
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
