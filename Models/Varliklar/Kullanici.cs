using System.ComponentModel.DataAnnotations;

namespace AlasTech.Models.Varliklar
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; } // Kullanıcı ID'si, birincil anahtar

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; } = string.Empty; // Kullanıcı adı (benzersiz)

        [Required]
        [StringLength(100)]
        public string Sifre { get; set; } = string.Empty; // Şifre (test için düz metin, üretimde hash'lenir)

        [Required]
        [StringLength(20)]
        public string Rol { get; set; } = "Musteri"; // Rol: Yonetici, Satici, Musteri

        [Required]
        [StringLength(100)]
        public string Eposta { get; set; } = string.Empty; // Kullanıcı e-postası

        [Required]
        [StringLength(100)]
        public string AdSoyad { get; set; } = string.Empty; // Kullanıcı adı ve soyadı

        public SaticiProfili? SaticiProfili { get; set; } // Kullanıcının satıcı profili (opsiyonel)

        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>(); // Kullanıcının siparişleri
        public ICollection<Favori> Favoriler { get; set; } = new List<Favori>(); // Kullanıcının favorileri
        public ICollection<Karsilastirma> Karsilastirmalar { get; set; } = new List<Karsilastirma>(); // Kullanıcının karşılaştırmaları
    }
}