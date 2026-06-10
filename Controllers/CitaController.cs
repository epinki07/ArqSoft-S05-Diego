using CitasApp.Interfaces;
using CitasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitasApp.Controllers
{
    public class CitaController : Controller
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IPacienteRepository _pacienteRepo;
        private readonly IMedicoRepository _medicoRepo;

        public CitaController(ICitaRepository citaRepo,
                              IPacienteRepository pacienteRepo,
                              IMedicoRepository medicoRepo)
        {
            _citaRepo = citaRepo;
            _pacienteRepo = pacienteRepo;
            _medicoRepo = medicoRepo;
        }

        public IActionResult Index()
        {
            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(_citaRepo.ObtenerTodos());
        }

        public IActionResult PorPaciente(int pacienteId)
        {
            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(_citaRepo.ObtenerPorPaciente(pacienteId));
        }

        public IActionResult Create()
        {
            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cita cita)
        {
            if (ModelState.IsValid)
            {
                _citaRepo.Agregar(cita);
                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        public IActionResult Edit(int id)
        {
            var cita = _citaRepo.ObtenerPorId(id);
            if (cita.Id == 0)
                return NotFound();

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        [HttpPost]
        public IActionResult Edit(Cita cita)
        {
            if (ModelState.IsValid)
            {
                _citaRepo.Editar(cita);
                return RedirectToAction("Index");
            }

            ViewBag.Pacientes = _pacienteRepo.ObtenerTodos();
            ViewBag.Medicos = _medicoRepo.ObtenerTodos();
            return View(cita);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _citaRepo.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}