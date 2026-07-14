# ADR 0001: Autenticacion por roles, usuarios semilla y agenda

## Estado

Aceptado

## Contexto

El proyecto necesitaba conectar la aplicacion con PostgreSQL y controlar la visibilidad de citas segun el tipo de usuario:

- El administrador debe ver todo.
- Los medicos deben poder ver la agenda completa.
- Los pacientes solo deben ver sus propias citas.
- Los pacientes no deben saber que otros pacientes tienen cita.

Tambien se pidio crear usuarios concretos para el proyecto escolar:

- `diego`
- `carlos`
- `joshua`
- `michelle`
- `jorge`

## Decisiones

### 1. Usar PostgreSQL como fuente activa

Se mantuvo `DataSource` en `postgres` y se uso la conexion:

```text
Host=localhost;Port=5432;Database=catalogoapp_db;Username=catalogo_user;Password=catalogo123
```

Decision: PostgreSQL queda como fuente real de datos porque permite persistir usuarios, citas y agenda en tablas relacionales.

### 2. Crear tabla `Usuarios`

Se agrego una tabla `Usuarios` con:

```text
NombreUsuario
NombreCompleto
PasswordHash
Rol
PacienteId
MedicoId
```

Decision: separar el concepto de usuario de los modelos `Paciente` y `Medico`.

Motivo: una persona que inicia sesion no siempre es un paciente o medico. Por ejemplo, el administrador `jorge` no necesita `PacienteId` ni `MedicoId`.

### 3. Usar roles

Se definieron tres roles:

```text
Administrador
Medico
Paciente
```

Decision: usar roles simplifica las reglas de acceso sin mezclar permisos dentro de las vistas.

### 4. Usar cookies de autenticacion

Se configuro autenticacion con cookies de ASP.NET Core.

Decision: cookies es suficiente para una app MVC escolar y evita meter JWT o infraestructura extra innecesaria.

### 5. Hashear contrasenas

Las contrasenas no se guardan en texto plano. Se usa PBKDF2 con salt.

Decision: aunque el proyecto sea escolar, se mantiene una practica minima correcta de seguridad.

Usuarios semilla:

```text
jorge    / Jorge123!    / Administrador / Jorge Javier Pedrozo Romero
joshua   / Joshua123!   / Medico
michelle / Michelle123! / Medico
diego    / Diego123!    / Paciente
carlos   / Carlos123!   / Paciente
```

### 6. Crear tabla `Agenda`

Se agrego una tabla `Agenda` sincronizada desde `Citas`.

Decision: aunque `Citas` ya contiene la informacion base, `Agenda` funciona como una vista persistida orientada a consulta.

Motivo: permite tener una tabla explicita para el apartado de agenda, sin cambiar el significado de `Citas`.

### 7. Sincronizar Agenda desde el repositorio PostgreSQL de citas

Cuando se crea, edita o elimina una cita desde `PostgresCitaRepository`, tambien se actualiza `Agenda`.

Decision: la sincronizacion queda en la capa de persistencia PostgreSQL porque es una preocupacion de almacenamiento, no de la vista.

### 8. Filtrar acceso en backend

La agenda se filtra en `AgendaController`:

```text
Administrador -> ve toda la agenda.
Medico        -> ve toda la agenda.
Paciente      -> solo ve las filas donde PacienteId coincide con su claim.
```

Decision: no se confia solamente en esconder columnas HTML. El filtro se aplica antes de enviar el modelo a la vista.

## Verificacion

Se verifico que la tabla `Agenda` contenga:

```text
Diego Ramirez   -> Joshua Medico
Carlos Paciente -> Michelle Medico
```

Se probaron logins y acceso a `/Agenda`:

```text
jorge    -> ve Diego/Joshua y Carlos/Michelle
joshua   -> ve Diego/Joshua y Carlos/Michelle
michelle -> ve Diego/Joshua y Carlos/Michelle
diego    -> ve solo Diego/Joshua
carlos   -> ve solo Carlos/Michelle
```

Resultado:

```text
Build OK
PostgreSQL OK
Login OK
Filtrado de Agenda OK
```

## Consideraciones SOLID

- Responsabilidad unica: el controlador de login delega autenticacion a `AuthService`.
- Inversion de dependencias: los controladores usan interfaces de repositorio cuando trabajan con datos de negocio.
- Separacion de reglas: la vista solo muestra lo que recibe; el filtrado sensible ocurre en controladores/repositorios.
- Abierto/cerrado: se agrego `Agenda` con su repositorio sin eliminar los repositorios JSON/CSV/SQLite existentes.

## Riesgos Y Mejores Practicas Pendientes

- La sincronizacion de `Agenda` y `Citas` esta implementada en codigo. En un sistema real convendria usar migraciones formales, constraints y posiblemente una vista SQL o eventos de dominio.
- Hay un warning de seguridad heredado por SQLite:

```text
SQLitePCLRaw.lib.e_sqlite3 2.1.11 tiene una vulnerabilidad alta conocida.
```

Si PostgreSQL sera la fuente definitiva, conviene quitar SQLite o actualizar sus paquetes.

## Uso De IA

La IA apoyo en:

- Crear comandos de PostgreSQL.
- Guiar la instalacion del paquete NuGet.
- Implementar autenticacion por roles.
- Crear y verificar usuarios semilla.
- Crear la tabla `Agenda`.
- Probar logins y acceso por rol con `curl`.
- Documentar decisiones tecnicas en este ADR.
