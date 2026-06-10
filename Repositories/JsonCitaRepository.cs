using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Repositories
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
                    return Enumerable.Empty<Cita>();

                var json = File.ReadAllText(_filePath);
                var citas = JsonSerializer.Deserialize<List<Cita>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Cita>();

                return citas;
            }
            catch
            {
                return Enumerable.Empty<Cita>();
            }
        }

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId)
        {
            var citas = ObtenerTodos();
            return citas.Where(c => c.PacienteId == pacienteId);
        }

        public void Agregar(Cita cita)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                cita.Id = citas.Count > 0 ? citas.Max(c => c.Id) + 1 : 1;
                citas.Add(cita);

                var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                var json = JsonSerializer.Serialize(citas, options);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Manejo de error silencioso
            }
        }

        public Cita ObtenerPorId(int id)
        {
            var citas = ObtenerTodos();
            return citas.FirstOrDefault(c => c.Id == id) ?? new Cita();
        }

        public void Editar(Cita cita)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                var citaExistente = citas.FirstOrDefault(c => c.Id == cita.Id);
                if (citaExistente != null)
                {
                    citaExistente.PacienteId = cita.PacienteId;
                    citaExistente.MedicoId = cita.MedicoId;
                    citaExistente.Fecha = cita.Fecha;
                    citaExistente.Hora = cita.Hora;
                    citaExistente.Motivo = cita.Motivo;
                    citaExistente.Estado = cita.Estado;

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(citas, options);
                    File.WriteAllText(_filePath, json);
                }
            }
            catch
            {
                // Manejo de error silencioso
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                var citas = ObtenerTodos().ToList();
                var cita = citas.FirstOrDefault(c => c.Id == id);
                if (cita != null)
                {
                    citas.Remove(cita);

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(citas, options);
                    File.WriteAllText(_filePath, json);
                }
            }
            catch
            {
                // Manejo de error silencioso
            }
        }
    }
}
