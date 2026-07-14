# ADR 0001: Creacion de la rama doe-smell

## Estado

Aceptado

## Contexto

Se necesitaba crear una rama llamada `doe-smell` para conservar una version del proyecto antes de aplicar cambios posteriores relacionados con refactorizacion, separacion de responsabilidades e inyeccion de dependencias.

El objetivo de esta rama es tener un punto de comparacion para analizar el estado previo del codigo y poder contrastarlo con versiones donde ya se aplican mejoras de arquitectura o refactorizacion.

## Decision

Se creo la rama `doe-smell` a partir del commit:

```text
5420f96 CREATE: Infrastructure - JsonItemRepository
```

Este commit fue elegido porque es el punto anterior al commit donde se integran cambios de presentacion y configuracion de servicios en `Program.cs`.

La rama quedo ubicada en:

```text
doe-smell -> 5420f96
```

## Como Se Hizo

Primero se reviso el historial de Git para identificar el commit adecuado:

```bash
git log --oneline --decorate --all --graph
```

Despues se detecto que existian cambios locales que impedian cambiar de rama. Para no perderlos, se guardaron temporalmente con `stash`:

```bash
git stash push -u -m "guardar cambios antes de doe-smell"
```

Luego se creo la rama en el commit seleccionado:

```bash
git switch -c doe-smell 5420f96
```

Como la rama ya habia sido creada localmente, se verifico que apuntara al commit correcto y despues se cambio a ella:

```bash
git rev-parse doe-smell
git log --oneline -1 doe-smell
git switch doe-smell
```

Finalmente se confirmo el estado:

```bash
git branch --show-current
git log --oneline -3
git status --short --branch
```

## Resultado

La rama `doe-smell` quedo creada localmente en el commit correcto.

Esto permite:

- Comparar el codigo antes y despues de aplicar refactorizaciones.
- Tener una base para documentar code smells o decisiones de arquitectura.
- Evitar modificar directamente la rama principal.
- Mantener un punto estable para analisis academico o tecnico.

## Verificacion De Rama Remota

Se verifico que la rama remota `origin/doe-smell` existe y apunta al commit del ADR:

```text
10f46a0 docs: agregar ADR de rama doe-smell
```

Tambien se verifico que la rama local `doe-smell` esta alineada con `origin/doe-smell`.

Quedo un cambio local independiente en:

```text
Catalogo.Infrastructure/Catalogo.Infrastructure.csproj
```

Ese cambio no forma parte del ADR.

## Verificacion De PostgreSQL

En el proyecto escolar se creo y probo una base de datos PostgreSQL para la aplicacion de citas.

Datos de conexion usados:

```text
Host=localhost
Port=5432
Database=catalogoapp_db
Username=catalogo_user
Password=catalogo123
```

Cadena de conexion:

```text
Host=localhost;Port=5432;Database=catalogoapp_db;Username=catalogo_user;Password=catalogo123
```

La conexion se verifico con `psql` usando el usuario `catalogo_user`. Tambien se verifico que existan las tablas:

```text
Citas
Medicos
Pacientes
```

## Verificacion De Logins Y Roles

Se reviso si existian logins separados para:

- Usuario o paciente.
- Medico.
- Administrador.

Resultado de la revision:

```text
No hay implementacion de login por roles en el codigo revisado.
```

En el proyecto de citas se encontro:

- `UseAuthorization()` en `Program.cs`.
- Autenticacion anonima habilitada en `launchSettings.json`.
- No se encontro `UseAuthentication()`.
- No se encontro `AddAuthentication()`.
- No se encontraron controladores de login.
- No se encontraron claims o roles para `Paciente`, `Medico` o `Administrador`.

Por lo tanto, no se puede afirmar que los logins de usuario, medico y administrador funcionen correctamente. Para que esa validacion sea posible, primero se debe implementar autenticacion y autorizacion por roles.

## Uso De IA

La IA se uso como apoyo para guiar el proceso, no para tomar control del repositorio remoto.

Sirvio principalmente para:

- Revisar el historial de commits.
- Identificar el commit adecuado para crear la rama.
- Explicar por que Git bloqueaba el cambio de rama cuando habia cambios locales.
- Dar los comandos en orden.
- Aclarar la diferencia entre crear una rama local y hacer `push` a GitHub.
- Verificar que la rama quedara apuntando al commit correcto.

No se hizo `push` como parte de esta decision. La IA solo dio guia y ejecuto/verifico comandos locales cuando fue necesario.

## Consecuencias

La rama `doe-smell` funciona como una fotografia del proyecto en un momento anterior a cambios posteriores de arquitectura.

Si se desea subir esta rama a GitHub, debe hacerse manualmente y con cuidado para no afectar repositorios ajenos:

```bash
git push --set-upstream origin doe-smell
```

Ese paso no forma parte de este ADR.
