using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class PacienteController : Controller
    {
        private readonly IPacienteRepository _repo;
        public PacienteController(IPacienteRepository repo) { _repo = repo; }

        public IActionResult Index() => View(_repo.ObtenerTodos());

        public IActionResult Detalle(int id)
        {
            var paciente = _repo.ObtenerPorId(id);
            return paciente.Id == 0 ? NotFound() : View(paciente);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                _repo.Agregar(paciente);
                return RedirectToAction("Index");
            }
            return View(paciente);
        }

        public IActionResult Edit(int id)
        {
            var paciente = _repo.ObtenerPorId(id);
            if (paciente.Id == 0)
                return NotFound();
            return View(paciente);
        }

        [HttpPost]
        public IActionResult Edit(Paciente paciente)
        {
            if (ModelState.IsValid)
            {
                _repo.Editar(paciente);
                return RedirectToAction("Index");
            }
            return View(paciente);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repo.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}
