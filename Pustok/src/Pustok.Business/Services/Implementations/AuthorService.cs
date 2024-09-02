using Microsoft.EntityFrameworkCore;
using Pustok.Business.Exceptions.AuthorExceptions;
using Pustok.Business.Exceptions.CommonExceptions;
using Pustok.Business.Services.Interfaces;
using Pustok.Business.ViewModels;
using Pustok.Core.Models;
using Pustok.Core.Repositories;
using System.Linq.Expressions;

namespace Pustok.Business.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }
    public async Task CreateAsync(AuthorCreateVM vm)
    {
        if (string.IsNullOrEmpty(vm.FullName))
        {
            throw new AuthorFullnameException("FullName","Author Fullname can't be empty");
        }

        var data = new Author()
        {
            FullName = vm.FullName,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            IsDeleted = vm.IsDeleted
        };
        await _authorRepository.CreateAsync(data);
        await _authorRepository.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = await _authorRepository.GetByIdAsync(id);

        if(data is null)
        {
            throw new EntityNotFoundException("Author not found");
        }

        _authorRepository.Delete(data);
        await _authorRepository.CommitAsync();
    }

    public async Task<ICollection<Author>> GetAllAsync(Expression<Func<Author, bool>> expression)
    {
        return await _authorRepository.GetAll(expression).ToListAsync();
    }

    public async Task<Author> GetByIdAsync(int? id)
    {
        if (id < 1 || id is null)
        {
            throw new IdIsNotValidException("Id not valid");
        }

        return await _authorRepository.GetByIdAsync(id);
    }

    public async Task UpdateAsync(int? id, AuthorUpdateVM vm)
    {
        if(id < 1 || id is null)
        {
            throw new IdIsNotValidException("Id not valid");
        }
        if (string.IsNullOrEmpty(vm.FullName))
        {
            throw new AuthorFullnameException("FullName", "Author Fullname can't be empty");
        }
        var data = await _authorRepository.GetByIdAsync(id);

        if (data is null)
        {
            throw new EntityNotFoundException("Author not found");
        }

        data.FullName = vm.FullName;
        data.UpdatedDate = DateTime.UtcNow;

        await _authorRepository.CommitAsync();
    }
}
