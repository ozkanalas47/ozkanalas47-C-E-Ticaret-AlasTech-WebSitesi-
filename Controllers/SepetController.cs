using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using AlasTech.Models.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlasTech.Controllers
{
    public class SepetController : Controller
    {
        private readonly AlasTechContext _context;

        public SepetController(AlasTechContext context)
        {
            _context = context;
        }

        // Sepet ekranı
        public async Task<IActionResult> Sepet()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var sepetKalemleri = await _context.SepetKalemleri
                .Where(sk => sk.KullaniciId == kullaniciId.Value)
                .Include(sk => sk.Urun)
                .ToListAsync();

            var sepet = sepetKalemleri
                .Where(sk => sk.Urun != null) // Yalnızca geçerli ürünleri al
                .Select(sk => new SepetGorunumModeli
                {
                    Id = sk.Id,
                    UrunId = sk.UrunId,
                    UrunAdi = sk.Urun!.Ad, // Null kontrolü yapıldı, artık güvenli
                    BirimFiyat = sk.Urun.Fiyat,
                    Miktar = sk.Miktar,
                    Toplam = sk.Urun.Fiyat * sk.Miktar,
                    ResimUrl = sk.Urun.ResimUrl ?? ""
                })
                .ToList();

            // Geçersiz ürünleri (Urun null olanlar) ayrı bir şekilde ele al
            if (sepetKalemleri.Any(sk => sk.Urun == null))
            {
                TempData["Uyari"] = "Bazı sepet öğeleri geçersiz ürünler içeriyor ve görüntülenmedi.";
            }

            return View(sepet);
        }

        // Sepete ekle
        [HttpPost]
        public async Task<IActionResult> Ekle(int urunId, int miktar = 1)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var urun = await _context.Urunler.FindAsync(urunId);
            if (urun == null || urun.Stok < miktar)
            {
                TempData["Hata"] = "Ürün bulunamadı veya stok yetersiz.";
                return RedirectToAction("Hata", "Shared");
            }

            var sepetKalemi = await _context.SepetKalemleri
                .FirstOrDefaultAsync(sk => sk.KullaniciId == kullaniciId.Value && sk.UrunId == urunId);

            if (sepetKalemi == null)
            {
                sepetKalemi = new SepetKalemi
                {
                    KullaniciId = kullaniciId.Value,
                    UrunId = urunId,
                    Miktar = miktar
                };
                _context.SepetKalemleri.Add(sepetKalemi);
            }
            else
            {
                sepetKalemi.Miktar += miktar;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Sepet");
        }

        // Sepetten kaldır
        [HttpPost]
        public async Task<IActionResult> Kaldir(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var sepetKalemi = await _context.SepetKalemleri
                .FirstOrDefaultAsync(sk => sk.Id == id && sk.KullaniciId == kullaniciId.Value);

            if (sepetKalemi != null)
            {
                _context.SepetKalemleri.Remove(sepetKalemi);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Sepet");
        }
    }
}