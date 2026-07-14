# ADR 0001: Autenticacion por roles, PostgreSQL y agenda

## Estado

Aceptado

## Contexto

La aplicacion necesitaba dejar de depender solo de archivos locales y usar PostgreSQL para guardar usuarios, pacientes, medicos, citas y agenda.

Tambien se necesitaba controlar que cada tipo de usuario viera solo lo que le corresponde:

- El administrador ve todo.
- Los medicos ven la agenda completa.
- Los pacientes solo ven sus propias citas.
- Los pacientes no deben ver nombres ni citas de otros pacientes.

Los usuarios pedidos para validar el flujo fueron:

```text
jorge    -> administrador
joshua   -> medico
michelle -> medico
diego    -> paciente
carlos   -> paciente
```

## Decision

Se implemento autenticacion con cookies de ASP.NET Core, roles simples y persistencia en PostgreSQL.

La aplicacion usa esta conexion:

```text
Host=localhost;Port=5432;Database=catalogoapp_db;Username=catalogo_user;Password=catalogo123
```

Se agregaron dos tablas principales:

```text
Usuarios
Agenda
```

La tabla `Usuarios` guarda las cuentas de acceso:

```text
NombreUsuario
NombreCompleto
PasswordHash
Rol
PacienteId
MedicoId
```

La tabla `Agenda` guarda una version consultable de las citas:

```text
CitaId
PacienteId
MedicoId
PacienteNombre
MedicoNombre
Fecha
Hora
Motivo
Estado
```

## Credenciales De Prueba

```text
jorge    / Jorge123!    / Administrador / Jorge Javier Pedrozo Romero
joshua   / Joshua123!   / Medico
michelle / Michelle123! / Medico
diego    / Diego123!    / Paciente
carlos   / Carlos123!   / Paciente
```

Las contrasenas no se guardan en texto plano. Se guardan con PBKDF2 y salt.

## Datos Semilla

Se crean estos datos al iniciar la aplicacion si no existen:

```text
Paciente: Diego Ramirez
Medico:   Joshua Medico
Cita:     Consulta de Diego con Joshua

Paciente: Carlos Paciente
Medico:   Michelle Medico
Cita:     Consulta de Carlos con Michelle
```

Tambien se limpio la semilla anterior de demostracion para evitar datos que confundieran la prueba:

```text
admin / medico / paciente
Paciente Demo
Medico Demo
Consulta general
```

## Reglas De Acceso

El filtrado se hace en backend, no solo en la vista.

En `AgendaController`:

```text
Administrador -> ve toda la agenda.
Medico        -> ve toda la agenda.
Paciente      -> solo ve las filas donde PacienteId coincide con su usuario.
```

En la vista de agenda:

- Administrador y medicos ven paciente, medico, fecha, hora, motivo y estado.
- Pacientes ven fecha, hora, medico y estado de sus propias citas.
- Pacientes no ven la columna de otros pacientes.

## Por Que Se Tomaron Estas Decisiones

Se separo `Usuario` de `Paciente` y `Medico` porque no todos los usuarios son pacientes o medicos. El administrador `jorge` necesita iniciar sesion, pero no debe tener `PacienteId` ni `MedicoId`.

Se usaron roles (`Administrador`, `Medico`, `Paciente`) porque son suficientes para las reglas actuales y mantienen el codigo simple.

Se uso autenticacion por cookies porque el proyecto es MVC y no necesita tokens JWT.

Se creo `Agenda` como tabla separada porque el requisito pide una tabla de agenda. Aunque `Citas` ya tiene la informacion base, `Agenda` funciona como una tabla preparada para consulta.

Se sincroniza `Agenda` desde `PostgresCitaRepository` cuando se crea, edita o elimina una cita. Asi la vista no tiene que construir la agenda manualmente.

## Como Correr El Proyecto En Rider

Abrir esta carpeta en Rider:

```text
/Users/diegoramirezmagana/RiderProjects/ArqSoft-S05-Diego
```

Abrir la solucion:

```text
CitasApp.sln
```

Desde Rider:

