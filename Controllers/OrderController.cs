using COSMESTIC.Models.Data;
using COSMESTIC.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
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
                return RedirectToAction("Login", "Login");
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
        public IActionResult ConfirmOrder(string fullName, string address, string phoneNumber, string discountCode, bool applyDiscount)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cart = _context.ShoppingCart
                .Include(c => c.cartItems)
                .ThenInclude(ci => ci.products)
                .FirstOrDefault(c => c.userID == userId);

            if (cart == null || cart.cartItems.Count == 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            decimal TotaldiscountAmount = 0;
            if (applyDiscount && !string.IsNullOrEmpty(discountCode))
            {
                var discount = _context.Discount.FirstOrDefault(d => d.isActive == true && d.discountType == discountCode && d.startDate <= DateTime.Now && d.endDate >= DateTime.Now);
                if (discount != null)
                {
                    if (cart.totalPrice >= discount.discountAmount)
                    {
                        TotaldiscountAmount = cart.totalPrice * (discount.value / 100);
                        TempData["SuccessMessage"] = "Mã giảm giá đã được áp dụng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Đơn hàng của bạn chưa đủ điều kiện để áp dụng mã giảm giá này.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ!";
                }
            }
            cart.totalPrice -= TotaldiscountAmount;

            ViewBag.Cart = cart;
            ViewBag.FullName = fullName;
            ViewBag.Address = address;
            ViewBag.PhoneNumber = phoneNumber;

            return View();
        }
        [HttpPost]
        public IActionResult CreateOrder(string fullName, string address, string phoneNumber, string  discountCode)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var cart = _context.ShoppingCart
                .Include(c => c.cartItems)
                .FirstOrDefault(c => c.userID == userId);
            if (cart == null || cart.cartItems.Count == 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }
            decimal TotaldiscountAmount = 0;
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = _context.Discount.FirstOrDefault(d => d.isActive == true && d.discountType == discountCode && d.startDate <= DateTime.Now && d.endDate >= DateTime.Now);
                if (discount != null)
                {
                    if (cart.totalPrice >= discount.discountAmount)
                    {
                        TotaldiscountAmount = cart.totalPrice * (discount.value / 100);
                        TempData["SuccessMessage"] = "Mã giảm giá đã được áp dụng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Đơn hàng của bạn chưa đủ điều kiện để áp dụng mã giảm giá này.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ!";
                }
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
                status = "Chờ xử lý",
                totalAmount = 0, 
                DeliveryID = delivery.deliveryID 
            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges(); 

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

                var product = _context.Products.Find(item.productID);
                if(product != null)
                {
                    product.quantity -= item.quantity;
                    if(product.quantity < 0)
                    {
                        product.quantity = 0;
                    }
                }
            }
            newOrder.totalAmount -= TotaldiscountAmount;
            _context.SaveChanges();

            _context.CartItem.RemoveRange(cart.cartItems);
            _context.SaveChanges();

            return RedirectToAction("OrderSuccess", new { orderId = newOrder.orderID });
        }
        public IActionResult OrderSuccess(int orderId)
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
        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if(userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var orders = _context.Orders
                                 .Where(o => o.userID == userId)
                                 .Include(o => o.orderDetails)
                                 .ThenInclude(od => od.products)
                                 .OrderByDescending(o => o.orderDate)
                                 .ToList();
            return View(orders);
        }
        public IActionResult Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var order = _context.Orders
                                .Include(o => o.orderDetails)
                                .ThenInclude(od => od.products)
                                .Include(o => o.Delivery)
                                .FirstOrDefault(o => o.orderID == id && o.userID == userId);
            if(order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if(userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var order = _context.Orders
                                .Include(o => o.orderDetails)
                                .FirstOrDefault(o => o.orderID == id && o.userID == userId);
            if(order == null)
            {
                return NotFound();
            }
            order.endDate = DateTime.Now;
            _context.SaveChanges();
            if (order.status == "Chờ xử lý")
            {
                order.status = "Bị từ chối";
                _context.SaveChanges();
                //Hoàn sản phẩm vào kho
                foreach (var item in order.orderDetails)
                {
                    var product = _context.Products.Find(item.productID);
                    if(product != null)
                    {
                        product.quantity += item.quantity;
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("MyOrders");
        }
        [HttpPost]
        public IActionResult ShippingInformationBuyNow(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return NotFound();
            }
            // Truyền thông tin sản phẩm và số lượng đến View
            ViewBag.Product = product;
            ViewBag.Quantity = quantity;

            return View();  
        }
        [HttpPost]
        public IActionResult CreateOrderBuyNow(string fullName, string address, string phoneNumber, int productId, int quantity, string discountCode, bool applyDiscount)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }
            decimal TotaldiscountAmount = 0;
            if(applyDiscount && !string.IsNullOrEmpty(discountCode))
            {
                var discount = _context.Discount.FirstOrDefault(d => d.isActive == true && d.discountType == discountCode && d.startDate <= DateTime.Now && d.endDate >= DateTime.Now);
                if (discount != null)
                {
                    if (product.price * quantity >= discount.discountAmount)
                    {
                        TotaldiscountAmount = product.price * quantity * (discount.value / 100);
                        TempData["SuccessMessage"] = "Mã giảm giá đã được áp dụng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Đơn hàng của bạn chưa đủ điều kiện để áp dụng mã giảm giá này.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ!";
                }
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
                status = "Chờ xử lý",  // Trạng thái đang chờ duyệt
                totalAmount = 0,
                DeliveryID = delivery.deliveryID
            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            var orderDetail = new orderDetail
            {
                orderID = newOrder.orderID,
                productID = productId,
                quantity = quantity,
                unitPrice = product.price
            };
            newOrder.totalAmount = quantity * product.price - TotaldiscountAmount;
            _context.orderDetails.Add(orderDetail);

            // Trừ sản phẩm khỏi kho
            if (product != null)
            {
                product.quantity -= quantity;
                if (product.quantity < 0)
                {
                    product.quantity = 0;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("OrderSuccess", new { orderId = newOrder.orderID });
        }




      
        
        
        
        
        
        
        //dưới đây là code của admin



        [HttpGet]
        [Authorize(Roles = "admin")]
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
            ViewBag.Statuses = new[] { "Chờ xử lý", "Đang giao", "Bị Từ chối", "Đã hoàn thành" }; // Danh sách trạng thái
            ViewBag.SelectedStatus = status;
            ViewBag.SearchCustomer = searchCustomer;

            return View(orders);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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

            return RedirectToAction("IndexAdminOrder");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
