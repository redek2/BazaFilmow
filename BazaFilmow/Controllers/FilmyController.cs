using BazaFilmow.Data;
using BazaFilmow.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace KatalogFilmow.Controllers
{
    public class FilmyController : Controller
    {
        private readonly AppDbContext _context;
        public FilmyController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var filmy = await _context.Filmy.ToListAsync();
            return View(filmy);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Filmy.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }
    }
}