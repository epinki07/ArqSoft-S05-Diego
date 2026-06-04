using ArqSoft_S05_Diego.Interfaces;
using ArqSoft_S05_Diego.Models;
using System.Text.Json;

namespace ArqSoft_S05_Diego.Repositories
{
    public class JsonCitaRepository : ICitaRepository
    {
        private readonly string _filePath;

        public JsonCitaRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "Citas.json");
        }

        public IEnumerable<Cita> ObtenerTodos()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Enumerable.Empty<Cita>();
                }

                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Cita>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Cita>();
            }
            catch
            {
                return Enumerable.Empty<Cita>();
            }
        }

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId)
        {
            return ObtenerTodos().Where(c => c.PacienteId == pacienteId);
        }
    }
}
