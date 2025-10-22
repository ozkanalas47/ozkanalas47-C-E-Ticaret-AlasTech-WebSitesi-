using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Urun
    {
        [Key]
        public int Id { get; set; } // Ürünün benzersiz kimliği

        [Required]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty; // Ürün adı

        [Required]
        [StringLength(500)]
        public string Aciklama { get; set; } = string.Empty; // Ürün açıklaması

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Fiyat { get; set; } // Ürün fiyatı

        [Required]
        public int Stok { get; set; } // Ürün stok miktarı

        [Required]
        [StringLength(200)]
        public string ResimUrl { get; set; } = string.Empty; // Ürün resmi URL'si

        [Required]
        public int KategoriId { get; set; } // Ürünün bağlı olduğu kategori

        [ForeignKey("KategoriId")]
        public Kategori? Kategori { get; set; } // Ürünün bağlı olduğu kategori (nullable)

        [Required]
        public int SaticiProfilId { get; set; } // Ürünü ekleyen satıcı profili

        [ForeignKey("SaticiProfilId")]
        public SaticiProfili? SaticiProfili { get; set; } // Ürünü ekleyen satıcı profili (nullable)

        public ICollection<SepetKalemi> SepetKalemleri { get; set; } = new List<SepetKalemi>(); // Sepete eklenmiş kalemler
        public ICollection<SiparisKalemi> SiparisKalemleri { get; set; } = new List<SiparisKalemi>(); // Siparişe eklenmiş kalemler
        public ICollection<Yorum> Yorumlar { get; set; } = new List<Yorum>(); // Ürün için yapılan yorumlar
        public ICollection<Favori> Favoriler { get; set; } = new List<Favori>(); // Ürünü favoriye ekleyenler
        public ICollection<Karsilastirma> Karsilastirmalar { get; set; } = new List<Karsilastirma>(); // Ürünü karşılaştıranlar
        public ICollection<Puanlama> Puanlamalar { get; set; } = new List<Puanlama>(); // Ürün için yapılan puanlamalar
    }
}