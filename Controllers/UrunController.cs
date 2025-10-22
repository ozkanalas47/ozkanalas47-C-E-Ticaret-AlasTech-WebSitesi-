using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using AlasTech.Models.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AlasTech.Controllers
{
    public class UrunController : Controller
    {
        private readonly AlasTechContext _context;

        public UrunController(AlasTechContext context)
        {
            _context = context;
        }

        private async Task LoadCategoriesAsync()
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
        }

        // Ürün listesi (sayfalama ve kategori filtresi ile)
        public async Task<IActionResult> Liste(int? kategoriId, int sayfa = 1)
        {
            await LoadCategoriesAsync();

            int sayfaBasina = 12; // Her sayfada 12 ürün
            var query = _context.Urunler.AsQueryable();

            if (kategoriId.HasValue)
            {
                query = query.Where(u => u.KategoriId == kategoriId.Value);
            }

            var urunler = await query
                .Skip((sayfa - 1) * sayfaBasina)
                .Take(sayfaBasina)
                .Select(u => new UrunGorunumModeli
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Fiyat = u.Fiyat,
                    Aciklama = u.Aciklama,
                    ResimUrl = u.ResimUrl,
                    Stok = u.Stok,
                    KategoriAdi = u.Kategori != null ? u.Kategori.Adi : "Bilinmiyor",
                    MagazaAdi = u.SaticiProfili != null ? u.SaticiProfili.MagazaAdi : "Bilinmiyor"
                })
                .ToListAsync();

            ViewBag.KategoriId = kategoriId;
            ViewBag.Sayfa = sayfa;
            ViewBag.ToplamSayfa = (int)Math.Ceiling((double)await query.CountAsync() / sayfaBasina);

            return View(urunler);
        }

        // Ürün detayları
        public async Task<IActionResult> Detay(int id)
        {
            await LoadCategoriesAsync();

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.SaticiProfili)
                .Include(u => u.Yorumlar)
                .ThenInclude(y => y.Kullanici)
                .Include(u => u.Favoriler)
                .ThenInclude(f => f.Kullanici)
                .Select(u => new UrunGorunumModeli
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Fiyat = u.Fiyat,
                    Aciklama = u.Aciklama,
                    ResimUrl = u.ResimUrl,
                    Stok = u.Stok,
                    KategoriAdi = u.Kategori != null ? u.Kategori.Adi : "Bilinmiyor",
                    MagazaAdi = u.SaticiProfili != null ? u.SaticiProfili.MagazaAdi : "Bilinmiyor"
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            // Yorumlar ve Favoriler için ViewBag ile veri gönder
            ViewBag.Yorumlar = await _context.Yorumlar
                .Where(y => y.UrunId == id)
                .Include(y => y.Kullanici)
                .ToListAsync();

            ViewBag.Favoriler = await _context.Favoriler
                .Where(f => f.UrunId == id)
                .Include(f => f.Kullanici)
                .ToListAsync();

            // Puanlama dağılımını hesapla
            var puanlamalar = await _context.Puanlamalar
                .Where(p => p.UrunId == id)
                .GroupBy(p => p.Yildiz)
                .Select(g => new { Yildiz = g.Key, Sayi = g.Count() })
                .ToListAsync();

            ViewBag.PuanDağılımı = Enumerable.Range(1, 5)
                .Select(i => new
                {
                    Yildiz = i,
                    Sayi = puanlamalar.FirstOrDefault(p => p.Yildiz == i)?.Sayi ?? 0
                })
                .OrderByDescending(x => x.Yildiz)
                .ToList();

            return View(urun);
        }

        // Ürün arama
        public async Task<IActionResult> Ara(string kelime, int sayfa = 1)
        {
            await LoadCategoriesAsync();

            int sayfaBasina = 12;
            var query = _context.Urunler.AsQueryable();

            if (!string.IsNullOrEmpty(kelime))
            {
                query = query.Where(u => u.Ad.Contains(kelime) || u.Aciklama.Contains(kelime));
            }

            var urunler = await query
                .Skip((sayfa - 1) * sayfaBasina)
                .Take(sayfaBasina)
                .Select(u => new UrunGorunumModeli
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Fiyat = u.Fiyat,
                    Aciklama = u.Aciklama,
                    ResimUrl = u.ResimUrl,
                    Stok = u.Stok,
                    KategoriAdi = u.Kategori != null ? u.Kategori.Adi : "Bilinmiyor",
                    MagazaAdi = u.SaticiProfili != null ? u.SaticiProfili.MagazaAdi : "Bilinmiyor"
                })
                .ToListAsync();

            ViewBag.Kelime = kelime;
            ViewBag.Sayfa = sayfa;
            ViewBag.ToplamSayfa = (int)Math.Ceiling((double)await query.CountAsync() / sayfaBasina);

            return View(urunler);
        }

        // Favoriye ekle
        [HttpPost]
        public async Task<IActionResult> FavoriEkle(int urunId)
        {
            await LoadCategoriesAsync();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var mevcutFavori = await _context.Favoriler
                .FirstOrDefaultAsync(f => f.KullaniciId == kullaniciId.Value && f.UrunId == urunId);

            if (mevcutFavori == null)
            {
                _context.Favoriler.Add(new Favori
                {
                    KullaniciId = kullaniciId.Value,
                    UrunId = urunId,
                    EklenmeTarihi = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detay", new { id = urunId });
        }

        // Karşılaştırmaya ekle
        [HttpPost]
        public async Task<IActionResult> KarsilastirmaEkle(int urunId)
        {
            await LoadCategoriesAsync();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var mevcutKarsilastirma = await _context.Karsilastirmalar
                .FirstOrDefaultAsync(k => k.KullaniciId == kullaniciId.Value && k.UrunId == urunId);

            if (mevcutKarsilastirma == null)
            {
                _context.Karsilastirmalar.Add(new Karsilastirma
                {
                    KullaniciId = kullaniciId.Value,
                    UrunId = urunId,
                    EklenmeTarihi = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detay", new { id = urunId });
        }

        // Yoruma ekle
        [HttpPost]
        public async Task<IActionResult> YorumEkle(int urunId, string icerik)
        {
            await LoadCategoriesAsync();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var yorum = new Yorum
            {
                UrunId = urunId,
                KullaniciId = kullaniciId.Value,
                Icerik = icerik,
                Tarih = DateTime.Now
            };

            _context.Yorumlar.Add(yorum);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detay", new { id = urunId });
        }

        // Ürüne yıldız puanı ekle
        [HttpPost]
        public async Task<IActionResult> Puanla(int urunId, int yildiz)
        {
            await LoadCategoriesAsync();

            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Musteri")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // Kullanıcının daha önce bu ürüne puan verip vermediğini kontrol et
            var mevcutPuan = await _context.Puanlamalar
                .FirstOrDefaultAsync(p => p.UrunId == urunId && p.KullaniciId == kullaniciId.Value);

            if (mevcutPuan == null)
            {
                _context.Puanlamalar.Add(new Puanlama
                {
                    UrunId = urunId,
                    KullaniciId = kullaniciId.Value,
                    Yildiz = yildiz,
                    Tarih = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                // Eğer kullanıcı zaten puan verdiyse, puanı güncelle
                mevcutPuan.Yildiz = yildiz;
                mevcutPuan.Tarih = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detay", new { id = urunId });
        }
    }
}