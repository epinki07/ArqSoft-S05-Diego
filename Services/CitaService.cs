using CitasApp.Interfaces;
using CitasApp.Models;

namespace CitasApp.Services
{
    public class CitaService
    {
        private readonly ICitaRepository _citaRepository;
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IReadOnlyCollection<ICitaObserver> _observers;

        public CitaService(
            ICitaRepository citaRepository,
            IPacienteRepository pacienteRepository,
            IMedicoRepository medicoRepository,
            IEnumerable<ICitaObserver> observers)
        {
            _citaRepository = citaRepository;
            _pacienteRepository = pacienteRepository;
            _medicoRepository = medicoRepository;
            _observers = observers.ToList();
        }

        public (List<Cita> citas, List<Paciente> pacientes, List<Medico> medicos)
            ObtenerTodasLasCitas()
        {
            return (
                _citaRepository.ObtenerTodos().ToList(),
                _pacienteRepository.ObtenerTodos().ToList(),
                _medicoRepository.ObtenerTodos().ToList());
        }

        public (List<Cita> citas, List<Paciente> pacientes, List<Medico> medicos)
            ObtenerCitasPorPaciente(int pacienteId)
        {
            return (
                _citaRepository.ObtenerPorPaciente(pacienteId).ToList(),
                _pacienteRepository.ObtenerTodos().ToList(),
                _medicoRepository.ObtenerTodos().ToList());
        }

        public Cita? ConfirmarCita(int citaId)
        {
            var cita = _citaRepository.ObtenerPorId(citaId);
            if (cita.Id == 0)
                return null;

            if (!string.Equals(cita.Estado, "Confirmada", StringComparison.OrdinalIgnoreCase))
            {
                cita.Estado = "Confirmada";
                _citaRepository.Editar(cita);
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Cita {cita.Id} confirmada");

            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnCitaConfirmada(cita);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[WARNING] {observer.GetType().Name} falló al notificar la cita " +
                        $"{cita.Id}: {ex.Message}");
                }
            }

            return cita;
        }
    }
}
