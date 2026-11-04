using Microsoft.AspNetCore.Mvc;
using MiniRentaSakila.Data;
using MiniRentaSakila.Filters;
using MiniRentaSakila.Models;

namespace MiniRentaSakila.Controllers
{
    [Autorizar]
    public class PeliculasController : Controller
    {
        private readonly PeliculaRepository _peliculaRepository;

        public PeliculasController(PeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        public IActionResult Index(string titulo)
        {
            titulo = titulo ?? string.Empty;
            ViewBag.Titulo = titulo;

            var peliculas = _peliculaRepository.ObtenerCatalogo(titulo);
            return View(peliculas);
        }

        public IActionResult Detalle(int id)
        {
            var pelicula = _peliculaRepository.ObtenerPorId(id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [Autorizar("Administrador")]
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Pelicula { TarifaAlquiler = 4.99m });
        }

        [Autorizar("Administrador")]
        [HttpPost]
        public IActionResult Crear(Pelicula pelicula)
        {
            if (!ModelState.IsValid)
            {
                return View(pelicula);
            }

            _peliculaRepository.Crear(pelicula);
            TempData["Mensaje"] = "Película creada correctamente.";
            return RedirectToAction("Index");
        }

        [Autorizar("Administrador")]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var pelicula = _peliculaRepository.ObtenerPorId(id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [Autorizar("Administrador")]
        [HttpPost]
        public IActionResult Editar(Pelicula pelicula)
        {
            if (!ModelState.IsValid)
            {
                return View(pelicula);
            }

            _peliculaRepository.Actualizar(pelicula);
            TempData["Mensaje"] = "Película actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [Autorizar("Administrador")]
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var pelicula = _peliculaRepository.ObtenerPorId(id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [Autorizar("Administrador")]
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            var pelicula = _peliculaRepository.ObtenerPorId(id);

            if (pelicula == null)
            {
                return NotFound();
            }

            if (pelicula.CopiasDisponibles < pelicula.CopiasTotales)
            {
                TempData["Error"] = "No se puede eliminar una película con copias actualmente alquiladas.";
                return RedirectToAction("Index");
            }

            _peliculaRepository.Desactivar(id);
            TempData["Mensaje"] = "Película eliminada del catálogo.";
            return RedirectToAction("Index");
        }
    }
}