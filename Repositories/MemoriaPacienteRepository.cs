using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Repositories
{
    public class MemoriaPacienteRepository : IPacienteRepository
    {
        private static readonly object SyncRoot = new();

        private static readonly List<Paciente> Pacientes =
        [
            new Paciente
            {
                Id = 1,
                Nombre = "Paciente",
                Apellido = "Producción",
                Email = "produccion@citasapp.local",
                Telefono = "9990000001"
            },
            new Paciente
            {
                Id = 2,
                Nombre = "Datos",
                Apellido = "En memoria",
                Email = "memoria@citasapp.local",
                Telefono = "9990000002"
            }
        ];

        public IEnumerable<Paciente> ObtenerTodos()
        {
            lock (SyncRoot)
            {
                return Pacientes.Select(Copiar).ToList();
            }
        }

        public Paciente ObtenerPorId(int id)
        {
            lock (SyncRoot)
            {
                var paciente = Pacientes.FirstOrDefault(p => p.Id == id);
                return paciente is null ? new Paciente() : Copiar(paciente);
            }
        }

        public void Agregar(Paciente paciente)
        {
            ArgumentNullException.ThrowIfNull(paciente);

            lock (SyncRoot)
            {
                paciente.Id = Pacientes.Count == 0 ? 1 : Pacientes.Max(p => p.Id) + 1;
                Pacientes.Add(Copiar(paciente));
            }
        }

        public void Editar(Paciente paciente)
        {
            ArgumentNullException.ThrowIfNull(paciente);

            lock (SyncRoot)
            {
                var existente = Pacientes.FirstOrDefault(p => p.Id == paciente.Id);
                if (existente is null)
                    return;

                existente.Nombre = paciente.Nombre;
                existente.Apellido = paciente.Apellido;
                existente.Email = paciente.Email;
                existente.Telefono = paciente.Telefono;
            }
        }

        public void Eliminar(int id)
        {
            lock (SyncRoot)
            {
                Pacientes.RemoveAll(p => p.Id == id);
            }
        }

        private static Paciente Copiar(Paciente paciente) => new()
        {
            Id = paciente.Id,
            Nombre = paciente.Nombre,
            Apellido = paciente.Apellido,
            Email = paciente.Email,
            Telefono = paciente.Telefono
        };
    }
}
