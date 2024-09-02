using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pustok.Business.Services.Interfaces;
using Pustok.MVC.ViewModels;
using System.Diagnostics;

namespace Pustok.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;

        public HomeController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                FeaturedBooks = await _bookService.GetAllAsync(x => !x.IsDeleted && x.IsFeatured, "BookImages", "Author", "Genre"),
                NewBooks = await _bookService.GetAllAsync(x => !x.IsDeleted && x.IsNew, "BookImages", "Author", "Genre"),
                ExpensiveBooks = await _bookService.GetAllByOrderDescAsync(x => !x.IsDeleted, x=>x.SalePrice, "BookImages", "Author", "Genre")
            };

            return View(homeVM);
        }

        public async Task<IActionResult> AddToBasket(int? bookId)
        {
            if(bookId < 1 || bookId is null)
            {
                return NotFound();
            }

            if(await _bookService.IsExist(x=>x.Id == bookId) == false)
            {
                return NotFound();
            }

            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            BasketItemVM basketItem = null;


            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if(basketItemsStr is not null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemVM>>(basketItemsStr);

                basketItem = basketItems.FirstOrDefault(x=>x.BookId == bookId);

                if(basketItem is not null)
                {
                    basketItem.Count++;
                }
                else
                {
                    basketItem = new BasketItemVM()
                    {
                        BookId = bookId,
                        Count = 1
                    };
                    basketItems.Add(basketItem);
                }
            }
            else
            {
                basketItem = new BasketItemVM()
                {
                    BookId = bookId,
                    Count = 1
                };
                basketItems.Add(basketItem);
            }

            basketItemsStr = JsonConvert.SerializeObject(basketItems);

            HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);

            return Ok();
        }

        public IActionResult GetBasketItems()
        {
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if(basketItemsStr is not null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemVM>>(basketItemsStr);
            }

            return Ok(basketItems);
        }

        //public IActionResult SetSession()
        //{
        //    HttpContext.Session.SetString("Name", "Huseyn");


        //    return Content("Add olundu BEY");
        //}

        //public IActionResult GetSession()
        //{
        //    string data = HttpContext.Session.GetString("Name");

        //    return Content(data);
        //}

        //public IActionResult RemoveSession()
        //{
        //    HttpContext.Session.Remove("Name");

        //    return Content("Bas uste BEY");
        //}

        //public IActionResult SetCookie(int id)
        //{
        //    List<int> ids = new List<int>();
        //    string idsStr = HttpContext.Request.Cookies["Ids"];
        //    if (idsStr is not null)
        //    {
        //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
        //    }
        //    ids.Add(id);

        //    idsStr = JsonConvert.SerializeObject(ids);

        //    HttpContext.Response.Cookies.Append("Ids", idsStr);

        //    return Content("Add olundu");
        //}

        //public IActionResult GetCookie()
        //{
        //    string idsStr = HttpContext.Request.Cookies["Ids"];
        //    List<int> ids = new List<int>();
        //    if (idsStr is not null)
        //    {
        //        ids = JsonConvert.DeserializeObject<List<int>>(idsStr);
        //    }

        //    return Ok(ids);
        //}
    }
}
