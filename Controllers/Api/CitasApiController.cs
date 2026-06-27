using CitasApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers.Api
{
    [ApiController]
    [Route("api/citas")]
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
            return Ok(new { citas, pacientes, medicos });
        }

        [HttpGet("porpaciente/{pacienteId:int}")]
        public IActionResult ObtenerPorPaciente(int pacienteId)
        {
            var (citas, pacientes, medicos) =
                _citaService.ObtenerCitasPorPaciente(pacienteId);

            return citas.Count == 0
                ? NotFound(new { mensaje = "No se encontraron citas para el paciente." })
                : Ok(new { citas, pacientes, medicos });
        }

        [HttpPost("confirmar/{citaId:int}")]
        public IActionResult Confirmar(int citaId)
        {
            var cita = _citaService.ConfirmarCita(citaId);
            return cita is null
                ? NotFound(new { mensaje = $"No existe la cita {citaId}." })
                : Ok(new { mensaje = "Cita confirmada", cita });
        }
    }
}
