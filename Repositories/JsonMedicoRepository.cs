using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Repositories
{
    public class JsonMedicoRepository : IMedicoRepository
    {
        private readonly string _filePath;

        public JsonMedicoRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "Medicos.json");
        }

        public IEnumerable<Medico> ObtenerTodos()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return Enumerable.Empty<Medico>();

                var json = File.ReadAllText(_filePath);
                var medicos = JsonSerializer.Deserialize<List<Medico>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Medico>();

                return medicos;
            }
            catch
            {
                return Enumerable.Empty<Medico>();
            }
        }

        public Medico ObtenerPorId(int id)
        {
            var medicos = ObtenerTodos();
            return medicos.FirstOrDefault(m => m.Id == id) ?? new Medico();
        }

        public void Agregar(Medico medico)
        {
            try
            {
                var medicos = ObtenerTodos().ToList();
                medico.Id = medicos.Count > 0 ? medicos.Max(m => m.Id) + 1 : 1;
                medicos.Add(medico);

                var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                var json = JsonSerializer.Serialize(medicos, options);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Manejo de error silencioso
            }
        }

        public void Editar(Medico medico)
        {
            try
            {
                var medicos = ObtenerTodos().ToList();
                var medicoExistente = medicos.FirstOrDefault(m => m.Id == medico.Id);
                if (medicoExistente != null)
                {
                    medicoExistente.Nombre = medico.Nombre;
                    medicoExistente.Apellido = medico.Apellido;
                    medicoExistente.Especialidad = medico.Especialidad;
                    medicoExistente.NumeroLicencia = medico.NumeroLicencia;

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(medicos, options);
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
                var medicos = ObtenerTodos().ToList();
                var medico = medicos.FirstOrDefault(m => m.Id == id);
                if (medico != null)
                {
                    medicos.Remove(medico);

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(medicos, options);
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
