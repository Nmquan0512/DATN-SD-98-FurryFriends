using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VouchersController : Controller
    {
        private readonly IVoucherService _voucherService;

        public VouchersController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        // GET: Admin/Voucher
        public async Task<IActionResult> Index()
        {
            var vouchers = await _voucherService.GetAllAsync();
            return View(vouchers);
        }

        // GET: Admin/Voucher/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // GET: Admin/Voucher/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Voucher/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Voucher voucher)
        {
            if (!ModelState.IsValid)
                return View(voucher);

            voucher.NgayTao = DateTime.Now;
            await _voucherService.CreateAsync(voucher);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Voucher/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Admin/Voucher/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Voucher voucher)
        {
            if (!ModelState.IsValid)
                return View(voucher);

            voucher.NgayCapNhat = DateTime.Now;
            var updated = await _voucherService.UpdateAsync(id, voucher);
            if (updated == null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Voucher/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var voucher = await _voucherService.GetByIdAsync(id);
            if (voucher == null)
                return NotFound();

            return View(voucher);
        }

        // POST: Admin/Voucher/DeleteConfirmed/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _voucherService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
