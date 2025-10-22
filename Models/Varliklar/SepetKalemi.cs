using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class SepetKalemi
    {
        [Key]
        public int Id { get; set; } // Sepet kalemi ID'si, birincil anahtar

        [Required]
        public int KullaniciId { get; set; } // Sepeti kullanan kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı

        [Required]
        public int UrunId { get; set; } // Sepetteki ürün

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // İlişkili ürün

        [Required]
        public int Miktar { get; set; } // Ürün miktarı
    }
}