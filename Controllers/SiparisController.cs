using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using AlasTech.Models.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlasTech.Controllers
{
    public class SiparisController : Controller
    {
        private readonly AlasTechContext _context;

        public SiparisController(AlasTechContext context)
        {
            _context = context;
        }

        // Sipariş listesi (kullanıcıya özel)
        public async Task<IActionResult> Liste()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId.Value)
                .Select(s => new SiparisGorunumModeli
                {
                    Id = s.Id,
                    Tarih = s.Tarih,
                    Durum = s.Durum,
                    Toplam = s.Toplam
                })
                .ToListAsync();

            return View(siparisler);
        }

        // Sipariş detayları
        public async Task<IActionResult> Detay(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var siparis = await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                .ThenInclude(sk => sk.Urun)
                .FirstOrDefaultAsync(s => s.Id == id && s.KullaniciId == kullaniciId.Value);

            if (siparis == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var model = new SiparisGorunumModeli
            {
                Id = siparis.Id,
                Tarih = siparis.Tarih,
                Durum = siparis.Durum,
                Toplam = siparis.Toplam,
                Kalemler = siparis.SiparisKalemleri
                    .Select(sk => new SiparisKalemiGorunumModeli
                    {
                        UrunId = sk.UrunId,
                        UrunAdi = sk.Urun?.Ad ?? "Bilinmeyen Ürün",
                        Miktar = sk.Miktar,
                        BirimFiyat = sk.BirimFiyat,
                        ResimUrl = sk.Urun?.ResimUrl ?? ""
                    })
                    .ToList()
            };

            return View(model);
        }

        // Ödeme ekranı
        public IActionResult Odeme()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            return View();
        }

        // Ödeme simülasyonu
        [HttpPost]
        public async Task<IActionResult> OdemeOnay()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var sepet = await _context.SepetKalemleri
                .Where(sk => sk.KullaniciId == kullaniciId.Value)
                .Include(sk => sk.Urun)
                .ToListAsync();

            if (!sepet.Any())
            {
                return RedirectToAction("Hata", "Shared");
            }

            // Stok kontrolü
            foreach (var kalem in sepet)
            {
                if (kalem.Urun == null || kalem.Urun.Stok < kalem.Miktar)
                {
                    return RedirectToAction("Hata", "Shared");
                }
            }

            // Geçerli ürünleri filtrele
            var gecerliSepet = sepet.Where(sk => sk.Urun != null).ToList();

            // Sipariş oluştur
            var siparis = new Siparis
            {
                KullaniciId = kullaniciId.Value,
                Tarih = DateTime.Now,
                Durum = "Bekliyor",
                Toplam = gecerliSepet.Sum(sk => sk.Urun!.Fiyat * sk.Miktar),
                SiparisKalemleri = gecerliSepet.Select(sk => new SiparisKalemi
                {
                    UrunId = sk.UrunId,
                    Miktar = sk.Miktar,
                    BirimFiyat = sk.Urun!.Fiyat
                }).ToList()
            };

            // Stok güncelle
            foreach (var kalem in gecerliSepet)
            {
                kalem.Urun!.Stok -= kalem.Miktar;
            }

            _context.Siparisler.Add(siparis);
            _context.SepetKalemleri.RemoveRange(sepet);
            await _context.SaveChangesAsync();

            // Simüle ödeme başarılı mesajı
            TempData["Mesaj"] = "Ödeme başarıyla tamamlandı!";
            return RedirectToAction("Liste");
        }
    }
}