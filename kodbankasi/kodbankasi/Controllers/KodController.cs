using kodbankasi.IServices;
using kodbankasi.Models;
using kodbankasi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;


namespace kodbankasi.Controllers
{
    public class KodController : Controller
    {
        private readonly IKodService _kodService;
        private readonly IKategoriService _kategoriService;

        public KodController(IKodService kodService, IKategoriService kategoriService)
        {
            _kodService = kodService;
            _kategoriService = kategoriService;
        }

        public async Task<IActionResult> Index()
        {
             var data= await _kodService.GetAllKodWithKategoriNamesAsync();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var kategori = await _kategoriService.GetAllAsync();
            ViewBag.kategoriView = new SelectList(kategori, "id", "name");
            return View();
            
        }

        
         [HttpPost]
         public async Task<IActionResult> Create(Kod kod)
         {
            await _kodService.CreateAsync(kod);
            return RedirectToAction(nameof(Index));
         }
        

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var kategori = await _kategoriService.GetAllAsync();
            ViewBag.kategoriView = new SelectList(kategori, "id", "name");
            var kod = await _kodService.GetByIdAsync(id);
            return View(kod);
        }

        
         [HttpPost]
         public async Task<IActionResult> Update(Kod kod)
         {
            await _kodService.UpdateAsync(kod);
             return RedirectToAction(nameof(Index));
         }
        


        public async Task<IActionResult> Delete(int id)
        {
            await _kodService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var data = await _kodService.GetByIdWithKategoriNamesAsync(id);
            return View(data);
        }

    }
}
