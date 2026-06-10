 CitasApp.Web/Program.cs
// ─────────────────────────────────────────────────────────────────────────────
// AQUÍ enchufas el Adapter que quieres usar para cada entidad.
// Domain y Application NO se tocan — solo cambia este archivo.
// ─────────────────────────────────────────────────────────────────────────────

using CitasApp.Application.Services;
using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Carpeta de datos ───────────────────────────────────────────────────────
var dataFolder = Path.Combine(builder.Environment.WebRootPath, "data");
Directory.CreateDirectory(dataFolder);

// Rutas para CSV
var csvPacientes = Path.Combine(dataFolder, "pacientes.csv");
var csvMedicos   = Path.Combine(dataFolder, "medicos.csv");
var csvCitas     = Path.Combine(dataFolder, "citas.csv");

// Ruta para SQLite (un solo archivo .db para las 3 tablas)
var sqlitePath   = Path.Combine(dataFolder, "citasapp.db");


// ── 2. Elige tus Adapters ─────────────────────────────────────────────────────
// Descomenta el bloque que quieras y comenta los otros dos.
// ¡Las interfaces (Ports) no cambian!

// ▶ Bloque A — JSON (como estaba antes)
/*
builder.Services.AddSingleton<IPacienteRepository, JsonPacienteRepository>();
builder.Services.AddSingleton<IMedicoRepository,   JsonMedicoRepository>();
builder.Services.AddSingleton<ICitaRepository,     JsonCitaRepository>();
*/

// ▶ Bloque B — CSV  ← activo ahora

builder.Services.AddSingleton<IPacienteRepository>(_ => new CsvPacienteRepository(csvPacientes));
builder.Services.AddSingleton<IMedicoRepository>  (_ => new CsvMedicoRepository(csvMedicos));
builder.Services.AddSingleton<ICitaRepository>    (_ => new CsvCitaRepository(csvCitas));


// ▶ Bloque C — SQLite
/*
builder.Services.AddSingleton<IPacienteRepository>(_ => new SqlitePacienteRepository(sqlitePath));
builder.Services.AddSingleton<IMedicoRepository>  (_ => new SqliteMedicoRepository(sqlitePath));
builder.Services.AddSingleton<ICitaRepository>    (_ => new SqliteCitaRepository(sqlitePath));
*/


// ── 3. Servicios de aplicación (no cambian con el Adapter) ───────────────────
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<CitaService>();

// ── 4. MVC ────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
