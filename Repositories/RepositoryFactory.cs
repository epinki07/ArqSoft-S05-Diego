using CitasApp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CitasApp.Repositories
{
    public static class RepositoryFactory
    {
        public static IPacienteRepository CrearPacienteRepository(
            string entorno,
            IWebHostEnvironment env)
        {
            ArgumentNullException.ThrowIfNull(env);

            return string.Equals(entorno, Environments.Production, StringComparison.OrdinalIgnoreCase)
                ? new MemoriaPacienteRepository()
                : new JsonPacienteRepository(env);
        }
    }
}
