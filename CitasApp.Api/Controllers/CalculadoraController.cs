using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculadoraController : ControllerBase
{
    [HttpGet("sumar")]
    public IActionResult Sumar([FromQuery] decimal a, [FromQuery] decimal b)
    {
        return Ok(new { operacion = "Suma", resultado = a + b });
    }

    [HttpGet("restar")]
    public IActionResult Restar([FromQuery] decimal a, [FromQuery] decimal b)
    {
        return Ok(new { operacion = "Resta", resultado = a - b });
    }

    [HttpGet("multiplicar")]
    public IActionResult Multiplicar([FromQuery] decimal a, [FromQuery] decimal b)
    {
        return Ok(new { operacion = "Multiplicación", resultado = a * b });
    }

    [HttpGet("dividir")]
    public IActionResult Dividir([FromQuery] decimal a, [FromQuery] decimal b)
    {
        if (b == 0)
        {
            return BadRequest(new { operacion = "División", mensaje = "No se puede dividir entre cero" });
        }

        return Ok(new { operacion = "División", resultado = a / b });
    }
}
