using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Business.Exceptions.CommonExceptions;
using Pustok.Business.Services.Interfaces;
using Pustok.Business.Utilities.Extensions;
using Pustok.Business.ViewModels;
using Pustok.Core.Models;
using Pustok.Core.Repositories;
using Pustok.Data.Repositories;

namespace Pustok.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BookController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly IBookImageRepository _bookImageRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IBookRepository _bookRepository;

        public BookController(IGenreService genreService,
                        IAuthorService authorService,
                        IBookService bookService,
                        IBookImageRepository bookImageRepository,
                        IWebHostEnvironment env,
                        IBookRepository bookRepository
                       )
        {
            _genreService = genreService;
            _authorService = authorService;
            _bookService = bookService;
            _bookImageRepository = bookImageRepository;
            _env = env;
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _bookService.GetAllAsync(x => !x.IsDeleted, "BookImages", "Genre", "Author"));
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = await _genreService.GetAllAsync(x => !x.IsDeleted);
            ViewBag.Authors = await _authorService.GetAllAsync(x => !x.IsDeleted);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateVM vm)
        {
            ViewBag.Genres = await _genreService.GetAllAsync(x => !x.IsDeleted);
            ViewBag.Authors = await _authorService.GetAllAsync(x => !x.IsDeleted);

            if (!ModelState.IsValid) return View(vm);

            try
            {
                await _bookService.CreateAsync(vm);
            }
            catch (EntityNotFoundException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (FileValidationException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Genres = await _genreService.GetAllAsync(x => !x.IsDeleted);
            ViewBag.Authors = await _authorService.GetAllAsync(x => !x.IsDeleted);
            Book data = null;
            try
            {
                data = await _bookService.GetByExpressionAsync(x => x.Id == id, "BookImages", "Author", "Genre");
            }
            catch (EntityNotFoundException)
            {
                return View("Error");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            BookUpdateVM vm = new BookUpdateVM()
            {
                Title = data.Title,
                Desc = data.Desc,
                StockCount = data.StockCount,
                CostPrice = data.CostPrice,
                DiscountPercent = data.DiscountPercent,
                IsAvailable = data.IsAvailable,
                ProductCode = data.ProductCode,
                SalePrice = data.SalePrice,
                AuthorId = data.AuthorId,
                GenreId = data.GenreId,
                BookImages = data.BookImages
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, BookUpdateVM vm)
        {
            ViewBag.Genres = await _genreService.GetAllAsync(x => !x.IsDeleted);
            ViewBag.Authors = await _authorService.GetAllAsync(x => !x.IsDeleted);

            if (id < 1 || id is null)
            {
                return View("Error");
            }
            if (!ModelState.IsValid) return View();

            var existData = await _bookService.GetByExpressionAsync(x => x.Id == id, "BookImages", "Author", "Genre");

            if (existData is null)
            {
                return View("Error");
            }

            existData.Title = vm.Title;
            existData.Desc = vm.Desc;
            existData.StockCount = vm.StockCount;
            existData.CostPrice = vm.CostPrice;
            existData.DiscountPercent = vm.DiscountPercent;
            existData.IsAvailable = vm.IsAvailable;
            existData.IsDeleted = false;
            existData.ProductCode = vm.ProductCode;
            existData.SalePrice = vm.SalePrice;
            existData.AuthorId = vm.AuthorId;
            existData.GenreId = vm.GenreId;

            if (vm.PosterImageFile is not null)
            {
                if (vm.PosterImageFile.ContentType != "image/jpeg" && vm.PosterImageFile.ContentType != "image/png")
                {
                    throw new FileValidationException("PosterImageFile", "Content type must be png or jpeg");
                }

                if (vm.PosterImageFile.Length > 2097152)
                {
                    throw new FileValidationException("PosterImageFile", "Image size must be lower than 2 mb");
                }

                BookImage bookImage = new BookImage()
                {
                    ImageUrl = vm.PosterImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsDeleted = false,
                    IsPoster = true,
                    BookId = existData.Id
                };

                await _bookImageRepository.CreateAsync(bookImage);

                _bookImageRepository.Delete(existData.BookImages.FirstOrDefault(x => x.IsPoster == true));
                existData.BookImages.FirstOrDefault(x => x.IsPoster == true)?.ImageUrl.DeleteFile(_env.WebRootPath, "uploads/books");

            }

            if (vm.HoverImageFile is not null)
            {
                if (vm.HoverImageFile.ContentType != "image/jpeg" && vm.HoverImageFile.ContentType != "image/png")
                {
                    throw new FileValidationException("HoverImageFile", "Content type must be png or jpeg");
                }

                if (vm.HoverImageFile.Length > 2097152)
                {
                    throw new FileValidationException("HoverImageFile", "Image size must be lower than 2 mb");
                }

                BookImage bookImage = new BookImage()
                {
                    ImageUrl = vm.HoverImageFile.SaveFile(_env.WebRootPath, "uploads/books"),
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    IsDeleted = false,
                    IsPoster = false,
                    BookId = existData.Id
                };
                await _bookImageRepository.CreateAsync(bookImage);

                _bookImageRepository.Delete(existData.BookImages.FirstOrDefault(x => x.IsPoster == false));
                existData.BookImages.FirstOrDefault(x => x.IsPoster == false)?.ImageUrl.DeleteFile(_env.WebRootPath, "uploads/books");
            }

            if(vm.BookImageIds != null)
            {
                foreach (var item in existData.BookImages.Where(bi => !vm.BookImageIds.Exists(bid => bi.Id == bid) && bi.IsPoster == null))
                {
                    item.ImageUrl.DeleteFile(_env.WebRootPath, "uploads/books");
                }
                existData.BookImages.RemoveAll(bi => !vm.BookImageIds.Exists(bid => bi.Id == bid) && bi.IsPoster == null);
            }
            else
            {
                foreach (var item in existData.BookImages.Where(bi => bi.IsPoster == null))
                {
                    item.ImageUrl.DeleteFile(_env.WebRootPath, "uploads/books");
                }
                existData.BookImages.RemoveAll(bi => bi.IsPoster == null);
            }

            if (vm.ImageFiles is not null)
            {
                if (vm.ImageFiles.Count > 0)
                {
                    foreach (var imgFile in vm.ImageFiles)
                    {
                        if (imgFile.ContentType != "image/jpeg" && imgFile.ContentType != "image/png")
                        {
                            throw new FileValidationException("ImageFiles", "Content type must be png or jpeg");
                        }

                        if (imgFile.Length > 2097152)
                        {
                            throw new FileValidationException("ImageFiles", "Image size must be lower than 2 mb");
                        }

                        BookImage bookImage = new BookImage()
                        {
                            ImageUrl = imgFile.SaveFile(_env.WebRootPath, "uploads/books"),
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsDeleted = false,
                            IsPoster = null,
                            Book = existData
                        };
                        await _bookImageRepository.CreateAsync(bookImage);
                    }
                }
            }

            await _bookRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
