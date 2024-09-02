using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Pustok.Business.Exceptions.CommonExceptions;
using Pustok.Business.Services.Interfaces;
using Pustok.Business.Utilities.Extensions;
using Pustok.Business.ViewModels;
using Pustok.Core.Models;
using Pustok.Core.Repositories;
using System.Linq.Expressions;

namespace Pustok.Business.Services.Implementations;

public class BookService : IBookService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IBookImageRepository _bookImageRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IWebHostEnvironment _env;

    public BookService(IGenreRepository genreRepository,
                IAuthorRepository authorRepository,
                IBookImageRepository bookImageRepository,
                IBookRepository bookRepository,
                IWebHostEnvironment env)
    {
        _genreRepository = genreRepository;
        _authorRepository = authorRepository;
        _bookImageRepository = bookImageRepository;
        _bookRepository = bookRepository;
        _env = env;
    }

    public async Task CreateAsync(BookCreateVM vm)
    {
        if (await _genreRepository.Table.AllAsync(x => x.Id != vm.GenreId))
        {
            throw new EntityNotFoundException("GenreId", "Genre not found");
        }

        if (await _authorRepository.Table.AllAsync(x => x.Id != vm.AuthorId))
        {
            throw new EntityNotFoundException("AuthorId", "Author not found");
        }

        Book book = new Book()
        {
            Title = vm.Title,
            Desc = vm.Desc,
            StockCount = vm.StockCount,
            CostPrice = vm.CostPrice,
            DiscountPercent = vm.DiscountPercent,
            IsAvailable = vm.IsAvailable,
            IsDeleted = false,
            ProductCode = vm.ProductCode,
            SalePrice = vm.SalePrice,
            AuthorId = vm.AuthorId,
            GenreId = vm.GenreId,
        };

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
                Book = book
            };

            await _bookImageRepository.CreateAsync(bookImage);
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
                Book = book
            };
            await _bookImageRepository.CreateAsync(bookImage);
        }

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
                    Book = book
                };
                await _bookImageRepository.CreateAsync(bookImage);
            }
        }

        await _bookRepository.CreateAsync(book);
        await _bookRepository.CommitAsync();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Book>> GetAllAsync(Expression<Func<Book, bool>> expression, params string[] includes)
    {
        return await _bookRepository.GetAll(expression, includes).ToListAsync();
    }

    public async Task<ICollection<Book>> GetAllByOrderDescAsync(Expression<Func<Book, bool>> expression, Expression<Func<Book, dynamic>> orderExpression, params string[] includes)
    {
        return await _bookRepository.GetAll(expression, includes).OrderByDescending(orderExpression).ToListAsync();
    }

    public async Task<Book> GetByExpressionAsync(Expression<Func<Book, bool>> expression, params string[] includes)
    {
        var data = await _bookRepository.GetByExpressionAsync(expression, includes);
        if (data is null) throw new EntityNotFoundException("Book not found");

        return data;

    }

    public async Task<Book> GetByIdAsync(int id)
    {
        var data = await _bookRepository.GetByIdAsync(id);

        if (data is null) throw new EntityNotFoundException("Book not found");

        return data;
    }

    public async Task<bool> IsExist(Expression<Func<Book, bool>> expression)
    {
        return await _bookRepository.Table.AnyAsync(expression); 
    }

    public Task UpdateAsync(int id, BookUpdateVM vm)
    {
        throw new NotImplementedException();
    }
}
