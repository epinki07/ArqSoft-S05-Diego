using System.Diagnostics;
using ArqSoft_S05_Diego.Interfaces;
using ArqSoft_S05_Diego.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArqSoft_S05_Diego.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPacienteRepository _pacienteRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ICitaRepository _citaRepo;

        public HomeController(
            IPacienteRepository pacienteRepo,
            IMedicoRepository medicoRepo,
            ICitaRepository citaRepo)
        {
            _pacienteRepo = pacienteRepo;
            _medicoRepo = medicoRepo;
            _citaRepo = citaRepo;
        }

        public IActionResult Index()
        {
            var citas = _citaRepo.ObtenerTodos().ToList();

            ViewBag.TotalPacientes = _pacienteRepo.ObtenerTodos().Count();
            ViewBag.TotalMedicos = _medicoRepo.ObtenerTodos().Count();
            ViewBag.TotalCitas = citas.Count;
            ViewBag.CitasPendientes = citas.Count(c => c.Estado == "Pendiente");

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
