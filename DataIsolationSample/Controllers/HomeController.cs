using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Finbuckle.MultiTenant;
using DataIsolationSample.Data;
using DataIsolationSample.Models;

namespace DataIsolationSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoDbContext _dbContext;

        public HomeController(ToDoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Get the list of to do items. This will only return items for the current tenant.
            IEnumerable<ToDoItem> toDoItems = null;
            if (HttpContext.GetMultiTenantContext<TenantInfo>()?.TenantInfo != null)
            {
                toDoItems = _dbContext.ToDoItems.ToList();
            }

            return View(toDoItems);
        }
    }
}
