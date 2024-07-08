using Biblioteca07.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca07.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Libros.ToListAsync());
        }

        public async Task <IActionResult> Libros()
        {
            return View(await _context.Libros.ToListAsync());
        }

        public async Task <IActionResult> Clientes()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        public async Task <IActionResult> Alquileres()
        {
            return View(await _context.Alquileres.ToListAsync());
        }

        public IActionResult CrearLibro()
        {
            return View();
        }
 
        [HttpPost]

        public IActionResult CrearLibro([Bind("Id,Titulo,Autor,CantidadCopias")] Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libro);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Libros));
            }
            return View(libro);
        }

        public IActionResult CrearCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearCliente([Bind("Id,Nombre")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Clientes));
            }
            return View(cliente);
        }

        public IActionResult CrearAlquiler()
        {
            ViewData["Libros"] = new SelectList(_context.Libros, "Id", "Titulo");
            ViewData["Clientes"] = new SelectList(_context.Clientes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult CrearAlquiler([Bind("Id,LibroId,ClienteId")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                var libro = _context.Libros.Find(alquiler.LibroId);
                if (libro != null && libro.CantidadCopias > 0)
                {
                    libro.CantidadCopias--;
                    alquiler.Devuelto = "Alquiler";
                    _context.Update(libro);
                    _context.Add(alquiler);
                    _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Alquileres));
                }
                else
                {
                    // Maneja el caso de que no haya copias disponibles
                    ModelState.AddModelError("", "No hay copias disponibles para alquilar.");
                }
            }
            ViewData["Libros"] = new SelectList(_context.Libros, "Id", "Titulo", alquiler.LibroId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "Id", "Nombre", alquiler.ClienteId);
            return View(alquiler);
        }

        public IActionResult CrearDevolucion()
        {
            ViewData["Libros"] = new SelectList(_context.Libros, "Id", "Titulo");
            ViewData["Clientes"] = new SelectList(_context.Clientes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult CrearDevolucion([Bind("Id,LibroId,ClienteId")] Alquiler alquiler)
        {
            if (ModelState.IsValid)
            {
                var libro = _context.Libros.Find(alquiler.LibroId);
                if (libro != null)
                {
                    libro.CantidadCopias++;
                    alquiler.Devuelto = "Devolucion";
                    _context.Update(libro);
                    _context.Update(alquiler);
                    _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Alquileres));
                }
                else
                {
                    ModelState.AddModelError("", "No hay copias disponibles para alquilar.");
                }
            }
            ViewData["Libros"] = new SelectList(_context.Libros, "Id", "Titulo", alquiler.LibroId);
            ViewData["Clientes"] = new SelectList(_context.Clientes, "Id", "Nombre", alquiler.ClienteId);
            return View(alquiler);
        }
    }
}