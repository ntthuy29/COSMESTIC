using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace COSMESTIC.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = _context.Orders
                .Include(o => o.orderDetails)
                .ThenInclude(oi => oi.products)
                .Where(o => o.userID == userId)
                .ToList();
            return View(orders);
        }
        public IActionResult ShippingInformation()
        {
            return View();
        }
        [HttpPost]
        [HttpPost]
        public IActionResult CreateOrder(string fullName, string address, string phoneNumber)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var delivery = new DeliveryIFMT
            {
                deliveryName = fullName,
                deliveryAddress = address,
                deliveryPhone = phoneNumber,
                userID = userId.Value
            };
            _context.DeliveryIFMT.Add(delivery);
            _context.SaveChanges();

            var newOrder = new Orders
            {
                userID = userId.Value,
                orderDate = DateTime.Now,
                status = "Đang chờ duyệt",
                totalAmount = 0, 
                DeliveryID = delivery.deliveryID 
            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges(); 

            var cart = _context.ShoppingCart
                .Include(c => c.cartItems)
                .FirstOrDefault(c => c.userID == userId);
            if (cart != null && cart.cartItems.Any())
            {
                foreach (var item in cart.cartItems)
                {
                    var orderDetail = new orderDetail
                    {
                        orderID = newOrder.orderID,
                        productID = item.productID,
                        quantity = item.quantity,
                        unitPrice = item.unitprice
                    };
                    newOrder.totalAmount += item.quantity * item.unitprice;

                    _context.orderDetails.Add(orderDetail); 
                }
                _context.SaveChanges();
            }

            _context.CartItem.RemoveRange(cart.cartItems);
            _context.SaveChanges();

            return RedirectToAction("OrderDetails", new { orderId = newOrder.orderID });
        }

        public IActionResult OrderDetails(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.orderDetails)
                .ThenInclude(oi => oi.products)
                .Include(o => o.Delivery)
                .FirstOrDefault(o => o.orderID == orderId);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
