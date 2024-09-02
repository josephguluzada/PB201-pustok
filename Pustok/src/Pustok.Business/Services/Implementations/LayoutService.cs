using Microsoft.AspNetCore.Identity;
using Pustok.Business.Services.Interfaces;
using Pustok.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustok.Business.Services.Implementations
{
    public class LayoutService : ILayoutService
    {
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> GetUser(string username)
        {
            AppUser user = null;

            user = await _userManager.FindByNameAsync(username);

            return user;
        }
    }
}
