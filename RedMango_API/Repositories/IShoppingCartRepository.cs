using RedMango_API.Models;

namespace EcomApi.Repositories
{
    public interface IShoppingCartRepository
    {
        //Task<ShoppingCart> GetShoppingCartAsync(string userId);
        //Task AddOrUpdateItemInCartAsync(string userId, int menuItemId, int updateQuantityBy);
        Task<ShoppingCart> GetShoppingCartAsync(string userId);
        Task AddOrUpdateItemInCartAsync(string userId, int menuItemId, int updateQuantityBy);
    

}
}
