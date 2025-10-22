namespace AlasTech.Models.GorunumModelleri
{
    public class FavoriGorunumModeli
    {
        public int Id { get; set; } // Favori ID'si
        public int UrunId { get; set; } // Ürün ID'si
        public string UrunAdi { get; set; } = string.Empty; // Ürün adı
        public decimal Fiyat { get; set; } // Ürün fiyatı
        public string ResimUrl { get; set; } = string.Empty; // Ürün görsel URL'si
        public DateTime EklenmeTarihi { get; set; } // Favoriye eklenme tarihi
    }
}