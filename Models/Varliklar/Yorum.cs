using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Yorum
    {
        [Key]
        public int Id { get; set; } // Yorum ID'si, birincil anahtar

        [Required]
        public int UrunId { get; set; } // Yorum yapılan ürün

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // İlişkili ürün (null atanabilir)

        [Required]
        public int KullaniciId { get; set; } // Yorumu yapan kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı (null atanabilir)

        [Required]
        [StringLength(500)]
        public string Icerik { get; set; } = string.Empty; // Yorum metni

        [Required]
        public DateTime Tarih { get; set; } // Yorum tarihi
    }
}