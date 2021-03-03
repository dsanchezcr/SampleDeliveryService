using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleDeliveryService.Models;
using Microsoft.AspNetCore.Http;
using SampleDeliveryService.Services;

namespace SampleDeliveryService.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TokenAuthorizationProvider provider;
        private readonly ICosmosDbService _cosmosDbService;

        public OrdersController(TokenAuthorizationProvider provider, ICosmosDbService cosmosDbService)
        {
            this.provider = provider;
            _cosmosDbService = cosmosDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var option = new CookieOptions();
            var token = provider.CreateToken();
            option.Expires = token.ValidTo;
            Response.Cookies.Append("Authorization", token.RawData, option);
            return View(await _cosmosDbService.GetItemsAsync("SELECT * FROM c WHERE c.isComplete = false"));
        }

        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            return View(await _cosmosDbService.GetItemsAsync("SELECT * FROM c"));
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,FirstName,LastName,Packages,Street,City,State,ZipCode,Latitude,Longitude,Completed")] Order order)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.AddItemAsync(order);
                return RedirectToAction("List");
            }

            return View(order);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,FirstName,LastName,Packages,Street,City,State,ZipCode,Latitude,Longitude,Completed")] Order order)
        {
            if (ModelState.IsValid)
            {
                await _cosmosDbService.UpdateItemAsync(order);
                return RedirectToAction("List");
            }

            return View(order);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Order order = await _cosmosDbService.GetItemAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Order order = await _cosmosDbService.GetItemAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await _cosmosDbService.DeleteItemAsync(id);
            return RedirectToAction("List");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            return View(await _cosmosDbService.GetItemAsync(id));
        }

        [ActionName("Route")]
        public async Task<ActionResult> RouteAsync(string id)
        {
            return View(await _cosmosDbService.GetItemAsync(id));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
