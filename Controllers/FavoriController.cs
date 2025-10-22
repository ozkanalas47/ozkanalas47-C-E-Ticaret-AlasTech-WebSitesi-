using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AlasTech.Controllers
{
    public class FavoriController : Controller
    {
        private readonly AlasTechContext _context;

        public FavoriController(AlasTechContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Kullanıcının favorilerini listele
        public async Task<IActionResult> Index()
        {
            try
            {
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
                if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
                {
                    return RedirectToAction("Giris", "Hesap");
                }

                var favoriler = await _context.Favoriler
                    .Where(f => f.KullaniciId == kullaniciId.Value)
                    .Include(f => f.Urun)
                    .ThenInclude(u => u.Kategori)
                    .Include(f => f.Urun)
                    .ThenInclude(u => u.SaticiProfili)
                    .Select(f => new UrunGorunumModeli
                    {
                        Id = f.Urun != null ? f.Urun.Id : 0,
                        Ad = f.Urun != null ? f.Urun.Ad : "Bilinmeyen Ürün",
                        Fiyat = f.Urun != null ? f.Urun.Fiyat : 0,
                        Aciklama = f.Urun != null ? f.Urun.Aciklama : string.Empty,
                        ResimUrl = f.Urun != null ? f.Urun.ResimUrl : "/resimler/varsayilan.jpg",
                        Stok = f.Urun != null ? f.Urun.Stok : 0,
                        KategoriAdi = (f.Urun != null && f.Urun.Kategori != null) ? f.Urun.Kategori.Adi : "Bilinmiyor",
                        MagazaAdi = (f.Urun != null && f.Urun.SaticiProfili != null) ? f.Urun.SaticiProfili.MagazaAdi : "Bilinmiyor"
                    })
                    .ToListAsync();

                return View(favoriler);
            }
            catch (Exception ex)
            {
                TempData["Hata"] = $"Favoriler yüklenirken bir hata oluştu: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}