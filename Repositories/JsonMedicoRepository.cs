using ArqSoft_S05_Diego.Interfaces;
using ArqSoft_S05_Diego.Models;
using System.Text.Json;

namespace ArqSoft_S05_Diego.Repositories
{
    public class JsonMedicoRepository : IMedicoRepository
    {
        private readonly string _filePath;

        public JsonMedicoRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "citas.json");
        }

        public IEnumerable<Medico> ObtenerTodos()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return Enumerable.Empty<Medico>();
                }

                var json = File.ReadAllText(_filePath);
                using var doc = JsonDocument.Parse(json);
                var medicos = new List<Medico>();
                var medicosArray = doc.RootElement.GetProperty("medicos");

                foreach (var medicoElement in medicosArray.EnumerateArray())
                {
                    medicos.Add(new Medico
                    {
                        Id = medicoElement.GetProperty("id").GetInt32(),
                        Nombre = medicoElement.GetProperty("nombre").GetString() ?? string.Empty,
                        Apellido = medicoElement.GetProperty("apellido").GetString() ?? string.Empty,
                        Especialidad = medicoElement.GetProperty("especialidad").GetString() ?? string.Empty,
                        NumeroLicencia = medicoElement.GetProperty("numeroLicencia").GetString() ?? string.Empty
                    });
                }

                return medicos;
            }
            catch
            {
                return Enumerable.Empty<Medico>();
            }
        }

        public Medico? ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(m => m.Id == id);
        }
    }
}
