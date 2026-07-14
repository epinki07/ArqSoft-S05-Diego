using CitasApp.Data;
using CitasApp.Interfaces;
using CitasApp.Observers;
using CitasApp.Repositories;
using CitasApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// La fuente puede ser "postgres", "json" o "csv".
var fuenteDatos = builder.Configuration["DataSource"] ?? "postgres";

builder.Services.AddDbContext<CitasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

switch (fuenteDatos)
{
    case "postgres":
        builder.Services.AddScoped<IMedicoRepository, PostgresMedicoRepository>();
        builder.Services.AddScoped<ICitaRepository, PostgresCitaRepository>();
        builder.Services.AddScoped<IPacienteRepository, PostgresPacienteRepository>();
        builder.Services.AddScoped<IAgendaRepository, PostgresAgendaRepository>();
        break;

    case "json":
        builder.Services.AddScoped<IMedicoRepository, JsonMedicoRepository>();
        builder.Services.AddScoped<ICitaRepository, JsonCitaRepository>();
        builder.Services.AddScoped<IPacienteRepository>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var repo = RepositoryFactory.CrearPacienteRepository(
                builder.Environment.EnvironmentName,
                env);

            return new LoggingPacienteRepository(repo);
        });
        break;

    case "csv":
        builder.Services.AddScoped<IMedicoRepository, CsvMedicoRepository>();
        builder.Services.AddScoped<ICitaRepository, CsvCitaRepository>();
        builder.Services.AddScoped<IPacienteRepository>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var repo = RepositoryFactory.CrearPacienteRepository(
                builder.Environment.EnvironmentName,
                env);

            return new LoggingPacienteRepository(repo);
        });
        break;

    default:
        throw new InvalidOperationException("DataSource debe ser \"postgres\", \"json\" o \"csv\".");
}

builder.Services.AddScoped<ICitaObserver, SmsObserver>();
builder.Services.AddScoped<ICitaObserver, EmailObserver>();
builder.Services.AddScoped<CitaService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

if (fuenteDatos == "postgres")
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CitasDbContext>();
    DatabaseSeeder.EnsureSeedData(dbContext);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
