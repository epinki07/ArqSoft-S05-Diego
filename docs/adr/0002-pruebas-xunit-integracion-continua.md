# ADR 0002: Pruebas xUnit e integracion continua

## Estado

Aceptado

## Contexto

La semana 12 pide agregar pruebas unitarias con xUnit y configurar un pipeline de integracion continua que compile y corra las pruebas automaticamente en GitHub Actions.

El objetivo es verificar que los cambios no rompan comportamiento importante de CitasApp antes de integrarlos mediante un Pull Request.

## Decision

Se agrego un proyecto de pruebas:

```text
tests/CitasApp.Tests
```

El proyecto usa xUnit y referencia al proyecto principal:

```text
CitasApp.csproj
```

Tambien se agrego el workflow:

```text
.github/workflows/ci.yml
```

El pipeline corre en `push` y en `pull_request`, y ejecuta:

```bash
dotnet restore CitasApp.sln
dotnet build CitasApp.sln --no-restore
dotnet test CitasApp.sln --no-build
```

## Clases Probadas

Se eligieron estas clases porque cubren comportamiento central y facil de validar automaticamente:

- `CitaService`: concentra la logica de confirmacion de citas y notificacion a observadores.
- `PasswordHashService`: protege la validacion de contrasenas con hash PBKDF2.
- `RepositoryFactory`: decide que repositorio de pacientes se usa segun el entorno.

## Pruebas Agregadas

### CitaService

Se prueba que una cita existente y pendiente:

- cambie su estado a `Confirmada`;
- sea enviada al repositorio para editarse;
- notifique a los observers configurados.

Tambien se prueba que una cita inexistente:

- regrese `null`;
- no intente editar datos;
- no notifique observers.

### PasswordHashService

Se prueba que:

- una contrasena correcta sea aceptada por `Verify`;
- una contrasena incorrecta sea rechazada por `Verify`.

### RepositoryFactory

Se prueba que:

- en `Production` se use `MemoriaPacienteRepository`;
- en `Development` se use `JsonPacienteRepository`.

## Consecuencias

Cada push o Pull Request puede mostrar un check verde si la solucion compila y las pruebas pasan.

Si una prueba falla, GitHub Actions debe marcar el pipeline en rojo y mostrar el error en el log del job.

La carpeta `tests` se excluyo de la compilacion del proyecto principal para que `CitasApp.csproj` no intente compilar los archivos de pruebas como parte de la aplicacion web.

## Verificacion

Comando usado:

```bash
dotnet test CitasApp.sln
```

Resultado esperado:

```text
Passed: 6
Failed: 0
Total: 6
```

Permanece un warning heredado de una dependencia de SQLite, pero no bloquea la compilacion ni las pruebas:

```text
SQLitePCLRaw.lib.e_sqlite3 2.1.11 tiene una vulnerabilidad alta conocida.
```
