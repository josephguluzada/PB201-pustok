using Pustok.Core.Models;

namespace Pustok.MVC.ViewModels
{
    public class HomeVM
    {
        public ICollection<Book> FeaturedBooks { get; set; }
        public ICollection<Book> NewBooks { get; set; }
        public ICollection<Book> ExpensiveBooks { get; set; }
    }
}
