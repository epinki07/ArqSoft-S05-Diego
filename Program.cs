using CitasApp.Interfaces;
using CitasApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Activa SOLO una de estas dos lineas.
// JSON = datos antiguos. CSV = datos nuevos descargados.
var fuenteDatos = "json";
// var fuenteDatos = "csv";

switch (fuenteDatos)
{
    case "json":
        builder.Services.AddScoped<IMedicoRepository, JsonMedicoRepository>();
        builder.Services.AddScoped<IPacienteRepository, JsonPacienteRepository>();
        builder.Services.AddScoped<ICitaRepository, JsonCitaRepository>();
        break;

    case "csv":
        builder.Services.AddScoped<IMedicoRepository, CsvMedicoRepository>();
        builder.Services.AddScoped<IPacienteRepository, CsvPacienteRepository>();
        builder.Services.AddScoped<ICitaRepository, CsvCitaRepository>();
        break;

    default:
        throw new InvalidOperationException("fuenteDatos debe ser \"json\" o \"csv\".");
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/api/pacientes", (IPacienteRepository pacientes) =>
    Results.Ok(pacientes.ObtenerTodos()));

app.MapGet("/api/pacientes/{id:int}", (int id, IPacienteRepository pacientes) =>
{
    var paciente = pacientes.ObtenerPorId(id);
    return paciente.Id == 0 ? Results.NotFound() : Results.Ok(paciente);
});

app.MapGet("/api/medicos", (IMedicoRepository medicos) =>
    Results.Ok(medicos.ObtenerTodos()));

app.MapGet("/api/medicos/{id:int}", (int id, IMedicoRepository medicos) =>
{
    var medico = medicos.ObtenerPorId(id);
    return medico.Id == 0 ? Results.NotFound() : Results.Ok(medico);
});

app.MapGet("/api/citas", (
    ICitaRepository citas,
    IPacienteRepository pacientes,
    IMedicoRepository medicos) =>
    Results.Ok(new
    {
        citas = citas.ObtenerTodos(),
        pacientes = pacientes.ObtenerTodos(),
        medicos = medicos.ObtenerTodos()
    }));

app.MapGet("/api/citas/porpaciente/{pacienteId:int}", (
    int pacienteId,
    ICitaRepository citas,
    IPacienteRepository pacientes,
    IMedicoRepository medicos) =>
{
    var citasDelPaciente = citas.ObtenerPorPaciente(pacienteId).ToList();
    return citasDelPaciente.Count == 0
        ? Results.NotFound()
        : Results.Ok(new
        {
            citas = citasDelPaciente,
            pacientes = pacientes.ObtenerTodos(),
            medicos = medicos.ObtenerTodos()
        });
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
