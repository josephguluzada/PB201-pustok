using Microsoft.EntityFrameworkCore;
using Pustok.Business.Exceptions.GenreExceptions;
using Pustok.Business.Services.Interfaces;
using Pustok.Business.ViewModels;
using Pustok.Core.Models;
using Pustok.Core.Repositories;
using System.Linq.Expressions;

namespace Pustok.Business.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;

    public GenreService(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task CreateAsync(GenreCreateViewModel vm)
    {
        if(await _genreRepository.Table.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower()))
        {
            throw new GenreAlreadyExistException("Name", "Genre already exist");
        }
        
        var entity = new Genre()
        {
            Name = vm.Name,
            IsDeleted = false,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now 
        };

        await _genreRepository.CreateAsync(entity);
        await _genreRepository.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _genreRepository.GetByIdAsync(id);

        if (entity is null) 
        {
            throw new NullReferenceException();
        }

        _genreRepository.Delete(entity);
        await _genreRepository.CommitAsync();
    }

    public async Task<ICollection<Genre>> GetAllAsync(Expression<Func<Genre, bool>> expression)
    {
        return await _genreRepository.GetAll(expression).ToListAsync();
    }

    public async Task<Genre> GetByIdAsync(int id)
    {
        var entity = await _genreRepository.GetByIdAsync(id);

        if (entity is null)
        {
            throw new NullReferenceException();
        }

        return entity;
    }

    public async Task UpdateAsync(int id, GenreUpdateVM vm)
    {
        if (await _genreRepository.Table.AnyAsync(x => x.Name.ToLower() == vm.Name.ToLower() && x.Id != id))
        {
            throw new GenreAlreadyExistException("Name", "Genre already exist");
        }

        var entity = await _genreRepository.GetByIdAsync(id);

        if (entity is null)
        {
            throw new NullReferenceException();
        }
        entity.Name = vm.Name;
        entity.UpdatedDate = DateTime.Now;

        await _genreRepository.CommitAsync();
    }
}
