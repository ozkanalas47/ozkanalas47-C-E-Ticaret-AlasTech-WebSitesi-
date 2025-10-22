using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Puanlama
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UrunId { get; set; }

        [ForeignKey("UrunId")]
        public Urun? Urun { get; set; } // Null atanabilir

        [Required]
        public int KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // Null atanabilir

        [Required]
        [Range(1, 5)]
        public int Yildiz { get; set; } // 1-5 arasında yıldız derecesi

        [Required]
        public DateTime Tarih { get; set; }
    }
}