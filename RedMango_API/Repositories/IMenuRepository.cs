using RedMango_API.Models;
using System.Drawing;

namespace EcomApi.Repositories
{
    public interface IMenuRepository
    {
        //GetMenuItems  and  GetMenuItem(int id)
        Task<List<MenuItem>> GetMenuItems();
        Task<MenuItem> GetMenuItem(int id);

    }
}
