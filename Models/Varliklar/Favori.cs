using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Favori
    {
        [Key]
        public int Id { get; set; } // Favori ID'si, birincil anahtar

        [Required]
        public int KullaniciId { get; set; } // Favoriyi ekleyen kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı

        [Required]
        public int UrunId { get; set; } // Favoriye eklenen ürün

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // İlişkili ürün

        [Required]
        public DateTime EklenmeTarihi { get; set; } // Favoriye eklenme tarihi
    }
}