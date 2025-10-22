using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AlasTech.Models.GorunumModelleri
{
    public class UrunGorunumModeli
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        public string Ad { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Aciklama { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Fiyat { get; set; }

        [Required(ErrorMessage = "Stok adedi zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok adedi negatif olamaz.")]
        public int Stok { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int KategoriId { get; set; }

        public string KategoriAdi { get; set; } = string.Empty;

        public string MagazaAdi { get; set; } = string.Empty;

        // Resim yükleme için
        public IFormFile? Resim { get; set; }

        // Resim URL'si için
        public string ResimUrl { get; set; } = string.Empty;

        // Yeni alanlar: Ortalama yıldız puanı ve puan veren kişi sayısı
        public double OrtalamaPuan { get; set; } = 0.0;
        public int PuanVerenKisiSayisi { get; set; } = 0;
    }
}