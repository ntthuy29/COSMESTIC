using COSMESTIC.Models.Data;
using COSMESTIC.Models.Order;
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


        [HttpGet]
        public async Task<IActionResult> IndexAdminOrder(string status, string searchCustomer)
        {
          
            var query = _context.Orders
                .Include(o => o.users)
                .Select(o => new ListOrderModel
                {
                    OrderID = o.orderID,
                    UserID = o.userID,
                    CustomerName = o.users.fullName, 
                    TotalAmount = o.totalAmount,
                    OrderDate = o.orderDate,
                    EndDate = o.endDate,
                    Status = o.status,
                    DeliveryID = o.DeliveryID
                });

            
            if (!string.IsNullOrEmpty(status) && status != "Tất cả trạng thái")
            {
                query = query.Where(o => o.Status == status);
            }
            
            if (!string.IsNullOrEmpty(searchCustomer))
            {
                query = query.Where(o => o.CustomerName.Contains(searchCustomer));
            }
            var orders = await query.ToListAsync();
            ViewBag.Statuses = new[] {"Chờ xử lý","Đang giao", "Bị Từ chối", "Đã hoàn thành" }; // Danh sách trạng thái
            ViewBag.SelectedStatus = status;
            ViewBag.SearchCustomer = searchCustomer;

            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            order.status = "Đang giao";
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đơn hàng được duyệt thành công";
            return RedirectToAction("IndexAdminOrder");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.status = "Bị từ chối";
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đơn hàng đã bị từ chối";

            return RedirectToAction ("IndexAdminOrder");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.status != "Rejected")
            {
                TempData["Error"] = "Đơn hàng ở trạng thái không cho phép xóa";
                return RedirectToAction(nameof(Index));
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> BulkApprove(int[] selectedOrders)
        {
            if (selectedOrders == null || !selectedOrders.Any())
            {
               TempData["SuccessMessage"] = "Không có đơn hàng nào dc chọn";
                return RedirectToAction(nameof(Index));
            }
            int count = 0;
            foreach (var id in selectedOrders)
            {
                var article = _context.Orders.Find(id);
                if (article != null && article.status == "Chờ xử lý")
                {
                    article.status = "Đang giao";
                    count++;
                }
            }
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{count} đơn hàng đã được duyệt thành công";
            return RedirectToAction("IndexAdminOrder");
        }
    }
}
