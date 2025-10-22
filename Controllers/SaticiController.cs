using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using AlasTech.Models.Varliklar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AlasTech.Controllers
{
    public class SaticiController : Controller
    {
        private readonly AlasTechContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SaticiController(AlasTechContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // Satıcı paneli ana sayfası
        public async Task<IActionResult> KontrolPaneli()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var model = new SaticiPanelGorunumModeli
            {
                ToplamUrunSayisi = await _context.Urunler
                    .Where(u => u.SaticiProfilId == saticiProfili.Id)
                    .CountAsync(),
                ToplamSiparisSayisi = await _context.SiparisKalemleri
                    .Where(sk => sk.Urun != null && sk.Urun.SaticiProfilId == saticiProfili.Id)
                    .CountAsync(),
                ToplamCiro = await _context.SiparisKalemleri
                    .Where(sk => sk.Urun != null && sk.Urun.SaticiProfilId == saticiProfili.Id)
                    .SumAsync(sk => sk.BirimFiyat * sk.Miktar)
            };

            return View(model);
        }

        // Satıcının ürünleri
        public async Task<IActionResult> UrunYonetimi()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urunler = await _context.Urunler
                .Where(u => u.SaticiProfilId == saticiProfili.Id)
                .Include(u => u.Kategori)
                .Select(u => new UrunGorunumModeli
                {
                    Id = u.Id,
                    Ad = u.Ad,
                    Fiyat = u.Fiyat,
                    Aciklama = u.Aciklama,
                    Stok = u.Stok,
                    KategoriAdi = u.Kategori != null ? u.Kategori.Adi : "Bilinmiyor",
                    MagazaAdi = u.SaticiProfili != null ? u.SaticiProfili.MagazaAdi : "Bilinmiyor"
                })
                .ToListAsync();

            return View(urunler);
        }

        // Ürün ekleme (GET)
        public async Task<IActionResult> UrunEkle()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(new UrunGorunumModeli());
        }

        // Ürün ekleme (POST)
        [HttpPost]
        public async Task<IActionResult> UrunEkle(UrunGorunumModeli model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            if (ModelState.IsValid)
            {
                var urun = new Urun
                {
                    Ad = model.Ad,
                    Aciklama = model.Aciklama,
                    Fiyat = model.Fiyat,
                    Stok = model.Stok,
                    KategoriId = model.KategoriId,
                    SaticiProfilId = saticiProfili.Id
                };

                // Resim yükleme
                if (model.Resim != null && model.Resim.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Resim.FileName);
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "resimler/urunler");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Resim.CopyToAsync(fileStream);
                    }
                    urun.ResimUrl = $"/resimler/urunler/{uniqueFileName}";
                }
                else
                {
                    urun.ResimUrl = "/resimler/urunler/varsayilan.jpg";
                }

                _context.Urunler.Add(urun);
                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Ürün başarıyla eklendi!";
                return RedirectToAction("UrunYonetimi");
            }

            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(model);
        }

        // Ürün düzenleme (GET)
        public async Task<IActionResult> UrunDuzenle(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.Id == id && u.SaticiProfilId == saticiProfili.Id);

            if (urun == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var model = new UrunGorunumModeli
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Aciklama = urun.Aciklama,
                Fiyat = urun.Fiyat,
                Stok = urun.Stok,
                KategoriId = urun.KategoriId,

                // Corrected line in UrunDetay method
                KategoriAdi = urun.Kategori != null ? urun.Kategori.Adi : "Bilinmiyor",
                // Corrected line in UrunDetay method
                MagazaAdi = saticiProfili.MagazaAdi
            };

            // Mevcut ResimUrl’yi ViewBag ile gönderiyoruz
            ViewBag.MevcutResimUrl = urun.ResimUrl;
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(model);
        }

        // Ürün düzenleme (POST)
        [HttpPost]
        public async Task<IActionResult> UrunDuzenle(UrunGorunumModeli model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urun = await _context.Urunler
                .FirstOrDefaultAsync(u => u.Id == model.Id && u.SaticiProfilId == saticiProfili.Id);

            if (urun == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            if (ModelState.IsValid)
            {
                urun.Ad = model.Ad;
                urun.Aciklama = model.Aciklama;
                urun.Fiyat = model.Fiyat;
                urun.Stok = model.Stok;
                urun.KategoriId = model.KategoriId;

                // Resim güncelleme
                if (model.Resim != null && model.Resim.Length > 0)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(urun.ResimUrl) && urun.ResimUrl != "/resimler/urunler/varsayilan.jpg")
                    {
                        var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, urun.ResimUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Eski dosya silme hatası: {ex.Message}");
                            }
                        }
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Resim.FileName);
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "resimler/urunler");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Resim.CopyToAsync(fileStream);
                    }
                    urun.ResimUrl = $"/resimler/urunler/{uniqueFileName}";
                }

                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Ürün başarıyla güncellendi!";
                return RedirectToAction("UrunYonetimi");
            }

            ViewBag.MevcutResimUrl = urun.ResimUrl;
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UrunSil(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urun = await _context.Urunler
                .FirstOrDefaultAsync(u => u.Id == id && u.SaticiProfilId == saticiProfili.Id);

            if (urun == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            // Resmi sil
            if (!string.IsNullOrEmpty(urun.ResimUrl) && urun.ResimUrl != "/resimler/urunler/varsayilan.jpg")
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, urun.ResimUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Dosya silme hatası: {ex.Message}");
                    }
                }
            }

            // Ürünü sil (ilişkili kayıtlar otomatik silinecek)
            _context.Urunler.Remove(urun);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Ürün başarıyla silindi!";
            return RedirectToAction("UrunYonetimi");
        }

        // Ürün detay (GET)
        public async Task<IActionResult> UrunDetay(int id)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urun = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.Id == id && u.SaticiProfilId == saticiProfili.Id);

            if (urun == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var model = new UrunGorunumModeli
            {
                Id = urun.Id,
                Ad = urun.Ad,
                Aciklama = urun.Aciklama,
                Fiyat = urun.Fiyat,
                Stok = urun.Stok,
                KategoriAdi = urun.Kategori != null ? urun.Kategori.Adi : "Bilinmiyor",
                MagazaAdi = saticiProfili.MagazaAdi
            };

            ViewBag.MevcutResimUrl = urun.ResimUrl;
            return View(model);
        }

        // Satıcının siparişleri
        public async Task<IActionResult> SiparisYonetimi()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (!kullaniciId.HasValue || HttpContext.Session.GetString("Rol") != "Satici")
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var saticiProfili = await _context.SaticiProfilleri
                .FirstOrDefaultAsync(sp => sp.KullaniciId == kullaniciId.Value);

            if (saticiProfili == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisKalemleri)
                .ThenInclude(sk => sk.Urun)
                .Where(s => s.SiparisKalemleri.Any(sk => sk.Urun != null && sk.Urun.SaticiProfilId == saticiProfili.Id))
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
    }
}