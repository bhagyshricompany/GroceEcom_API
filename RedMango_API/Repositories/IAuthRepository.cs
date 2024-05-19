using RedMango_API.Models.Dto;
using RedMango_API.Models;

namespace EcomApi.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IEnumerable<string>> GetUserRolesAsync(ApplicationUser user);
        Task<ApplicationUser> RegisterUserAsync(RegisterRequestDTO model);
        Task<bool> RoleExistsAsync(string roleName);
        Task CreateRoleAsync(string roleName);
        Task AddToRoleAsync(ApplicationUser user, string roleName);
    }
}
