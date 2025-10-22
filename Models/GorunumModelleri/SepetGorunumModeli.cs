namespace AlasTech.Models.GorunumModelleri
{
    public class SepetGorunumModeli
    {
        public int Id { get; set; } // Sepet kalemi ID'si
        public int UrunId { get; set; } // Ürün ID'si
        public string UrunAdi { get; set; } = string.Empty; // Ürün adı
        public decimal BirimFiyat { get; set; } // Ürün fiyatı
        public int Miktar { get; set; } // Ürün miktarı
        public decimal Toplam { get; set; } // Öğe toplam fiyatı
        public string ResimUrl { get; set; } = string.Empty; // Ürün görsel URL'si
    }
}