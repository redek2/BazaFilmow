using BazaFilmow.Data;
using BazaFilmow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BazaFilmow.Controllers
{
    public class FilmyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FilmyController> _logger;

        public FilmyController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<FilmyController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
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
        public async Task<IActionResult> Create(Film film, IFormFile? plikOkladki)
        {
            if (ModelState.IsValid)
            {
                if (plikOkladki != null && plikOkladki.Length > 0)
                {
                    var dozwoloneRozszerzenia = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var dozwoloneTypyMime = new[] { "image/jpeg", "image/png", "image/webp" };

                    var rozszerzenie = Path.GetExtension(plikOkladki.FileName).ToLowerInvariant();
                    var typMime = plikOkladki.ContentType.ToLowerInvariant();

                    if (string.IsNullOrEmpty(rozszerzenie) || !dozwoloneRozszerzenia.Contains(rozszerzenie) || !dozwoloneTypyMime.Contains(typMime))
                    {
                        ModelState.AddModelError("SciezkaOkladki", "Niedozwolony format pliku. Akceptowane są tylko obrazy (JPG, PNG, WEBP).");
                        return View(film);
                    }

                    string folderDocelowy = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(folderDocelowy);
                    string nazwaWlasnaPliku = Guid.NewGuid().ToString() + Path.GetExtension(plikOkladki.FileName);
                    string sciezkaDocelowa = Path.Combine(folderDocelowy, nazwaWlasnaPliku);

                    using (FileStream stream = new FileStream(sciezkaDocelowa, FileMode.Create))
                    {
                        await plikOkladki.CopyToAsync(stream);
                    }

                    film.SciezkaOkladki = "/images/" + nazwaWlasnaPliku;
                }

                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var film = await _context.Filmy.FindAsync(id);

            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Film zaktualizowanyFilm, IFormFile? plikOkladki, bool usunOkladke = false)
        {
            if (id != zaktualizowanyFilm.Id) return NotFound();

            var filmWBazie = await _context.Filmy.FindAsync(id);
            if (filmWBazie == null) return NotFound();

            if (ModelState.IsValid)
            {
                filmWBazie.Tytul = zaktualizowanyFilm.Tytul;
                filmWBazie.Dlugosc = zaktualizowanyFilm.Dlugosc;
                filmWBazie.Rezyser = zaktualizowanyFilm.Rezyser;

                if (plikOkladki != null && plikOkladki.Length > 0)
                {
                    var dozwoloneRozszerzenia = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                    var dozwoloneTypyMime = new[] { "image/jpeg", "image/png", "image/webp" };

                    var rozszerzenie = Path.GetExtension(plikOkladki.FileName).ToLowerInvariant();
                    var typMime = plikOkladki.ContentType.ToLowerInvariant();

                    if (string.IsNullOrEmpty(rozszerzenie) || !dozwoloneRozszerzenia.Contains(rozszerzenie) || !dozwoloneTypyMime.Contains(typMime))
                    {
                        ModelState.AddModelError("SciezkaOkladki", "Niedozwolony format pliku. Akceptowane są tylko obrazy (JPG, PNG, WEBP).");
                        return View(zaktualizowanyFilm);
                    }

                    if (!string.IsNullOrEmpty(filmWBazie.SciezkaOkladki))
                    {
                        var staraSciezka = Path.Combine(_webHostEnvironment.WebRootPath, filmWBazie.SciezkaOkladki.TrimStart('/'));
                        if (System.IO.File.Exists(staraSciezka)) System.IO.File.Delete(staraSciezka);
                    }

                    string folderDocelowy = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(folderDocelowy);
                    string nazwaWlasnaPliku = Guid.NewGuid().ToString() + rozszerzenie;
                    string sciezkaDocelowa = Path.Combine(folderDocelowy, nazwaWlasnaPliku);

                    using (FileStream stream = new FileStream(sciezkaDocelowa, FileMode.Create))
                    {
                        await plikOkladki.CopyToAsync(stream);
                    }
                    filmWBazie.SciezkaOkladki = "/images/" + nazwaWlasnaPliku;
                }
                else if (usunOkladke && !string.IsNullOrEmpty(filmWBazie.SciezkaOkladki))
                {
                    var staraSciezka = Path.Combine(_webHostEnvironment.WebRootPath, filmWBazie.SciezkaOkladki.TrimStart('/'));
                    if (System.IO.File.Exists(staraSciezka))
                    {
                        System.IO.File.Delete(staraSciezka);
                    }
                    filmWBazie.SciezkaOkladki = null;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(zaktualizowanyFilm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Filmy.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (film == null) return NotFound();

            return View(film);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Filmy.FindAsync(id);
            if (film == null) return NotFound();
            string? sciezkaDoPliku = film.SciezkaOkladki;
            _context.Filmy.Remove(film);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(sciezkaDoPliku))
            {
                string fizycznaSciezka = Path.Combine(_webHostEnvironment.WebRootPath, sciezkaDoPliku.TrimStart('/'));

                try
                {
                    if (System.IO.File.Exists(fizycznaSciezka))
                    {
                        System.IO.File.Delete(fizycznaSciezka);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Nie usunięto pliku: {Path}", fizycznaSciezka);
                }

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Filmy.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (film == null) return NotFound();

            return View(film);
        }
    }
}