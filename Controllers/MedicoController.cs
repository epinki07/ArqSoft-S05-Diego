using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MedicoController : Controller
    {
        private readonly IMedicoRepository _repo;
        public MedicoController(IMedicoRepository repo) { _repo = repo; }

        public IActionResult Index() => View(_repo.ObtenerTodos());

        public IActionResult Detalle(int id)
        {
            var medico = _repo.ObtenerPorId(id);
            return medico == null ? NotFound() : View(medico);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Medico medico)
        {
            if (ModelState.IsValid)
            {
                _repo.Agregar(medico);
                return RedirectToAction("Index");
            }
            return View(medico);
        }

        public IActionResult Edit(int id)
        {
            var medico = _repo.ObtenerPorId(id);
            if (medico.Id == 0)
                return NotFound();
            return View(medico);
        }

        [HttpPost]
        public IActionResult Edit(Medico medico)
        {
            if (ModelState.IsValid)
            {
                _repo.Editar(medico);
                return RedirectToAction("Index");
            }
            return View(medico);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repo.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}
