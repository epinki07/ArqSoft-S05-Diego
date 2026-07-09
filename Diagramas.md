# Diagramas de CitasApp

Este archivo concentra los diagramas de arquitectura de CitasApp como codigo Mermaid. La rama de entrega para estos diagramas es `UML`.

## C4 Nivel 1 - Contexto

**Para quien es:** usuarios, revisores academicos y personas que necesitan entender el sistema sin entrar al codigo.

**Pregunta que responde:** quien usa CitasApp, que problema resuelve y con que elementos externos se relaciona.

```mermaid
flowchart LR
    usuario["Usuario de la clinica<br/>Administra pacientes, medicos y citas"]
    navegador["Navegador web<br/>Interfaz para usar la app"]
    clienteApi["Cliente REST opcional<br/>Navegador, curl o herramienta API"]
    citasApp["CitasApp<br/>Aplicacion web para gestionar citas medicas"]
    archivos["Archivos locales<br/>JSON / CSV"]
    consola["Consola del servidor<br/>Logs y notificaciones simuladas"]

    usuario -->|"Consulta, registra, edita y elimina datos"| navegador
    navegador -->|"Peticiones HTTP MVC"| citasApp
    clienteApi -->|"Peticiones REST /api"| citasApp
    citasApp -->|"Lee y guarda pacientes, medicos y citas"| archivos
    citasApp -->|"Muestra vistas HTML"| navegador
    citasApp -->|"Imprime confirmaciones, SMS y email simulados"| consola
```

CitasApp es una aplicacion web ASP.NET Core MVC para administrar pacientes, medicos y citas medicas. En el alcance academico no depende de una base de datos externa: puede trabajar con archivos locales JSON o CSV y tiene repositorios SQLite disponibles como alternativa de persistencia.

## C4 Nivel 2 - Contenedores

**Para quien es:** desarrolladores o evaluadores que necesitan ubicar las piezas tecnicas grandes.

**Pregunta que responde:** cuales son los contenedores principales del sistema y como se comunican.

```mermaid
flowchart TB
    usuario["Usuario de la clinica"]
    clienteApi["Cliente REST opcional"]

    subgraph servidor["Servidor local de CitasApp"]
        webApp["Aplicacion ASP.NET Core MVC<br/>Controllers, Razor Views, Services y DI"]
        staticFiles["Archivos estaticos<br/>wwwroot: CSS y JS"]
        jsonData["Data/*.json<br/>Pacientes, medicos y citas"]
        csvData["Data/*.csv<br/>Almacenamiento alternativo"]
        sqlite["SQLite<br/>Repositorios disponibles para extender persistencia"]
        console["Consola<br/>Logs, SMS y email simulados"]
    end

    usuario -->|"Usa UI web"| webApp
    clienteApi -->|"Consume endpoints /api"| webApp
    webApp -->|"Renderiza HTML con Razor"| usuario
    webApp -->|"Sirve estilos y scripts"| staticFiles
    webApp -->|"Repositorio JSON por configuracion actual"| jsonData
    webApp -. "Fuente alternativa" .-> csvData
    webApp -. "Adaptador disponible" .-> sqlite
    webApp -->|"Escribe eventos de confirmacion y observadores"| console
```

El contenedor principal es una sola aplicacion ASP.NET Core MVC. Dentro de ella conviven la interfaz Razor, los controladores MVC, los endpoints REST y la capa de servicios. La persistencia queda aislada por repositorios intercambiables.

## C4 Nivel 3 - Componentes

**Para quien es:** desarrolladores que daran mantenimiento al codigo o necesitan explicar la arquitectura interna.

**Pregunta que responde:** que componentes internos procesan las acciones del usuario, la API, la persistencia y la confirmacion de citas.

