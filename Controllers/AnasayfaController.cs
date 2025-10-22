using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AlasTech.Controllers
{
    public class AnasayfaController : Controller
    {
        private readonly AlasTechContext _context;

        public AnasayfaController(AlasTechContext context)
        {
            _context = context;
        }

        private async Task LoadCategoriesAsync()
        {
            ViewBag.Kategoriler = await _context.Kategoriler.ToListAsync();
        }

        // Ana sayfa: Öne çıkan ürünleri göster
        public async Task<IActionResult> Index()
        {
            await LoadCategoriesAsync();

            var urunler = await _context.Urunler
                .Take(10) // İlk 10 ürünü al
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

            // Her ürün için yıldız puanı ve puan veren kişi sayısını hesapla
            foreach (var urun in urunler)
            {
                var puanlamalar = await _context.Puanlamalar
                    .Where(p => p.UrunId == urun.Id)
                    .ToListAsync();

                if (puanlamalar.Any())
                {
                    urun.OrtalamaPuan = Math.Round(puanlamalar.Average(p => p.Yildiz), 1);
                    urun.PuanVerenKisiSayisi = puanlamalar.Count;
                }
                else
                {
                    urun.OrtalamaPuan = 0.0;
                    urun.PuanVerenKisiSayisi = 0;
                }
            }

            return View(urunler);
        }
    }
}