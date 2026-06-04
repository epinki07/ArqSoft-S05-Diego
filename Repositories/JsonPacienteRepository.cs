using ArqSoft_S05_Diego.Interfaces;
using ArqSoft_S05_Diego.Models;
using System.Text.Json;

namespace ArqSoft_S05_Diego.Repositories
{
    public class JsonPacienteRepository : IPacienteRepository
    {
        private readonly string _filePath;

        public JsonPacienteRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "citas.json");
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Enumerable.Empty<Paciente>();
                }

                var json = File.ReadAllText(_filePath);
                using var doc = JsonDocument.Parse(json);
                var pacientes = new List<Paciente>();
                var pacientesArray = doc.RootElement.GetProperty("pacientes");

                foreach (var pacienteElement in pacientesArray.EnumerateArray())
                {
                    pacientes.Add(new Paciente
                    {
                        Id = pacienteElement.GetProperty("id").GetInt32(),
                        Nombre = pacienteElement.GetProperty("nombre").GetString() ?? string.Empty,
                        Apellido = pacienteElement.GetProperty("apellido").GetString() ?? string.Empty,
                        Email = pacienteElement.GetProperty("email").GetString() ?? string.Empty,
                        Telefono = pacienteElement.GetProperty("telefono").GetString() ?? string.Empty
                    });
                }

                return pacientes;
            }
            catch
            {
                return Enumerable.Empty<Paciente>();
            }
        }

        public Paciente? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(p => p.Id == id);
        }
    }
}
