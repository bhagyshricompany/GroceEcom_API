using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;
using System.Net;

namespace EcomApi.Repositories
{
    public class SQLMenuitemRepository : IMenuRepository
    {

        private readonly ApplicationDbContext _db;
        public SQLMenuitemRepository(ApplicationDbContext dbcontext)
        {
            this._db = dbcontext;
            
        }
        public  async Task<List<MenuItem>> GetMenuItems()
        {
            return  await _db.MenuItems.ToListAsync();
        }

        public async Task<MenuItem> GetMenuItem(int id)
        {
            return await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
