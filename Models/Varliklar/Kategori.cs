using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlasTech.Models.Varliklar
{
    public class Kategori
    {
        [Key]
        public int Id { get; set; } // Kategori ID'si, birincil anahtar

        [Required]
        [StringLength(50)]
        public string Adi { get; set; } = string.Empty; // Kategori adı

        public int? UstKategoriId { get; set; } // Üst kategori (opsiyonel)

        [ForeignKey("UstKategoriId")]
        public Kategori? UstKategori { get; set; } // İlişkili üst kategori

        public ICollection<Urun> Urunler { get; set; } = new List<Urun>(); // Kategorideki ürünler
    }
}