using CitasApp.Interfaces;
using CitasApp.Models;
using System.Text.Json;

namespace CitasApp.Repositories
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
                    return Enumerable.Empty<Paciente>();

                var json = File.ReadAllText(_filePath);
                var pacientes = JsonSerializer.Deserialize<List<Paciente>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Paciente>();

                return pacientes;
            }
            catch
            {
                return Enumerable.Empty<Paciente>();
            }
        }

        public Paciente ObtenerPorId(int id)
        {
            var pacientes = ObtenerTodos();
            return pacientes.FirstOrDefault(p => p.Id == id) ?? new Paciente();
        }

        public void Agregar(Paciente paciente)
        {
            try
            {
                var pacientes = ObtenerTodos().ToList();
                paciente.Id = pacientes.Count > 0 ? pacientes.Max(p => p.Id) + 1 : 1;
                pacientes.Add(paciente);

                var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                var json = JsonSerializer.Serialize(pacientes, options);
                File.WriteAllText(_filePath, json);
            }
            catch
            {
                // Manejo de error silencioso
            }
        }

        public void Editar(Paciente paciente)
        {
            try
            {
                var pacientes = ObtenerTodos().ToList();
                var pacienteExistente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);
                if (pacienteExistente != null)
                {
                    pacienteExistente.Nombre = paciente.Nombre;
                    pacienteExistente.Apellido = paciente.Apellido;
                    pacienteExistente.Email = paciente.Email;
                    pacienteExistente.Telefono = paciente.Telefono;

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(pacientes, options);
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
                var pacientes = ObtenerTodos().ToList();
                var paciente = pacientes.FirstOrDefault(p => p.Id == id);
                if (paciente != null)
                {
                    pacientes.Remove(paciente);

                    var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
                    var json = JsonSerializer.Serialize(pacientes, options);
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
