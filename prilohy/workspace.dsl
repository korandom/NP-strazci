workspace "Planovací kalendář"  {

    model {
        strazce = person "Strážce" 
        vedouciObvodu = person "Vedoucí strážního obvoudu"
        vedouciSluzby = person "Vedoucí strážní služby"
        kalendar = softwareSystem "Plánovací kalendář"  {
            Webapp = container "Webová aplikace" "Poskytne front-end webové aplikace klientovi"
            WebFrontend = container "Webová aplikace Front-end" "" "" "Web Front-End" 
            MobileApp = container "Mobilní aplikace" 
            Backend = container "Backend aplikace" 
            Database = container "Databáze" "" "" "Database"
            
            Webapp -> WebFrontend 
            WebFrontend -> Backend "volá API"
            Backend -> WebFrontend "posílá data"
            Backend -> MobileApp "posílá data"
            MobileApp -> Backend "volá API"
            Backend -> Database "zapisuje, čte"
            
            strazce -> MobileApp "Zaznamenává docházku, zobrazuje a plánuje trasy"
            vedouciObvodu -> WebFrontend "Vytváří plán tras, kontroluje docházku a spravuje objekty"
            vedouciSluzby -> WebFrontend "Zobrazuje si informace o umístění strážců"
            vedouciObvodu -> WebApp "Načtení frontendu aplikace" "" "internal"
            vedouciSluzby -> WebApp "Načtení frontendu aplikace" "" "internal"
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
