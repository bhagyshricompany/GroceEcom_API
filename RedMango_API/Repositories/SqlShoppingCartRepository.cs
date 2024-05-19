using Microsoft.EntityFrameworkCore;
using RedMango_API.Data;
using RedMango_API.Models;

namespace EcomApi.Repositories
{
    public class SqlShoppingCartRepository:IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public SqlShoppingCartRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string userId)
        {
            ShoppingCart shoppingCart = await _db.ShoppingCarts
                .Include(u => u.CartItems)
                .ThenInclude(u => u.MenuItem)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (shoppingCart != null && shoppingCart.CartItems != null)
            {
                shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
            }

            return shoppingCart;
        }

        public async Task AddOrUpdateItemInCartAsync(string userId, int menuItemId, int updateQuantityBy)
        {
            ShoppingCart shoppingCart = await _db.ShoppingCarts
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            MenuItem menuItem = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == menuItemId);
            if (menuItem == null) return;

            if (shoppingCart == null && updateQuantityBy > 0)
            {
                ShoppingCart newCart = new() { UserId = userId };
                await _db.ShoppingCarts.AddAsync(newCart);
                await _db.SaveChangesAsync();

                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = newCart.Id,
                    MenuItem = null
                };
                await _db.CartItems.AddAsync(newCartItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                CartItem cartItemInCart = shoppingCart.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);
                if (cartItemInCart == null)
                {
                    CartItem newCartItem = new()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.Id,
                        MenuItem = null
                    };
                    await _db.CartItems.AddAsync(newCartItem);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                    if (updateQuantityBy == 0 || newQuantity <= 0)
                    {
                        _db.CartItems.Remove(cartItemInCart);
                        if (shoppingCart.CartItems.Count == 1)
                        {
                            _db.ShoppingCarts.Remove(shoppingCart);
                        }
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        cartItemInCart.Quantity = newQuantity;
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
