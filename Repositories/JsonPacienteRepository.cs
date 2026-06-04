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

        public void Agregar(Paciente paciente)
        {
            try
            {
                var pacientes = ObtenerTodos().ToList();
                paciente.Id = pacientes.Count > 0 ? pacientes.Max(p => p.Id) + 1 : 1;
                pacientes.Add(paciente);
                Guardar(pacientes);
            }
            catch
            {
            }
        }

        public void Editar(Paciente paciente)
        {
            try
            {
                var pacientes = ObtenerTodos().ToList();
                var pacienteExistente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);
                if (pacienteExistente == null)
                {
                    return;
                }

                pacienteExistente.Nombre = paciente.Nombre;
                pacienteExistente.Apellido = paciente.Apellido;
                pacienteExistente.Email = paciente.Email;
                pacienteExistente.Telefono = paciente.Telefono;
                Guardar(pacientes);
            }
            catch
            {
            }
        }

        public void Eliminar(int id)
        {
            try
            {
                var pacientes = ObtenerTodos().ToList();
                var paciente = pacientes.FirstOrDefault(p => p.Id == id);
                if (paciente == null)
                {
                    return;
                }

                pacientes.Remove(paciente);
                Guardar(pacientes);
            }
            catch
            {
            }
        }

        private void Guardar(List<Paciente> pacientes)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(pacientes, options);
            File.WriteAllText(_filePath, json);
        }
    }
}
