using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using AlasTech.Models.Varliklar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AlasTech.Controllers
{
    public class YoneticiController : Controller
    {
        private readonly AlasTechContext _context;

        public YoneticiController(AlasTechContext context)
        {
            _context = context;
        }

        // Yönetici paneli ana sayfası
        public async Task<IActionResult> KontrolPaneli()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var model = new YoneticiPanelGorunumModeli
            {
                ToplamUrunSayisi = await _context.Urunler.CountAsync(),
                ToplamSiparisSayisi = await _context.Siparisler.CountAsync(),
                ToplamKullaniciSayisi = await _context.Kullanicilar.CountAsync(),
                ToplamCiro = await _context.Siparisler.SumAsync(s => s.Toplam)
            };

            return View(model);
        }

        // Satıcı ekleme ekranı
        public IActionResult SaticiEkle()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            return View(new KayitGorunumModeli());
        }

        // Satıcı ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> SaticiEkle(KayitGorunumModeli model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == model.KullaniciAdi))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(model);
            }

            if (await _context.Kullanicilar.AnyAsync(k => k.Eposta == model.Eposta))
            {
                ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor.");
                return View(model);
            }

            var kullanici = new Kullanici
            {
                KullaniciAdi = model.KullaniciAdi,
                Sifre = GetMd5Hash(model.Sifre),
                Rol = "Satici",
                Eposta = model.Eposta,
                AdSoyad = model.AdSoyad
            };

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();

            var saticiProfili = new SaticiProfili
            {
                KullaniciId = kullanici.Id,
                MagazaAdi = model.MagazaAdi ?? "Varsayılan Mağaza",
                Adres = model.Adres ?? "",
                Telefon = model.Telefon ?? ""
            };
            _context.SaticiProfilleri.Add(saticiProfili);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Satıcı başarıyla eklendi!";
            return RedirectToAction("KontrolPaneli");
        }

        // Kullanıcı yönetimi
        public async Task<IActionResult> KullaniciYonetimi()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanicilar = await _context.Kullanicilar
                .Include(k => k.SaticiProfili)
                .ToListAsync();

            return View(kullanicilar);
        }

        // Kullanıcı düzenleme ekranı
        public async Task<IActionResult> KullaniciDuzenle(int id)
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanici = await _context.Kullanicilar
                .Include(k => k.SaticiProfili)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var model = new KayitGorunumModeli
            {
                KullaniciAdi = kullanici.KullaniciAdi,
                Sifre = kullanici.Sifre,
                Eposta = kullanici.Eposta,
                AdSoyad = kullanici.AdSoyad,
                Rol = kullanici.Rol,
                MagazaAdi = kullanici.SaticiProfili?.MagazaAdi,
                Adres = kullanici.SaticiProfili?.Adres,
                Telefon = kullanici.SaticiProfili?.Telefon
            };

            ViewBag.KullaniciId = id;
            return View(model);
        }

        // Kullanıcı düzenleme işlemi
        [HttpPost]
        public async Task<IActionResult> KullaniciDuzenle(int id, KayitGorunumModeli model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KullaniciId = id;
                return View(model);
            }

            var kullanici = await _context.Kullanicilar
                .Include(k => k.SaticiProfili)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            if (await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == model.KullaniciAdi && k.Id != id))
            {
                ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");
                ViewBag.KullaniciId = id;
                return View(model);
            }

            if (await _context.Kullanicilar.AnyAsync(k => k.Eposta == model.Eposta && k.Id != id))
            {
                ModelState.AddModelError("", "Bu e-posta zaten kullanılıyor.");
                ViewBag.KullaniciId = id;
                return View(model);
            }

            kullanici.KullaniciAdi = model.KullaniciAdi;
            kullanici.Sifre = GetMd5Hash(model.Sifre);
            kullanici.Eposta = model.Eposta;
            kullanici.AdSoyad = model.AdSoyad;
            kullanici.Rol = model.Rol;

            if (kullanici.Rol == "Satici")
            {
                if (kullanici.SaticiProfili == null)
                {
                    var saticiProfili = new SaticiProfili
                    {
                        KullaniciId = kullanici.Id,
                        MagazaAdi = model.MagazaAdi ?? "Varsayılan Mağaza",
                        Adres = model.Adres ?? "",
                        Telefon = model.Telefon ?? ""
                    };
                    _context.SaticiProfilleri.Add(saticiProfili);
                }
                else
                {
                    kullanici.SaticiProfili.MagazaAdi = model.MagazaAdi ?? "Varsayılan Mağaza";
                    kullanici.SaticiProfili.Adres = model.Adres ?? "";
                    kullanici.SaticiProfili.Telefon = model.Telefon ?? "";
                }
            }
            else
            {
                if (kullanici.SaticiProfili != null)
                {
                    _context.SaticiProfilleri.Remove(kullanici.SaticiProfili);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Mesaj"] = "Kullanıcı başarıyla güncellendi!";
            return RedirectToAction("KullaniciYonetimi");
        }

        // Kullanıcı silme (POST)
        [HttpPost]
        public async Task<IActionResult> KullaniciSil(int id)
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var currentKullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (currentKullaniciId.HasValue && currentKullaniciId.Value == id)
            {
                TempData["HataMesaj"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction("KullaniciYonetimi");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullanici.Id);
            if (saticiProfili != null)
            {
                var urunler = await _context.Urunler
                    .Where(u => u.SaticiProfilId == saticiProfili.Id)
                    .ToListAsync();
                if (urunler.Any())
                {
                    _context.Urunler.RemoveRange(urunler);
                }
                _context.SaticiProfilleri.Remove(saticiProfili);
            }

            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullanici.Id)
                .ToListAsync();
            if (siparisler.Any())
            {
                foreach (var siparis in siparisler)
                {
                    var siparisKalemleri = await _context.SiparisKalemleri
                        .Where(sk => sk.SiparisId == siparis.Id)
                        .ToListAsync();
                    if (siparisKalemleri.Any())
                    {
                        _context.SiparisKalemleri.RemoveRange(siparisKalemleri);
                    }
                }
                _context.Siparisler.RemoveRange(siparisler);
            }

            var favoriler = await _context.Favoriler
                .Where(f => f.KullaniciId == kullanici.Id)
                .ToListAsync();
            if (favoriler.Any())
            {
                _context.Favoriler.RemoveRange(favoriler);
            }

            var karsilastirmalar = await _context.Karsilastirmalar
                .Where(k => k.KullaniciId == kullanici.Id)
                .ToListAsync();
            if (karsilastirmalar.Any())
            {
                _context.Karsilastirmalar.RemoveRange(karsilastirmalar);
            }

            var sepetKalemleri = await _context.SepetKalemleri
                .Where(sk => sk.KullaniciId == kullanici.Id)
                .ToListAsync();
            if (sepetKalemleri.Any())
            {
                _context.SepetKalemleri.RemoveRange(sepetKalemleri);
            }

            var yorumlar = await _context.Yorumlar
                .Where(y => y.KullaniciId == kullanici.Id)
                .ToListAsync();
            if (yorumlar.Any())
            {
                _context.Yorumlar.RemoveRange(yorumlar);
            }

            _context.Kullanicilar.Remove(kullanici);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Kullanıcı başarıyla silindi!";
            return RedirectToAction("KullaniciYonetimi");
        }

        // Ürün yönetimi
        public async Task<IActionResult> UrunYonetimi()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var urunler = await _context.Urunler
                .Include(u => u.Kategori)
                .Include(u => u.SaticiProfili)
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

            return View(urunler);
        }

        // Sipariş yönetimi
        public async Task<IActionResult> SiparisYonetimi()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var siparisler = await _context.Siparisler
                .Include(s => s.Kullanici)
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

        // Kategori ekleme ekranı
        public async Task<IActionResult> KategoriEkle()
        {
            if (HttpContext.Session.GetString("Rol") != "Yonetici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            // Üst kategorileri dropdown için SelectListItem olarak al
            ViewBag.UstKategoriler = (await _context.Kategoriler
                .Where(k => k.UstKategoriId == null)
                .Select(k => new { k.Id, k.Adi })
                .ToListAsync())
                .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Adi
                }).ToList();

            return View(new KategoriEkleGorunumModeli());
        }

        // Kategori ekleme işlemi
        [HttpPost]
        public async Task<IActionResult> KategoriEkle(KategoriEkleGorunumModeli model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UstKategoriler = (await _context.Kategoriler
                    .Where(k => k.UstKategoriId == null)
                    .Select(k => new { k.Id, k.Adi })
                    .ToListAsync())
                    .Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Adi
                    }).ToList();
                return View(model);
            }

            var kategori = new Kategori
            {
                Adi = model.Adi,
                UstKategoriId = model.UstKategoriId
            };

            _context.Kategoriler.Add(kategori);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Kategori başarıyla eklendi!";
            return RedirectToAction("KontrolPaneli");
        }

        // MD5 ile hash oluşturma (örnek, üretimde BCrypt önerilir)
        private string GetMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}