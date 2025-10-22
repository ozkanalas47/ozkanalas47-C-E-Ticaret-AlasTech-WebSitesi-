using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Karsilastirma
    {
        [Key]
        public int Id { get; set; } // Karşılaştırma ID'si, birincil anahtar

        [Required]
        public int KullaniciId { get; set; } // Karşılaştırmayı yapan kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı

        [Required]
        public int UrunId { get; set; } // Karşılaştırılan ürün

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // İlişkili ürün

        [Required]
        public DateTime EklenmeTarihi { get; set; } // Karşılaştırmaya eklenme tarihi
    }
}