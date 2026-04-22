// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using System.Linq;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestAppService _serviceRequestService;
        private readonly IContractAppService _contractService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestAppService serviceRequestService,
            IContractAppService contractService,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _contractService = contractService;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _serviceRequestService.GetAllAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _serviceRequestService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            await LoadContracts();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest model, decimal usdAmount = 0)
        {
            if (usdAmount > 0)
            {
                try
                {
                    model.Cost = await _currencyService.ConvertUsdToZarAsync(usdAmount);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Currency conversion failed. Please try again later.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(model);
            }

            var created = await _serviceRequestService.AddAsync(model);
            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Service Request cannot be created because the selected contract is Draft, Expired, or On Hold.");
                await LoadContracts();
                return View(model);
            }

            TempData["SuccessMessage"] = "Service Request created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _serviceRequestService.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadContracts();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadContracts();
                return View(model);
            }

            var updated = await _serviceRequestService.UpdateAsync(model);
            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "Service Request cannot be updated because the selected contract is Draft, Expired, or On Hold.");
                await LoadContracts();
                return View(model);
            }

            TempData["SuccessMessage"] = "Service Request updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _serviceRequestService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRequestService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Service Request deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetZarAmount(decimal usdAmount)
        {
            try
            {
                var zarAmount = await _currencyService.ConvertUsdToZarAsync(usdAmount);
                return Json(new { success = true, zarAmount = zarAmount });
            }
            catch
            {
                return Json(new { success = false, message = "Currency conversion failed." });
            }
        }

        private async Task LoadContracts()
        {
            var contracts = await _contractService.GetAllAsync();

            ViewBag.Contracts = contracts.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"Contract #{c.Id} - {c.Client?.Name} - {c.ServiceLevel} - {c.Status}"
            }).ToList();
        }
    }
}