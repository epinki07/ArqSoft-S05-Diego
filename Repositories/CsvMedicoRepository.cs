using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Repositories
{
    public class CsvMedicoRepository : IMedicoRepository
    {
        private const string Header = "Id,Nombre,Apellido,Especialidad,NumeroLicencia";
        private readonly string _filePath;

        public CsvMedicoRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "medicos.csv");
            EnsureFileExists();
        }

        public CsvMedicoRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        public IEnumerable<Medico> ObtenerTodos() => LeerTodos();

        public Medico ObtenerPorId(int id) =>
            LeerTodos().FirstOrDefault(m => m.Id == id) ?? new Medico();

        public void Agregar(Medico medico)
        {
            var medicos = LeerTodos();
            medico.Id = medicos.Count > 0 ? medicos.Max(m => m.Id) + 1 : 1;
            medicos.Add(medico);
            EscribirTodos(medicos);
        }

        public void Editar(Medico medico)
        {
            var medicos = LeerTodos();
            var existente = medicos.FirstOrDefault(m => m.Id == medico.Id);

            if (existente is null)
                return;

            existente.Nombre = medico.Nombre;
            existente.Apellido = medico.Apellido;
            existente.Especialidad = medico.Especialidad;
            existente.NumeroLicencia = medico.NumeroLicencia;
            EscribirTodos(medicos);
        }

        public void Eliminar(int id)
        {
            var medicos = LeerTodos();
            medicos.RemoveAll(m => m.Id == id);
            EscribirTodos(medicos);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, Header + Environment.NewLine);
        }

        private List<Medico> LeerTodos()
        {
            return File.ReadAllLines(_filePath)
                .Skip(1)
                .Where(linea => !string.IsNullOrWhiteSpace(linea))
                .Select(linea => linea.Split(','))
                .Where(partes => partes.Length >= 5 && int.TryParse(partes[0], out _))
                .Select(partes => new Medico
                {
                    Id = int.Parse(partes[0]),
                    Nombre = partes[1],
                    Apellido = partes[2],
                    Especialidad = partes[3],
                    NumeroLicencia = partes[4]
                })
                .ToList();
        }

        private void EscribirTodos(IEnumerable<Medico> medicos)
        {
            var lineas = new List<string> { Header };
            lineas.AddRange(medicos.Select(m =>
                $"{m.Id},{Limpiar(m.Nombre)},{Limpiar(m.Apellido)},{Limpiar(m.Especialidad)},{Limpiar(m.NumeroLicencia)}"));
            File.WriteAllLines(_filePath, lineas);
        }

        private static string Limpiar(string? texto) =>
            (texto ?? string.Empty).Replace(",", ";");
    }
}
