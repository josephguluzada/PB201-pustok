using Microsoft.AspNetCore.Mvc;
using Pustok.Business.Exceptions.GenreExceptions;
using Pustok.Business.Services.Interfaces;
using Pustok.Business.ViewModels;

namespace Pustok.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _genreService.GetAllAsync(null));
        }
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);    
            }

            try
            {
                await _genreService.CreateAsync(vm);
            }
            catch(GenreAlreadyExistException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var data = await _genreService.GetByIdAsync(id);

            if (data is null) throw new NullReferenceException();

            var genreVm = new GenreUpdateVM()
            {
                Name = data.Name,
            };

            return View(genreVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, GenreUpdateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            try
            {
                await _genreService.UpdateAsync(id, vm);
            }
            catch (GenreAlreadyExistException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
