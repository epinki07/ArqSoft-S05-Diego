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
            _filePath = Path.Combine(env.ContentRootPath, "Data", "Pacientes.json");
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
                return JsonSerializer.Deserialize<List<Paciente>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Paciente>();
            }
            catch
            {
                return Enumerable.Empty<Paciente>();
            }
        }

        public Paciente ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(p => p.Id == id) ?? new Paciente();
        }
    }
}
