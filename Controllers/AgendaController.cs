using System.Security.Claims;
using CitasApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    [Authorize]
    public class AgendaController : Controller
    {
        private readonly IAgendaRepository _agendaRepository;

        public AgendaController(IAgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        public IActionResult Index()
        {
            var agenda = User.IsInRole("Paciente")
                ? _agendaRepository.ObtenerPorPaciente(ObtenerClaimEntero("PacienteId"))
                : _agendaRepository.ObtenerTodos();

            ViewBag.EsPaciente = User.IsInRole("Paciente");
            return View(agenda);
        }

        private int ObtenerClaimEntero(string claimType)
        {
            var valor = User.FindFirstValue(claimType);
            return int.TryParse(valor, out var id) ? id : 0;
        }
    }
}
