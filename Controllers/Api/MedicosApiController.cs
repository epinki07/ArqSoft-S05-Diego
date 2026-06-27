using CitasApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers.Api
{
    [ApiController]
    [Route("api/medicos")]
    public class MedicosApiController : ControllerBase
    {
        private readonly IMedicoRepository _medicoRepository;

        public MedicosApiController(IMedicoRepository medicoRepository)
        {
            _medicoRepository = medicoRepository;
        }

        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            return Ok(_medicoRepository.ObtenerTodos());
        }

        [HttpGet("{id:int}")]
        public IActionResult ObtenerPorId(int id)
        {
            var medico = _medicoRepository.ObtenerPorId(id);
            return medico.Id == 0 ? NotFound() : Ok(medico);
        }
    }
}
