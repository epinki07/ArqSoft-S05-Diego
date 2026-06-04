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
            _filePath = Path.Combine(env.ContentRootPath, "Data", "citas.json");
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
                using var doc = JsonDocument.Parse(json);
                var citas = new List<Cita>();
                var citasArray = doc.RootElement.GetProperty("citas");

                foreach (var citaElement in citasArray.EnumerateArray())
                {
                    citas.Add(new Cita
                    {
                        Id = citaElement.GetProperty("id").GetInt32(),
                        PacienteId = citaElement.GetProperty("pacienteId").GetInt32(),
                        MedicoId = citaElement.GetProperty("medicoId").GetInt32(),
                        Fecha = DateOnly.Parse(citaElement.GetProperty("fecha").GetString() ?? string.Empty),
                        Hora = TimeOnly.Parse(citaElement.GetProperty("hora").GetString() ?? string.Empty),
                        Motivo = citaElement.GetProperty("motivo").GetString() ?? string.Empty,
                        Estado = citaElement.GetProperty("estado").GetString() ?? "Pendiente"
                    });
                }

                return citas;
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
