using FurryFriends.API.Models;
using FurryFriends.Web.Filter;
using FurryFriends.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FurryFriends.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AuthorizeAdminOnly]
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
            ViewBag.TotalCount = list.Count();
            ViewBag.ActiveCount = list.Count(x => x.TrangThai);
            ViewBag.InactiveCount = list.Count(x => !x.TrangThai);
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

            try
            {
                await _chucVuService.AddAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                // Parse JSON lỗi (ValidationProblemDetails) từ API
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gửi dữ liệu: " + ex.Message);
                return View(model);
            }
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

            try
            {
                await _chucVuService.UpdateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(ex.Message);

                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var msg in error.Value)
                        {
                            ModelState.AddModelError(error.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Lỗi xác thực.");
                }

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi gửi dữ liệu: " + ex.Message);
                return View(model);
            }
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