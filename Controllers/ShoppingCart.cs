using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COSMESTIC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly AppDbContext _context;

        public ShoppingCartController(AppDbContext context)
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

            var cart = _context.ShoppingCart
                               .Include(c => c.cartItems) 
                               .ThenInclude(ci => ci.products) 
                               .FirstOrDefault(c => c.userID == userId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    userID = userId.Value,
                    totalQuantity = 0,
                    totalPrice = 0,
                    cartItems = new List<CartItem>()
                };
            }

            return View(cart); 
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); 
            }

            var product = _context.Products.Find(productId);

            if (product != null)
            {
                var cart = _context.ShoppingCart
                                   .Include(c => c.cartItems)
                                   .FirstOrDefault(c => c.userID == userId);

                // Nếu không có giỏ hàng, tạo mới giỏ hàng
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        userID = userId.Value,
                        totalQuantity = 0,
                        totalPrice = 0,
                        cartItems = new List<CartItem>()
                    };
                    _context.ShoppingCart.Add(cart);
                    _context.SaveChanges();  // Lưu giỏ hàng mới vào cơ sở dữ liệu
                }

                // Kiểm tra nếu sản phẩm đã có trong giỏ hàng
                var existingCartItem = cart.cartItems
                                            .FirstOrDefault(ci => ci.productID == productId);

                if (existingCartItem != null)
                {
                    // Nếu sản phẩm đã có trong giỏ, tăng số lượng lên 1
                    existingCartItem.quantity += 1;
                }
                else
                {
                    // Nếu chưa có trong giỏ, thêm sản phẩm mới vào giỏ hàng
                    var cartItem = new CartItem
                    {
                        productID = productId,
                        quantity = 1,
                        unitprice = product.price
                    };
                    cart.cartItems.Add(cartItem); // Thêm sản phẩm vào giỏ hàng
                }

                // Cập nhật tổng số lượng và tổng giá trị giỏ hàng
                cart.totalQuantity = cart.cartItems.Sum(ci => ci.quantity);
                cart.totalPrice = cart.cartItems.Sum(ci => ci.quantity * ci.unitprice);

                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            // Chuyển hướng lại đến trang giỏ hàng
            return RedirectToAction("Index", "ShoppingCart");
        }

    }
}
