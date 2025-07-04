﻿using COSMESTIC.Models;
using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using COSMESTIC.Models.User;
using COSMESTIC.Models.Order;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;
using BCrypt.Net;
namespace COSMESTIC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int i)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if(user == null)
            {
                return NotFound();
            }

            UpdateCustomerStatus(userId.Value);

            ViewBag.value = i;

            return View(user);
           
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(COSMESTIC.Models.Data.Users updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.fullName = updatedUser.fullName;
            user.email = updatedUser.email;
            user.phoneNumber = updatedUser.phoneNumber;
            user.dateOfBirth = updatedUser.dateOfBirth;
            _context.SaveChanges();
            return RedirectToAction("Detail");
        }
        public IActionResult ChangePassword()
        {
            Console.WriteLine("ChangePassword action called");
            return View();
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
       // thêm ở đầu file

public JsonResult ResetPassword(ChangePasswordViewModel model)
    {
        Console.WriteLine(model.NewPassword);
        Console.WriteLine(model.OldPassword);
        Console.WriteLine(model.ConfirmPassword);
        Console.WriteLine("ChangePassword POST action called");

        var userId = HttpContext.Session.GetInt32("UserID");

        if (userId == null)
        {
            return Json(new { success = false, redirect = Url.Action("Login", "Login") });
        }

        var account = _context.Accounts.Include(c => c.user).FirstOrDefault(a => a.userID == userId);
        if (account == null)
        {
            return Json(new { success = false, message = "Tài khoản không tồn tại." });
        }

        var errors = new Dictionary<string, string>();

        // Kiểm tra mật khẩu cũ có khớp không
        if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, account.password))
        {
            errors["oldPassword"] = "Mật khẩu cũ không đúng.";
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            errors["confirmPassword"] = "Mật khẩu mới không khớp.";
        }

        if (errors.Count > 0)
        {
            return Json(new { success = false, errors });
        }

        // Hash mật khẩu mới trước khi lưu
        account.password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        _context.SaveChanges();

        return Json(new { success = true, message = "Mật khẩu của bạn đã được thay đổi thành công!" });
    }

    public void UpdateCustomerStatus(int userId)
        {
            var user = _context.Users.Include(u => u.orders)
                                     .FirstOrDefault(u => u.userID == userId);
            if (user == null)
            {
                return;
            }
            decimal totalSpent = _context.Orders
                .Where(o => o.userID == userId && o.status == "Shipped")
                .Sum(o => o.totalAmount);
            user.TotalSpent += totalSpent;
            if (totalSpent >= 5000000)
            {
                user.status = "Vàng";
            }
            else if (totalSpent >= 1000000)
            {
                user.status = "Bạc";
            }
            else
            {
                user.status = "Đồng";
            }
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<IActionResult> logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Xóa session


            return RedirectToAction("Product", "Product");
        }


        public async Task<IActionResult> Index(string name, string role, string status)
        {
           
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.fullName.Contains(name));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.role == role);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.status == status);
            }


            var users = await query
                .Select(u => new UserViewModel
                {
                    UserID = u.userID,
                    FullName = u.fullName,
                    Email = u.email,
                    PhoneNumber = u.phoneNumber,
                    DateOfBirth = u.dateOfBirth.ToString("dd/MM/yyyy"),
                    Role = u.role,
                    Status = u.status
                })
                .ToListAsync();

            var roles = await _context.Users
                .Select(u => u.role)
                .Distinct()
                .ToListAsync();

            var statuses = await _context.Users
                .Select(u => u.status)
                .Distinct()
                .ToListAsync();

            var viewModel = new ListUserModel
            {
                Users = users,
                Name = name,
                Roles = roles,
                Statuses = statuses,
                SelectedRole = role,
                SelectedStatus = status
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Where(u => u.userID == id)
                .Select(u => new UserDetailViewModel
                {
                    UserID = u.userID,
                    FullName = u.fullName,
                    DateOfBirth = u.dateOfBirth,
                    Email = u.email,
                    PhoneNumber = u.phoneNumber,
                   
                    Role = u.role,
                    Status = u.status,
                    CreatedDate = u.createdDate, // Giả định có trường createdDate, nếu không có, thêm vào
                    Orders = u.orders.Select(o => new OrderViewModel
                    {
                        OrderId = o.orderID.ToString(),
                        OrderDate = o.orderDate,
                        TotalItems = o.totalItems,
                        TotalAmount = o.totalAmount,
                        Status = o.status
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            user.totalItems = user.Orders.Sum(o => o.TotalItems);
            user.totalAmount = user.Orders.Sum(o => o.TotalAmount);
            return View(user);
        }
    }
}
