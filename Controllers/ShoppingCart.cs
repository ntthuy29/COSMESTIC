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
                return RedirectToAction("Login", "Login"); 
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
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var product = _context.Products.Find(productId);
            
            if (product != null)
            {
                var cart = _context.ShoppingCart
                                   .Include(c => c.cartItems)
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
                    _context.ShoppingCart.Add(cart);
                    _context.SaveChanges();
                }

                var existingCartItem = cart.cartItems
                                            .FirstOrDefault(ci => ci.productID == productId);

                if (existingCartItem != null)
                {
                    existingCartItem.quantity += quantity;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        productID = productId,
                        quantity = quantity,
                        unitprice = product.price
                    };
                    cart.cartItems.Add(cartItem);
                }

                cart.totalQuantity = cart.cartItems.Sum(ci => ci.quantity);
                cart.totalPrice = cart.cartItems.Sum(ci => ci.quantity * ci.unitprice);
                ViewBag.TotalQuantity = cart.totalQuantity;
                HttpContext.Session.SetInt32("CartItemCount", cart.totalQuantity);
                ViewBag.TotalPrice = cart.totalPrice;

                _context.SaveChanges();
            }

            return RedirectToAction("Product", "Product");

        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemID, string action)
        {
            var cartItem = _context.CartItem.Find(cartItemID);
            if (cartItem != null)
            {
                if (action == "increase")
                {
                    cartItem.quantity += 1;
                }
                else if (action == "decrease" && cartItem.quantity > 1)
                {
                    cartItem.quantity -= 1;
                }
                _context.SaveChanges();
                // Tính lại tổng tiền và tổng số lượng
                var cart = _context.ShoppingCart
                                   .Include(c => c.cartItems)
                                   .ThenInclude(ci => ci.products)
                                   .FirstOrDefault(c => c.userID == HttpContext.Session.GetInt32("UserID"));

                if (cart != null)
                {
                    cart.totalQuantity = cart.cartItems.Sum(ci => ci.quantity);
                    cart.totalPrice = cart.cartItems.Sum(ci => ci.quantity * ci.unitprice);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index", "ShoppingCart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemID)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            var cartItem = _context.CartItem.Find(cartItemID);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                _context.SaveChanges();
            }
            var cart = _context.ShoppingCart.Include(c => c.cartItems)
                                                .ThenInclude(ci => ci.products)
                                                .FirstOrDefault(c => c.userID == userID);
            if (cart != null)
            {
                cart.totalQuantity = cart.cartItems.Sum(ci => ci.quantity);
                cart.totalPrice = cart.cartItems.Sum(ci => ci.quantity * ci.unitprice);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "ShoppingCart");
        }
        [HttpPost]
        public IActionResult RemoveSelectedFromCart(List<int> selectedItems)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            foreach (var itemId in selectedItems)
            {
                var item = _context.CartItem.FirstOrDefault(c => c.cartItemID == itemId);
                if (item != null)
                {
                    _context.CartItem.Remove(item);
                }
            }
            var cart = _context.ShoppingCart.Include(c => c.cartItems)
                                                .ThenInclude(ci => ci.products)
                                                .FirstOrDefault(c => c.userID == userID);
            if (cart != null)
            {
                cart.totalQuantity = cart.cartItems.Sum(ci => ci.quantity);
                cart.totalPrice = cart.cartItems.Sum(ci => ci.quantity * ci.unitprice);
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "ShoppingCart");
        }

    }
}
