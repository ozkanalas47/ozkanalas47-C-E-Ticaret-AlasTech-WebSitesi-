namespace AlasTech.Models.GorunumModelleri
{
    public class KategoriGorunumModeli
    {
        public int Id { get; set; } // Kategori ID'si
        public string Adi { get; set; } = string.Empty; // Kategori adı
        public List<UrunGorunumModeli> Urunler { get; set; } = new List<UrunGorunumModeli>(); // Kategorideki ürünler
    }
}