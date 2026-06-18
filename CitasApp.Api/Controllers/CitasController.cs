using CitasApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitasController : ControllerBase
{
    private readonly CitaService _citaService;

    public CitasController(CitaService citaService)
    {
        _citaService = citaService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var (citas, pacientes, medicos) = _citaService.ObtenerTodasLasCitas();
        return Ok(new { citas, pacientes, medicos });
    }

    [HttpGet("porpaciente/{pacienteId:int}")]
    public IActionResult PorPaciente(int pacienteId)
    {
        var (citas, pacientes, medicos) = _citaService.ObtenerCitasPorPaciente(pacienteId);
        return citas.Count == 0 ? NotFound() : Ok(new { citas, pacientes, medicos });
    }
}
