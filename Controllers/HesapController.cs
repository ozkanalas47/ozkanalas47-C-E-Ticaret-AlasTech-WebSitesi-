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
    public class HesapController : Controller
    {
        private readonly AlasTechContext _context;

        public HesapController(AlasTechContext context)
        {
            _context = context;
        }

        // Giriş ekranı
        public IActionResult Giris()
        {
            return View(new GirisGorunumModeli());
        }

        [HttpPost]
        public async Task<IActionResult> Giris(GirisGorunumModeli model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == model.KullaniciAdi && k.Sifre == model.Sifre);

            if (kullanici == null)
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            HttpContext.Session.SetInt32("KullaniciId", kullanici.Id);
            HttpContext.Session.SetString("Rol", kullanici.Rol);
            HttpContext.Session.SetString("AdSoyad", kullanici.AdSoyad);

            return RedirectToAction("Index", "Anasayfa");
        }

        // Kayıt ekranı
        public IActionResult Kayit()
        {
            return View(new KayitGorunumModeli());
        }

        // Kayıt işlemi (Geçici olarak hash'siz)
        [HttpPost]
        public async Task<IActionResult> Kayit(KayitGorunumModeli model)
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
                Sifre = model.Sifre, 
                Rol = "Musteri",
                Eposta = model.Eposta,
                AdSoyad = model.AdSoyad
            };

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();

            return RedirectToAction("Giris");
        }

        // Çıkış işlemi
        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Anasayfa");
        }

        // Profil sayfasını göster (görüntüleme)
        public async Task<IActionResult> Profil()
        {
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanici = await _context.Kullanicilar
                .Include(k => k.Siparisler)
                .Include(k => k.SaticiProfili)
                .FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        // Profil düzenleme formunu göster
        public async Task<IActionResult> Duzenle()
        {
            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            var model = new ProfilGorunumModeli
            {
                Id = kullanici.Id,
                KullaniciAdi = kullanici.KullaniciAdi,
                Eposta = kullanici.Eposta,
                AdSoyad = kullanici.AdSoyad
            };

            return View(model);
        }

        // Profil güncelle
        [HttpPost]
        public async Task<IActionResult> Duzenle(ProfilGorunumModeli model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? kullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                return RedirectToAction("Giris", "Hesap");
            }

            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.Id == kullaniciId);
            if (kullanici == null)
            {
                return NotFound();
            }

            // Mevcut şifreyi doğrula
            if (kullanici.Sifre != model.MevcutSifre) // Hash'siz kontrol
            {
                ModelState.AddModelError("MevcutSifre", "Mevcut şifre yanlış.");
                return View(model);
            }

            // Kullanıcı bilgilerini güncelle
            kullanici.KullaniciAdi = model.KullaniciAdi;
            kullanici.Eposta = model.Eposta;
            kullanici.AdSoyad = model.AdSoyad;

            // Yeni şifreyi güncelle
            if (!string.IsNullOrEmpty(model.YeniSifre))
            {
                kullanici.Sifre = model.YeniSifre; // Hash'siz
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("Profil");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu: " + ex.InnerException?.Message);
                return View(model);
            }
        }

        // Geçici olarak admin ve satıcı ekleme (Hash'siz)
        public async Task<IActionResult> EkleAdminVeSatici()
        {
            // Admin ekle
            if (!await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == "admin"))
            {
                var admin = new Kullanici
                {
                    KullaniciAdi = "admin",
                    Sifre = "admin123", // Düz metin
                    Rol = "Yonetici",
                    Eposta = "admin@alastech.com",
                    AdSoyad = "Admin Kullanıcı"
                };
                _context.Kullanicilar.Add(admin);
            }

            // Satıcı ekle
            if (!await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == "satici"))
            {
                var satici = new Kullanici
                {
                    KullaniciAdi = "satici",
                    Sifre = "satici123", // Düz metin
                    Rol = "Satici",
                    Eposta = "satici@alastech.com",
                    AdSoyad = "Satıcı Kullanıcı"
                };
                _context.Kullanicilar.Add(satici);
                await _context.SaveChangesAsync();

                // Satıcı profili oluştur
                var saticiProfili = new SaticiProfili
                {
                    KullaniciId = satici.Id,
                    MagazaAdi = "Satıcı Mağaza",
                    Adres = "Satıcı Adresi",
                    Telefon = "1234567890"
                };
                _context.SaticiProfilleri.Add(saticiProfili);
            }

            await _context.SaveChangesAsync();
            return Content("Admin ve satıcı kullanıcılar başarıyla eklendi. Admin: admin/admin123, Satıcı: satici/satici123");
        }


    }
}