// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractAppService _contractService;
        private readonly IClientAppService _clientService;
        private readonly IFileService _fileService;
        private readonly string _rootPath;

        public ContractsController(IContractAppService contractService, IClientAppService clientService, IFileService fileService, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _contractService = contractService;
            _clientService = clientService;
            _fileService = fileService;
            _rootPath = env.WebRootPath;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(string status, DateTime? startDate, DateTime? endDate)
        {
            var list = await _contractService.FilterAsync(status, startDate, endDate);
            ViewBag.Status = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(list);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = await _clientService.GetAllAsync();
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile signedAgreement)
        {
            if (signedAgreement != null)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("SignedAgreementPath", "Only PDF files are allowed.");
                }
                else
                {
                    var saved = await _fileService.SaveFileAsync(signedAgreement, _rootPath);
                    contract.SignedAgreementPath = saved;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = await _clientService.GetAllAsync();
                return View(contract);
            }

            await _contractService.AddAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            ViewBag.Clients = await _clientService.GetAllAsync();
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.Id) return BadRequest();

            if (signedAgreement != null)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("SignedAgreementPath", "Only PDF files are allowed.");
                }
                else
                {
                    var saved = await _fileService.SaveFileAsync(signedAgreement, _rootPath);
                    contract.SignedAgreementPath = saved;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = await _clientService.GetAllAsync();
                return View(contract);
            }

            await _contractService.UpdateAsync(contract);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contractService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = System.IO.Path.Combine(_rootPath, contract.SignedAgreementPath.TrimStart('/','\\'));
            var name = System.IO.Path.GetFileName(filePath);
            var mime = "application/pdf";
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, mime, name);
        }
    }
}
