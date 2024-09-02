using System.ComponentModel.DataAnnotations;

namespace Pustok.Core.Models;

public class Book : BaseEntity
{
    public int AuthorId { get; set; }
    public int GenreId { get; set; }
    public string ProductCode { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public int DiscountPercent { get; set; }
    public int StockCount { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }

    public Author Author { get; set; }
    public Genre Genre { get; set; }
    public List<BookImage> BookImages { get; set; }
}
