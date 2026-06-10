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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
