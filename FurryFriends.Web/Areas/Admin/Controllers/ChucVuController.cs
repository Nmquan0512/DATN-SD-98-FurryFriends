using FurryFriends.API.Models;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
//using FurryFriends.Web.Filter;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[AuthorizeAdminOnly]
    public class ChucVuController : Controller
    {
        private readonly IChucVuService _chucVuService;

        public ChucVuController(IChucVuService chucVuService)
        {
            _chucVuService = chucVuService;
        }

        // GET: Admin/ChucVu
        public async Task<IActionResult> Index()
        {
            var list = await _chucVuService.GetAllAsync();
            return View(list);
        }

        // GET: Admin/ChucVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ChucVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChucVu model)
        {
            if (!ModelState.IsValid)
                return View(model);
            await _chucVuService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ChucVu/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var chucVu = await _chucVuService.GetByIdAsync(id);
            if (chucVu == null) return NotFound();
            return View(chucVu);
        }

        // POST: Admin/ChucVu/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ChucVu model)
        {
            if (id != model.ChucVuId) return BadRequest();
            if (!ModelState.IsValid) return View(model);
            await _chucVuService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ChucVu/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var chucVu = await _chucVuService.GetByIdAsync(id);
            if (chucVu == null) return NotFound();
            return View(chucVu);
        }

        // POST: Admin/ChucVu/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _chucVuService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
} 