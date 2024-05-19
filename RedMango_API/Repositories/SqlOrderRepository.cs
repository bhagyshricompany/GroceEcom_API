using RedMango_API.Data;
using RedMango_API.Models.Dto;
using RedMango_API.Models;
using RedMango_API.Utility;
using Microsoft.EntityFrameworkCore;

namespace EcomApi.Repositories
{
    public class SqlOrderRepository:IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public SqlOrderRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<OrderHeader>> GetOrdersAsync(string userId, string searchString, string status, int pageNumber, int pageSize)
        {
            var orderHeaders = _db.OrderHeaders.Include(u => u.OrderDetails)
                                                .ThenInclude(u => u.MenuItem)
                                                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                orderHeaders = orderHeaders.Where(u => u.ApplicationUserId == userId);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderHeaders = orderHeaders.Where(u => u.PickupPhoneNumber.ToLower().Contains(searchString.ToLower()) ||
                                                       u.PickupEmail.ToLower().Contains(searchString.ToLower()) ||
                                                       u.PickupName.ToLower().Contains(searchString.ToLower()));
            }

            if (!string.IsNullOrEmpty(status))
            {
                orderHeaders = orderHeaders.Where(u => u.Status.ToLower() == status.ToLower());
            }

            return await orderHeaders.OrderByDescending(u => u.OrderHeaderId)
                                     .Skip((pageNumber - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToListAsync();
        }

        public async Task<OrderHeader> GetOrderByIdAsync(int id)
        {
            return await _db.OrderHeaders.Include(u => u.OrderDetails)
                                         .ThenInclude(u => u.MenuItem)
                                         .FirstOrDefaultAsync(u => u.OrderHeaderId == id);
        }

        public async Task<OrderHeader> CreateOrderAsync(OrderHeaderCreateDTO orderHeaderDTO)
        {
            var order = new OrderHeader
            {
                ApplicationUserId = orderHeaderDTO.ApplicationUserId,
                PickupEmail = orderHeaderDTO.PickupEmail,
                PickupName = orderHeaderDTO.PickupName,
                PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                OrderTotal = orderHeaderDTO.OrderTotal,
                OrderDate = DateTime.UtcNow,
                StripePaymentIntentID = orderHeaderDTO.StripePaymentIntentID,
                TotalItems = orderHeaderDTO.TotalItems,
                Status = string.IsNullOrEmpty(orderHeaderDTO.Status) ? SD.status_pending : orderHeaderDTO.Status,
            };

            _db.OrderHeaders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var orderDetailDTO in orderHeaderDTO.OrderDetailsDTO)
            {
                var orderDetails = new OrderDetails
                {
                    OrderHeaderId = order.OrderHeaderId,
                    ItemName = orderDetailDTO.ItemName,
                    MenuItemId = orderDetailDTO.MenuItemId,
                    Price = orderDetailDTO.Price,
                    Quantity = orderDetailDTO.Quantity,
                };
                _db.OrderDetails.Add(orderDetails);
            }

            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<bool> UpdateOrderHeaderAsync(int id, OrderHeaderUpdateDTO orderHeaderUpdateDTO)
        {
            var orderFromDb = await _db.OrderHeaders.FindAsync(id);

            if (orderFromDb == null) return false;

            orderFromDb.PickupName = orderHeaderUpdateDTO.PickupName ?? orderFromDb.PickupName;
            orderFromDb.PickupPhoneNumber = orderHeaderUpdateDTO.PickupPhoneNumber ?? orderFromDb.PickupPhoneNumber;
            orderFromDb.PickupEmail = orderHeaderUpdateDTO.PickupEmail ?? orderFromDb.PickupEmail;
            orderFromDb.Status = orderHeaderUpdateDTO.Status ?? orderFromDb.Status;
            orderFromDb.StripePaymentIntentID = orderHeaderUpdateDTO.StripePaymentIntentID ?? orderFromDb.StripePaymentIntentID;

            _db.OrderHeaders.Update(orderFromDb);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}

