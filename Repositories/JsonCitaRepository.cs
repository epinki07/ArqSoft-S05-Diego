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

        public Cita ObtenerPorId(int id)
        {
            return ObtenerTodos().FirstOrDefault(c => c.Id == id) ?? new Cita();
        }

        public void Agregar(Cita cita)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                cita.Id = citas.Count > 0 ? citas.Max(c => c.Id) + 1 : 1;
                citas.Add(cita);
                Guardar(citas);
            }
            catch
            {
            }
        }

        public void Editar(Cita cita)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                var citaExistente = citas.FirstOrDefault(c => c.Id == cita.Id);
                if (citaExistente == null)
                {
                    return;
                }

                citaExistente.PacienteId = cita.PacienteId;
                citaExistente.MedicoId = cita.MedicoId;
                citaExistente.Fecha = cita.Fecha;
                citaExistente.Hora = cita.Hora;
                citaExistente.Motivo = cita.Motivo;
                citaExistente.Estado = cita.Estado;
                Guardar(citas);
            }
            catch
            {
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                var cita = citas.FirstOrDefault(c => c.Id == id);
                if (cita == null)
                {
                    return;
                }

                citas.Remove(cita);
                Guardar(citas);
            }
            catch
            {
            }
        }

        private void Guardar(List<Cita> citas)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(citas, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
