using AlasTech.Data;
using AlasTech.Models.GorunumModelleri;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlasTech.Controllers
{
    public class KategoriController : Controller
    {
        private readonly AlasTechContext _context;

        public KategoriController(AlasTechContext context)
        {
            _context = context;
        }

        // Kategoriye göre ürün listesi
        public async Task<IActionResult> Liste(int id, int sayfa = 1)
        {
            int sayfaBasina = 12;
            var kategori = await _context.Kategoriler
                .Include(k => k.Urunler)
                .ThenInclude(u => u.SaticiProfili)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kategori == null)
            {
                return RedirectToAction("Hata", "Shared");
            }

            var urunler = kategori.Urunler
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
                    KategoriAdi = kategori.Adi,
                    MagazaAdi = u.SaticiProfili != null ? u.SaticiProfili.MagazaAdi : "Bilinmiyor"
                })
                .ToList();

            var model = new KategoriGorunumModeli
            {
                Id = kategori.Id,
                Adi = kategori.Adi,
                Urunler = urunler
            };

            ViewBag.Sayfa = sayfa;
            ViewBag.ToplamSayfa = (int)Math.Ceiling((double)kategori.Urunler.Count / sayfaBasina);

            return View(model);
        }
    }
}