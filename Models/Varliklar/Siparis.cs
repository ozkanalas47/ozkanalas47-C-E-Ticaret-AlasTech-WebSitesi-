using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Siparis
    {
        [Key]
        public int Id { get; set; } // Sipariş ID'si, birincil anahtar

        [Required]
        public int KullaniciId { get; set; } // Siparişi veren kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı

        [Required]
        public DateTime Tarih { get; set; } // Sipariş tarihi

        [Required]
        [StringLength(50)]
        public string Durum { get; set; } = "Bekliyor"; // Sipariş durumu: Bekliyor, Onaylandı, Gönderildi

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Toplam { get; set; } // Sipariş toplam tutarı

        public ICollection<SiparisKalemi> SiparisKalemleri { get; set; } = new List<SiparisKalemi>(); // Siparişin ürünleri
    }
}