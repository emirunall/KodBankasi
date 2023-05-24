using kodbankasi.IServices;
using kodbankasi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace kodbankasi.Controllers
{
    public class KategoriController : Controller
    {
        private readonly IKategoriService _kategoriService;

        public KategoriController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }

        public async Task<IActionResult> Index()
        {

            var data= await _kategoriService.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {

            var model = new Kategori();
            return View(model);
        }

         [HttpPost]
        public async Task<IActionResult> Create(Kategori kategori)
         {

            await _kategoriService.CreateAsync(kategori);
            return RedirectToAction(nameof(Index));
        }
        

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var data = await _kategoriService.GetByIdAsync(id);
            return View(data);

        }

        
         [HttpPost]
         public async Task<IActionResult> Update(Kategori kategori)
         {
            await _kategoriService.UpdateAsync(kategori);
             return RedirectToAction(nameof(Index));
         }
        



        public async Task<IActionResult> Delete(int id)
        {
          var isdeleted = await _kategoriService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
