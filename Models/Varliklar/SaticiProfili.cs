using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class SaticiProfili
    {
        [Key]
        public int Id { get; set; } // Satıcı profili ID'si, birincil anahtar

        [Required]
        public int KullaniciId { get; set; } // İlişkili kullanıcı

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; } // İlişkili kullanıcı

        [Required]
        [StringLength(100)]
        public string MagazaAdi { get; set; } = string.Empty; // Satıcı mağaza adı

        [StringLength(200)]
        public string Adres { get; set; } = string.Empty; // Satıcı adresi

        [StringLength(20)]
        public string Telefon { get; set; } = string.Empty; // Satıcı telefon numarası

        public ICollection<Urun> Urunler { get; set; } = new List<Urun>(); // Satıcının ürünleri
    }
}