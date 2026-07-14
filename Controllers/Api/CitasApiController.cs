using CitasApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitasApp.Controllers.Api
{
    [ApiController]
    [Route("api/citas")]
    [Authorize]
    public class CitasApiController : ControllerBase
    {
        private readonly CitaService _citaService;

        public CitasApiController(CitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet]
        public IActionResult ObtenerTodas()
        {
            var (citas, pacientes, medicos) = _citaService.ObtenerTodasLasCitas();
            if (User.IsInRole("Medico"))
            {
                var medicoId = ObtenerClaimEntero("MedicoId");
                citas = citas.Where(c => c.MedicoId == medicoId).ToList();
            }
            else if (User.IsInRole("Paciente"))
            {
                var pacienteId = ObtenerClaimEntero("PacienteId");
                citas = citas.Where(c => c.PacienteId == pacienteId).ToList();
                pacientes = pacientes.Where(p => p.Id == pacienteId).ToList();
            }

            return Ok(new { citas, pacientes, medicos });
        }

        [HttpGet("porpaciente/{pacienteId:int}")]
        public IActionResult ObtenerPorPaciente(int pacienteId)
        {
            if (User.IsInRole("Paciente") && ObtenerClaimEntero("PacienteId") != pacienteId)
                return Forbid();

            var (citas, pacientes, medicos) =
                _citaService.ObtenerCitasPorPaciente(pacienteId);

            if (User.IsInRole("Medico"))
            {
                var medicoId = ObtenerClaimEntero("MedicoId");
                citas = citas.Where(c => c.MedicoId == medicoId).ToList();
            }

            return citas.Count == 0
                ? NotFound(new { mensaje = "No se encontraron citas para el paciente." })
                : Ok(new { citas, pacientes, medicos });
        }

        [HttpPost("confirmar/{citaId:int}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Confirmar(int citaId)
        {
            var cita = _citaService.ConfirmarCita(citaId);
            return cita is null
                ? NotFound(new { mensaje = $"No existe la cita {citaId}." })
                : Ok(new { mensaje = "Cita confirmada", cita });
        }

        private int ObtenerClaimEntero(string claimType)
        {
            var valor = User.FindFirstValue(claimType);
            return int.TryParse(valor, out var id) ? id : 0;
        }
    }
}
