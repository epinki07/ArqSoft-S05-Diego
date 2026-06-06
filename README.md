# Sistema de Gestion de Citas Medicas

Este proyecto es una aplicacion web desarrollada con ASP.NET Core MVC para administrar citas medicas. Permite registrar pacientes, medicos y citas, manteniendo la informacion organizada y disponible desde una interfaz web sencilla.

La aplicacion trabaja con archivos JSON como fuente de datos, por lo que no necesita una base de datos externa para funcionar. Esto hace que el proyecto sea facil de ejecutar, revisar y probar en un entorno local.

## Como funciona

El sistema esta dividido en tres modulos principales:

- Pacientes: permite registrar, consultar, editar y eliminar informacion de pacientes.
- Medicos: permite administrar los datos de los medicos, incluyendo especialidad y numero de licencia.
- Citas: permite crear citas relacionando un paciente con un medico, fecha, hora, motivo y estado.

La navegacion principal se encuentra en la parte superior de la pagina. Desde ahi se puede acceder al inicio, al listado de citas, pacientes y medicos.

En la pantalla de inicio se muestra un panel resumen con la cantidad total de pacientes, medicos, citas registradas y citas pendientes. Esto ayuda a tener una vista general del estado del sistema sin entrar modulo por modulo.

## Estructura del proyecto

El proyecto sigue el patron MVC:

- `Models`: contiene las clases principales del sistema, como `Paciente`, `Medico` y `Cita`.
- `Views`: contiene las pantallas que ve el usuario.
- `Controllers`: recibe las acciones del usuario y decide que vista mostrar o que operacion ejecutar.
- `Interfaces`: define los contratos que deben cumplir los repositorios.
- `Repositories`: contiene la logica para leer y guardar informacion en archivos JSON.
- `Data`: guarda los archivos JSON con la informacion del sistema.
- `wwwroot`: contiene archivos estaticos como CSS y JavaScript.

Esta separacion ayuda a que el codigo sea mas ordenado, facil de mantener y mas claro al momento de hacer cambios.

## Uso de interfaces

El proyecto utiliza interfaces para separar la logica de los controladores de la forma en que se guardan los datos. Por ejemplo, los controladores no dependen directamente de un archivo JSON, sino de interfaces como:

- `IPacienteRepository`
- `IMedicoRepository`
- `ICitaRepository`

Gracias a esto, si en el futuro se quiere cambiar el almacenamiento de JSON a una base de datos, se puede hacer creando una nueva implementacion del repositorio sin modificar toda la aplicacion.

## Mejoras realizadas en la interfaz

La interfaz fue mejorada para que el sistema sea mas claro y comodo de usar. Entre los cambios principales se incluyen:

- Menu de navegacion mas limpio.
- Pantalla inicial tipo panel con metricas generales.
- Tablas mas ordenadas y responsivas.
- Formularios con mejor distribucion visual.
- Pantallas de detalle mas claras para pacientes y medicos.
- Mensajes cuando no existen registros.
- Estilos personalizados en `site.css`.

Estas mejoras no cambian la logica principal del proyecto, solo hacen que la aplicacion sea mas facil de navegar y presentar.

## Beneficios del proyecto

El sistema permite centralizar informacion basica de una clinica o consultorio. Sus principales beneficios son:

- Organiza pacientes, medicos y citas en un solo lugar.
- Reduce el manejo manual de informacion.
- Facilita consultar rapidamente la agenda de citas.
- Permite identificar citas pendientes, confirmadas o canceladas.
- Mantiene una estructura sencilla para seguir agregando funciones.
- No requiere instalar una base de datos para probarlo.

## Como ejecutar el proyecto

Desde la carpeta del proyecto se puede ejecutar:

```bash
dotnet run
```

Tambien se puede ejecutar indicando un puerto especifico:

```bash
dotnet run --urls http://localhost:5060
```

Despues de iniciar la aplicacion, se abre el navegador en la direccion indicada por la consola.

## Tecnologias utilizadas

- ASP.NET Core MVC
- C#
- Razor Views
- Bootstrap
- JSON para almacenamiento local
- CSS personalizado
