﻿using COSMESTIC.Models.Data;
using COSMESTIC.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Text.Json;
namespace COSMESTIC.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Customer")]
        public IActionResult Index(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var order = _context.Orders
                                 .Include(o => o.orderDetails)
                                 .ThenInclude(od => od.products)
                                 .Include(o => o.Delivery)
                                 .FirstOrDefault(o => o.orderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);

        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ShippingInformation(string selectedItems, int? savedAddress = null)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var deliveryAddresses = await _context.DeliveryIFMT
                .Where(d => d.userID == userId).ToListAsync();

            if (string.IsNullOrEmpty(selectedItems))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn sản phẩm để đặt hàng.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var selectedItemsList = selectedItems.Split(',').Select(int.Parse).ToList();

            // ✅ Bảo vệ: Chỉ lấy cart items của chính người đang đăng nhập
            var selectedCartItems = await _context.CartItem
                .Where(c => selectedItemsList.Contains(c.cartItemID) && c.cart.userID == userId)
                .Include(c => c.products)
                .ToListAsync();

            // Nếu có item không thuộc user → chặn lại
            if (selectedCartItems.Count != selectedItemsList.Count)
            {
                TempData["ErrorMessageNotAllow"] = "Phát hiện truy cập trái phép ";
                return RedirectToAction("Product", "Product");
            }

            ViewBag.SelectedCartItems = selectedCartItems;
            ViewBag.savedAddress = savedAddress;
  
            return View(deliveryAddresses);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(List<int> selectedItems, string fullName, string address, string phoneNumber, string discountCode, int? savedAddress)
        {
            //Console.WriteLine("HUHU");

            //Console.WriteLine(fullName);
            //Console.WriteLine(address);
            //Console.WriteLine(phoneNumber);
            //Console.WriteLine(discountCode);
            //Console.WriteLine("haha");
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Login");
            }

            // Lọc các sản phẩm trong giỏ hàng dựa trên selectedItemsList
            var selectedCartItems = await _context.CartItem
                                                   .Where(c => selectedItems.Contains(c.cartItemID))
                                                   .Include(c => c.products)
                                                   .ToListAsync();

            if (!selectedCartItems.Any())
            {
                Console.WriteLine("Hăm có sản p duoc chọn");
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm được chọn trong giỏ hàng.";
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Tính tổng giá trị các sản phẩm đã chọn
            decimal totalAmount = selectedCartItems.Sum(item => item.quantity * item.unitprice);
            decimal totalDiscountAmount = 0;

            // Xử lý mã giảm giá
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = await _context.Discount
                    .FirstOrDefaultAsync(d => d.isActive && d.discountType == discountCode && d.startDate <= DateTime.Now && d.endDate >= DateTime.Now);
                if (discount != null)
                {
                    if (totalAmount >= discount.discountAmount)
                    {
                        totalDiscountAmount = totalAmount * (discount.value / 100);
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

            decimal finalTotal = totalAmount - totalDiscountAmount;

            // Kiểm tra savedAddress
            var deliveryAddress = savedAddress.HasValue
                ? await _context.DeliveryIFMT.FirstOrDefaultAsync(d => d.deliveryID == savedAddress.Value && d.userID == userId.Value)
                : null;

            // Truyền dữ liệu vào View
            ViewBag.SelectedItems = selectedCartItems;
            ViewBag.FullName = deliveryAddress?.deliveryName ?? fullName;
            ViewBag.Address = deliveryAddress?.deliveryAddress ?? address;
            ViewBag.PhoneNumber = deliveryAddress?.deliveryPhone ?? phoneNumber;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.DiscountAmount = totalDiscountAmount;
            ViewBag.FinalTotal = finalTotal;
            ViewBag.DiscountCode = discountCode;
            ViewBag.SavedAddress = savedAddress;


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(List<int> selectedItems, string fullName, string address, string phoneNumber, string discountCode, int? savedAddress, string paymentMethod, string? note)
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

            // Lọc các sản phẩm trong giỏ hàng dựa trên selectedItemsList
            var selectedCartItems = await _context.CartItem
                                                   .Where(c => selectedItems.Contains(c.cartItemID))
                                                   .Include(c => c.products)
                                                   .ToListAsync();

            if (!selectedCartItems.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm được chọn trong giỏ hàng.";
                return RedirectToAction("Index", "ShoppingCart");
            }
            // Tính tổng giá trị các sản phẩm đã chọn
            decimal totalAmount = selectedCartItems.Sum(item => item.quantity * item.unitprice);
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
            // Nếu người dùng không chọn địa chỉ đã lưu, tạo mới địa chỉ
            if (savedAddress == null)
            {
                var delivery = new DeliveryIFMT
                {
                    deliveryName = HttpUtility.HtmlDecode(fullName),
                    deliveryAddress = HttpUtility.HtmlDecode(address),
                    deliveryPhone = phoneNumber,
                    userID = userId.Value
                };
                _context.DeliveryIFMT.Add(delivery);
                _context.SaveChanges();
            }
            totalAmount -= TotaldiscountAmount; // Áp dụng giảm giá ngay tại đây
            var newOrder = new Orders
            {
                userID = userId.Value,
                orderDate = DateTime.Now,

                status = "Chờ xử lý",  // Trạng thái đang chờ duyệt
                totalAmount = totalAmount,
                payMethod = paymentMethod,
                note = note,
                DeliveryID = savedAddress ?? _context.DeliveryIFMT
                                                    .Where(d => d.userID == userId)
                                                    .OrderBy(d => d.deliveryID)
                                                    .Select(d => d.deliveryID)
                                                    .LastOrDefault()

            };
            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            foreach (var item in selectedCartItems)
            {
                var orderDetail = new orderDetail
                {
                    orderID = newOrder.orderID,
                    productID = item.productID,
                    quantity = item.quantity,
                    unitPrice = item.unitprice
                };
                _context.orderDetails.Add(orderDetail);
            }

            _context.CartItem.RemoveRange(_context.CartItem
                                                   .Where(c => selectedItems.Contains(c.cartItemID)));
            _context.SaveChanges();

            ViewBag.SelectedItems = selectedCartItems;
            if (paymentMethod == "momo" || paymentMethod == "bankAccount")
            {
                var invoice = new Invoice
                {
                    orderID = newOrder.orderID,
                };
                _context.Invoice.Add(invoice);
                _context.SaveChanges();
                return RedirectToAction("InvoiceDetails", new { orderId = newOrder.orderID });
            }

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
        public IActionResult InvoiceDetails(int orderId)
        {
            var invoice = _context.Invoice
                .Include(o => o.orders)
                    .ThenInclude(od => od.orderDetails)
                    .ThenInclude(od => od.products)
                .Include(o => o.orders.Delivery)
                .FirstOrDefault(o => o.orderID == orderId);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }
        public IActionResult MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
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
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var order = _context.Orders
                                .Include(o => o.orderDetails)
                                .FirstOrDefault(o => o.orderID == id && o.userID == userId);
            if (order == null)
            {
                return NotFound();
            }
            order.endDate = DateTime.Now;
            _context.SaveChanges();
            if (order.status == "Chờ xử lý")
            {
                order.status = "Đã hủy";
                _context.SaveChanges();
                //Hoàn sản phẩm vào kho
                foreach (var item in order.orderDetails)
                {
                    var product = _context.Products.Find(item.productID);
                    if (product != null)
                    {
                        product.quantity += item.quantity;
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("MyOrders");
        }
        [HttpPost]
        public IActionResult MarkAsDelivered(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            if (order.status == "Đang giao")
            {
                order.status = "Đã hoàn thành";
                order.endDate = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("MyOrders", new { id = id });
        }
        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> ShippingInformationBuyNow(int productId, int quantity, string discountCode = null, string address = null, string fullName = null, string phoneNumber = null, int? savedAddress = null)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var deliveryAddresses = await _context.DeliveryIFMT
                .Where(d => d.userID == userId)
                .ToListAsync();

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return NotFound();
            }


            decimal totalAmount = product.price * quantity;
            decimal totalDiscountAmount = 0;

            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = await _context.Discount.FirstOrDefaultAsync(d => d.isActive == true &&
                    d.discountType == discountCode &&
                    d.startDate <= DateTime.Now &&
                    d.endDate >= DateTime.Now);

                if (discount != null)
                {
                    if (totalAmount >= discount.discountAmount)
                    {
                        totalDiscountAmount = totalAmount * (discount.value / 100);
                        TempData["SuccessMessage"] = "Mã giảm giá đã được áp dụng!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Đơn hàng của bạn chưa đủ điều kiện (yêu cầu tối thiểu {discount.discountAmount:N0}đ).";
                        discountCode = null;
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ!";
                    discountCode = null;
                }
            }
            ViewBag.productImg = product.imagePath;
            ViewBag.savedAddress = savedAddress;
            ViewBag.Product = product;
            ViewBag.Quantity = quantity;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.DiscountAmount = totalDiscountAmount;
            ViewBag.FinalTotal = totalAmount - totalDiscountAmount;
            ViewBag.DiscountCode = discountCode;

            //ViewBag là 1 đối tượng động dùng để truyền dữ liệu từ controller sang View 1 cách động

            return View(deliveryAddresses);
        }
        [HttpPost]
        public IActionResult CreateOrderBuyNow(string fullName, string address, string phoneNumber, int productId, int quantity, string discountCode, int? savedAddress, string paymentMethod, string? note)
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

            // Tính tổng tiền trước khi áp dụng giảm giá
            decimal totalAmount = product.price * quantity;
            decimal totalDiscountAmount = 0;

            // Kiểm tra và áp dụng mã giảm giá nếu có
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = _context.Discount.FirstOrDefault(d => d.isActive == true &&
                    d.discountType == discountCode &&
                    d.startDate <= DateTime.Now &&
                    d.endDate >= DateTime.Now);

                if (discount != null)
                {
                    if (totalAmount >= discount.discountAmount)
                    {
                        totalDiscountAmount = totalAmount * (discount.value / 100);
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

            // Nếu người dùng không chọn địa chỉ đã lưu, tạo mới địa chỉ
            if (savedAddress == null)
            {
                var delivery = new DeliveryIFMT
                {
                    deliveryName = HttpUtility.HtmlDecode(fullName),
                    deliveryAddress = HttpUtility.HtmlDecode(address),
                    deliveryPhone = phoneNumber,
                    userID = userId.Value
                };
                _context.DeliveryIFMT.Add(delivery);
                _context.SaveChanges();
            }

            // Tạo đơn hàng mới
            var newOrder = new Orders
            {
                userID = userId.Value,
                orderDate = DateTime.Now,

                status = "Chờ xử lý",


                totalAmount = totalAmount - totalDiscountAmount, // Áp dụng giảm giá ngay tại đây
                payMethod = paymentMethod,
                note = note,
                DeliveryID = savedAddress ?? _context.DeliveryIFMT
                                                    .Where(d => d.userID == userId)
                                                    .OrderBy(d => d.deliveryID)
                                                    .Select(d => d.deliveryID)
                                                    .LastOrDefault()

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
            _context.orderDetails.Add(orderDetail);

          
            if (product != null)
            {
                product.quantity -= quantity;
                if (product.quantity < 0)
                {
                    product.quantity = 0;
                }
            }
            if (paymentMethod == "momo" || paymentMethod == "bankAccount")
            {
                var invoice = new Invoice
                {
                    orderID = newOrder.orderID,
                };
                _context.Invoice.Add(invoice);
                _context.SaveChanges();
                return RedirectToAction("InvoiceDetails", new { orderId = newOrder.orderID });
            }
            _context.SaveChanges();
            return RedirectToAction("OrderSuccess", new { orderId = newOrder.orderID });
        }

        //dưới đây là code của admin
        [HttpGet]
        [Authorize(Roles = "admin,sale")]
        public async Task<IActionResult> IndexAdminOrder(string status, string searchCustomer)
        {
            if (string.IsNullOrEmpty(status))
            {
                status = "Chờ xử lý";
            }

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
            ViewBag.Statuses = new[] { "Chờ xử lý", "Đang giao", "Bị từ chối", "Đã hoàn thành", "Đã hủy" }; // Danh sách trạng thái
            ViewBag.SelectedStatus = status;
            ViewBag.SearchCustomer = searchCustomer;

            return View(orders);
        }
        [HttpPost]
        [Authorize(Roles = "admin,sale")]
        public async Task<IActionResult> Approve(int id)
        {
            // Tìm đơn hàng và kèm theo OrderDetails
            var order = await _context.Orders
                .Include(o => o.orderDetails)
                .FirstOrDefaultAsync(o => o.orderID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy ID người bán từ Session (giả sử là người duyệt)
            var sellerID = HttpContext.Session.GetInt32("UserID");
            if (sellerID == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để duyệt đơn.";
                return RedirectToAction("Login", "Account");
            }

          
            order.status = "Đang giao";

          
            foreach (var item in order.orderDetails)
            {
                item.SellerID = sellerID.Value; 
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn hàng được duyệt thành công";
            return RedirectToAction("IndexAdminOrder");
        }


        [HttpPost]
        [Authorize(Roles = "admin,sale")]
        public async Task<IActionResult> Reject(int id)
        {
            // Lấy đơn hàng kèm theo danh sách sản phẩm chi tiết
            var order = await _context.Orders
                .Include(o => o.orderDetails) // Load các chi tiết đơn hàng
                .ThenInclude(od => od.products) // Load thông tin sản phẩm
                .FirstOrDefaultAsync(o => o.orderID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Duyệt qua từng sản phẩm trong đơn hàng và hoàn lại số lượng
            foreach (var orderDetail in order.orderDetails)
            {
                var product = orderDetail.products;
                product.quantity += orderDetail.quantity; // Hoàn lại số lượng đã trừ
                _context.Products.Update(product);
            }

            // Cập nhật trạng thái đơn hàng
            order.status = "Bị từ chối";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn hàng đã bị từ chối và số lượng sản phẩm đã được hoàn lại";

            return RedirectToAction("IndexAdminOrder");
        }

        [HttpPost]
        [Authorize(Roles = "admin,sale")]
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
        [Authorize(Roles = "admin,sale")]
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