```mermaid
flowchart TB
    browser["Navegador web"]
    restClient["Cliente REST"]

    subgraph app["CitasApp ASP.NET Core"]
        program["Program.cs<br/>Configura DI, rutas MVC y fuente de datos"]

        subgraph presentation["Presentacion"]
            mvcControllers["Controladores MVC<br/>Home, Paciente, Medico y Cita"]
            apiControllers["Controladores API<br/>PacientesApi, MedicosApi y CitasApi"]
            views["Razor Views<br/>Pantallas para inicio, pacientes, medicos y citas"]
        end

        subgraph application["Aplicacion"]
            citaService["CitaService<br/>Consulta citas y confirma citas"]
            observers["ICitaObserver<br/>SmsObserver y EmailObserver"]
        end

        subgraph domain["Dominio"]
            models["Modelos<br/>Paciente, Medico y Cita"]
            repoContracts["Interfaces de repositorio<br/>IPacienteRepository, IMedicoRepository, ICitaRepository"]
        end

        subgraph infrastructure["Infraestructura"]
            factory["RepositoryFactory<br/>Factory para repositorio de pacientes"]
            loggingDecorator["LoggingPacienteRepository<br/>Decorator de operaciones de pacientes"]
            jsonRepos["Repositorios JSON<br/>JsonPaciente, JsonMedico y JsonCita"]
            csvRepos["Repositorios CSV<br/>CsvPaciente, CsvMedico y CsvCita"]
            sqliteRepos["Repositorios SQLite<br/>SqlitePaciente, SqliteMedico y SqliteCita"]
            localFiles["Data/<br/>JSON y CSV locales"]
            sqliteDb["SQLite local<br/>Persistencia extensible"]
            consoleLog["Consola<br/>Salida de logs y notificaciones simuladas"]
        end
    end

    browser -->|"HTTP MVC"| mvcControllers
    mvcControllers -->|"Devuelven HTML"| views
    views -->|"Renderiza pantallas"| browser
    restClient -->|"HTTP JSON /api"| apiControllers

    program -->|"Registra dependencias"| mvcControllers
    program -->|"Registra dependencias"| apiControllers
    mvcControllers -->|"CRUD directo"| repoContracts
    apiControllers -->|"Casos de uso API"| citaService
    citaService -->|"Consulta y actualiza"| repoContracts
    citaService -->|"Notifica cita confirmada"| observers
    observers -->|"SMS y email simulados"| consoleLog

    repoContracts --> factory
    factory -->|"Selecciona por entorno"| loggingDecorator
    loggingDecorator -->|"Decora acceso a pacientes"| jsonRepos
    repoContracts --> jsonRepos
    repoContracts -. "Fuente alternativa" .-> csvRepos
    repoContracts -. "Adaptador disponible" .-> sqliteRepos
    jsonRepos --> localFiles
    csvRepos --> localFiles
    sqliteRepos --> sqliteDb
    models --> repoContracts
```

Este nivel muestra la separacion principal de responsabilidades: la presentacion atiende pantallas y endpoints, `CitaService` concentra la confirmacion de citas y las consultas usadas por API, los modelos representan los datos centrales, y los repositorios aislan la persistencia local. Los patrones implementados aparecen asi: **Factory** en `RepositoryFactory`, **Decorator** en `LoggingPacienteRepository` y **Observer** en `SmsObserver` y `EmailObserver`.

## Diagrama de clases UML

**Para quien es:** revisores que quieren ver la relacion entre las clases de dominio principales.

**Pregunta que responde:** que datos componen pacientes, medicos y citas, y como se relacionan.

```mermaid
classDiagram
    class Paciente {
        +int Id
        +string Nombre
        +string Apellido
        +string Email
        +string Telefono
    }

    class Medico {
        +int Id
        +string Nombre
        +string Apellido
        +string Especialidad
        +string NumeroLicencia
    }

    class Cita {
        +int Id
        +int PacienteId
        +int MedicoId
        +DateOnly Fecha
        +TimeOnly Hora
        +string Motivo
        +string Estado
    }

    Paciente "1" --> "0..*" Cita : agenda
    Medico "1" --> "0..*" Cita : atiende
```

## Declaracion de uso de IA

Se utilizo inteligencia artificial como apoyo para organizar los diagramas, redactar notas de interpretacion y verificar que el contenido correspondiera con la estructura real del proyecto. La seleccion final, revision, commit y publicacion corresponden al autor del repositorio.
