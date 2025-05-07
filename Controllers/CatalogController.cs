using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext db;
        public CatalogController(AppDbContext _db)
        {
            db = _db;
        }
    }
}