1. Seleccionar la configuracion `http` o el proyecto `CitasApp`.
2. Presionar el boton verde Run.
3. Revisar en la consola el puerto que diga `Now listening on`.

En las pruebas manuales se uso:

```text
http://localhost:5099/Auth/Login
```

Importante: abrir solo `http://localhost` no funciona si la app esta escuchando en otro puerto. En ese caso el navegador puede quedar en blanco. Se debe abrir la URL completa con puerto.

## Como Correr Por Terminal

Desde la carpeta del proyecto:

```bash
cd /Users/diegoramirezmagana/RiderProjects/ArqSoft-S05-Diego
dotnet run --no-build --urls http://localhost:5099
```

Luego abrir:

```text
http://localhost:5099/Auth/Login
```

## Pruebas Ejecutadas

### Compilacion

```bash
dotnet build CitasApp.csproj
```

Resultado:

```text
Build succeeded
```

Quedo un warning heredado de SQLite, pero no bloquea la ejecucion:

```text
SQLitePCLRaw.lib.e_sqlite3 2.1.11 tiene una vulnerabilidad alta conocida.
```

### Verificar Usuarios En PostgreSQL

```bash
PGPASSWORD=catalogo123 psql -h localhost -U catalogo_user -d catalogoapp_db -c 'SELECT "NombreUsuario", "NombreCompleto", "Rol", "PacienteId", "MedicoId" FROM "Usuarios" ORDER BY "NombreUsuario";'
```

Resultado esperado:

```text
carlos   | Carlos Paciente             | Paciente
diego    | Diego Ramirez               | Paciente
jorge    | Jorge Javier Pedrozo Romero | Administrador
joshua   | Joshua Medico               | Medico
michelle | Michelle Medico             | Medico
```

### Verificar Agenda En PostgreSQL

```bash
PGPASSWORD=catalogo123 psql -h localhost -U catalogo_user -d catalogoapp_db -c 'SELECT "PacienteNombre", "MedicoNombre", "Fecha", "Hora", "Motivo" FROM "Agenda" ORDER BY "Fecha", "Hora";'
```

Resultado esperado:

```text
Diego Ramirez   | Joshua Medico
Carlos Paciente | Michelle Medico
```

### Verificar Login Y Filtros

Se probaron los cinco usuarios contra `/Agenda`.

Resultado:

```text
jorge    -> ve Diego/Joshua y Carlos/Michelle
joshua   -> ve Diego/Joshua y Carlos/Michelle
michelle -> ve Diego/Joshua y Carlos/Michelle
diego    -> ve solo Diego/Joshua
carlos   -> ve solo Carlos/Michelle
```

Tambien se verifico que:

```text
http://localhost:5099/Auth/Login -> responde OK
http://localhost/                -> no responde si no hay servidor en el puerto 80
```

## Consideraciones SOLID

- `AuthController` no valida contrasenas directamente; delega esa responsabilidad en `AuthService`.
- Los controladores consumen repositorios e interfaces cuando trabajan con datos de negocio.
- El filtrado sensible se hace antes de enviar datos a la vista.
- `Agenda` se agrego sin eliminar los repositorios JSON, CSV o SQLite existentes.

## Riesgos Y Mejoras Pendientes

- `Agenda` duplica datos de `Citas`. Para este proyecto escolar es aceptable porque el requisito pide una tabla de agenda. En una aplicacion real podria ser una vista SQL o una consulta con joins.
- La sincronizacion de `Agenda` se hace en codigo. En produccion convendria usar migraciones formales y restricciones de base de datos.
- Actualmente los medicos ven toda la agenda porque asi se pidio. Si despues cada medico debe ver solo sus citas, el filtro debe cambiar en `AgendaController`.
- Sigue existiendo el warning de SQLite. Si PostgreSQL sera la fuente definitiva, conviene quitar SQLite o actualizar sus paquetes.

## Uso De IA

La IA se uso para guiar comandos, revisar errores, proponer la estructura de autenticacion, ejecutar pruebas locales, validar la salida de PostgreSQL y ordenar este ADR.
