using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class SiparisKalemi
    {
        [Key]
        public int Id { get; set; } // Sipariş kalemi ID'si, birincil anahtar

        [Required]
        public int SiparisId { get; set; } // İlişkili sipariş

        [ForeignKey("SiparisId")]
        public Siparis? Siparis { get; set; } // İlişkili sipariş

        [Required]
        public int UrunId { get; set; } // İlişkili ürün

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // İlişkili ürün

        [Required]
        public int Miktar { get; set; } // Ürün miktarı

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BirimFiyat { get; set; } // Ürünün sipariş anındaki fiyatı
    }
}