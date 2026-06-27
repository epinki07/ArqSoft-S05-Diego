using CitasApp.Interfaces;
using CitasApp.Observers;
using CitasApp.Repositories;
using CitasApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// La selección JSON/CSV existente se conserva para médicos y citas.
// El repositorio de pacientes se elige por entorno mediante RepositoryFactory.
var fuenteDatos = "json";
// var fuenteDatos = "csv";

switch (fuenteDatos)
{
    case "json":
        builder.Services.AddScoped<IMedicoRepository, JsonMedicoRepository>();
        builder.Services.AddScoped<ICitaRepository, JsonCitaRepository>();
        break;

    case "csv":
        builder.Services.AddScoped<IMedicoRepository, CsvMedicoRepository>();
        builder.Services.AddScoped<ICitaRepository, CsvCitaRepository>();
        break;

    default:
        throw new InvalidOperationException("fuenteDatos debe ser \"json\" o \"csv\".");
}

builder.Services.AddScoped<IPacienteRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var repo = RepositoryFactory.CrearPacienteRepository(
        builder.Environment.EnvironmentName,
        env);

    return new LoggingPacienteRepository(repo);
});

builder.Services.AddScoped<ICitaObserver, SmsObserver>();
builder.Services.AddScoped<ICitaObserver, EmailObserver>();
builder.Services.AddScoped<CitaService>();

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

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
