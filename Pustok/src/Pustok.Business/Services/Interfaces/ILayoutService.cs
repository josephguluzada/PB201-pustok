using Pustok.Core.Models;

namespace Pustok.Business.Services.Interfaces;

public interface ILayoutService
{
    Task<AppUser> GetUser(string username);
}
