using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Repositories
{
    public class CsvCitaRepository : ICitaRepository
    {
        private const string Header = "Id,PacienteId,MedicoId,Fecha,Hora,Motivo,Estado";
        private readonly string _filePath;

        public CsvCitaRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "citas.csv");
            EnsureFileExists();
        }

        public CsvCitaRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        public IEnumerable<Cita> ObtenerTodos() => LeerTodos();

        public IEnumerable<Cita> ObtenerPorPaciente(int pacienteId) =>
            LeerTodos().Where(c => c.PacienteId == pacienteId);

        public Cita ObtenerPorId(int id) =>
            LeerTodos().FirstOrDefault(c => c.Id == id) ?? new Cita();

        public void Agregar(Cita cita)
        {
            var citas = LeerTodos();
            cita.Id = citas.Count > 0 ? citas.Max(c => c.Id) + 1 : 1;
            citas.Add(cita);
            EscribirTodos(citas);
        }

        public void Editar(Cita cita)
        {
            var citas = LeerTodos();
            var existente = citas.FirstOrDefault(c => c.Id == cita.Id);

            if (existente is null)
                return;

            existente.PacienteId = cita.PacienteId;
            existente.MedicoId = cita.MedicoId;
            existente.Fecha = cita.Fecha;
            existente.Hora = cita.Hora;
            existente.Motivo = cita.Motivo;
            existente.Estado = cita.Estado;
            EscribirTodos(citas);
        }

        public void Eliminar(int id)
        {
            var citas = LeerTodos();
            citas.RemoveAll(c => c.Id == id);
            EscribirTodos(citas);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, Header + Environment.NewLine);
        }

        private List<Cita> LeerTodos()
        {
            return File.ReadAllLines(_filePath)
                .Skip(1)
                .Where(linea => !string.IsNullOrWhiteSpace(linea))
                .Select(linea => linea.Split(','))
                .Where(partes => partes.Length >= 7
                    && int.TryParse(partes[0], out _)
                    && int.TryParse(partes[1], out _)
                    && int.TryParse(partes[2], out _)
                    && DateOnly.TryParseExact(partes[3], "yyyy-MM-dd", out _)
                    && TimeOnly.TryParseExact(partes[4], "HH:mm", out _))
                .Select(partes => new Cita
                {
                    Id = int.Parse(partes[0]),
                    PacienteId = int.Parse(partes[1]),
                    MedicoId = int.Parse(partes[2]),
                    Fecha = DateOnly.ParseExact(partes[3], "yyyy-MM-dd"),
                    Hora = TimeOnly.ParseExact(partes[4], "HH:mm"),
                    Motivo = partes[5],
                    Estado = partes[6]
                })
                .ToList();
        }

        private void EscribirTodos(IEnumerable<Cita> citas)
        {
            var lineas = new List<string> { Header };
            lineas.AddRange(citas.Select(c =>
                $"{c.Id},{c.PacienteId},{c.MedicoId},{c.Fecha:yyyy-MM-dd},{c.Hora:HH:mm},{Limpiar(c.Motivo)},{Limpiar(c.Estado)}"));
            File.WriteAllLines(_filePath, lineas);
        }

        private static string Limpiar(string? texto) =>
            (texto ?? string.Empty).Replace(",", ";");
    }
}
