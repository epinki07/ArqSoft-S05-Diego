using CitasApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers.Api
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesApiController : ControllerBase
    {
        private readonly IPacienteRepository _pacienteRepository;

        public PacientesApiController(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            return Ok(_pacienteRepository.ObtenerTodos());
        }

        [HttpGet("{id:int}")]
        public IActionResult ObtenerPorId(int id)
        {
            var paciente = _pacienteRepository.ObtenerPorId(id);
            return paciente.Id == 0 ? NotFound() : Ok(paciente);
        }
    }
}
