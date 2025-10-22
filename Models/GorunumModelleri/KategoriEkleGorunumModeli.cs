using System.ComponentModel.DataAnnotations;

namespace AlasTech.Models.GorunumModelleri
{
    public class KategoriEkleGorunumModeli
    {
        [Required(ErrorMessage = "Kategori adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Kategori adı 100 karakterden uzun olamaz.")]
        public string Adi { get; set; } = string.Empty;

        public int? UstKategoriId { get; set; } // Opsiyonel üst kategori
    }
}