using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Repositories
{
    public class CsvPacienteRepository : IPacienteRepository
    {
        private const string Header = "Id,Nombre,Apellido,Email,Telefono";
        private readonly string _filePath;

        public CsvPacienteRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "pacientes.csv");
            EnsureFileExists();
        }

        public CsvPacienteRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        public IEnumerable<Paciente> ObtenerTodos() => LeerTodos();

        public Paciente ObtenerPorId(int id) =>
            LeerTodos().FirstOrDefault(p => p.Id == id) ?? new Paciente();

        public void Agregar(Paciente paciente)
        {
            var pacientes = LeerTodos();
            paciente.Id = pacientes.Count > 0 ? pacientes.Max(p => p.Id) + 1 : 1;
            pacientes.Add(paciente);
            EscribirTodos(pacientes);
        }

        public void Editar(Paciente paciente)
        {
            var pacientes = LeerTodos();
            var existente = pacientes.FirstOrDefault(p => p.Id == paciente.Id);

            if (existente is null)
                return;

            existente.Nombre = paciente.Nombre;
            existente.Apellido = paciente.Apellido;
            existente.Email = paciente.Email;
            existente.Telefono = paciente.Telefono;
            EscribirTodos(pacientes);
        }

        public void Eliminar(int id)
        {
            var pacientes = LeerTodos();
            pacientes.RemoveAll(p => p.Id == id);
            EscribirTodos(pacientes);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, Header + Environment.NewLine);
        }

        private List<Paciente> LeerTodos()
        {
            return File.ReadAllLines(_filePath)
                .Skip(1)
                .Where(linea => !string.IsNullOrWhiteSpace(linea))
                .Select(linea => linea.Split(','))
                .Where(partes => partes.Length >= 5 && int.TryParse(partes[0], out _))
                .Select(partes => new Paciente
                {
                    Id = int.Parse(partes[0]),
                    Nombre = partes[1],
                    Apellido = partes[2],
                    Email = partes[3],
                    Telefono = partes[4]
                })
                .ToList();
        }

        private void EscribirTodos(IEnumerable<Paciente> pacientes)
        {
            var lineas = new List<string> { Header };
            lineas.AddRange(pacientes.Select(p =>
                $"{p.Id},{Limpiar(p.Nombre)},{Limpiar(p.Apellido)},{Limpiar(p.Email)},{Limpiar(p.Telefono)}"));
            File.WriteAllLines(_filePath, lineas);
        }

        private static string Limpiar(string? texto) =>
            (texto ?? string.Empty).Replace(",", ";");
    }
}
