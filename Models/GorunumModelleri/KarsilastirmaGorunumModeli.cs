namespace AlasTech.Models.GorunumModelleri
{
    public class KarsilastirmaGorunumModeli
    {
        public int Id { get; set; } // Karşılaştırma ID'si
        public int UrunId { get; set; } // Ürün ID'si
        public string UrunAdi { get; set; } = string.Empty; // Ürün adı
        public decimal Fiyat { get; set; } // Ürün fiyatı
        public string Aciklama { get; set; } = string.Empty; // Ürün açıklaması
        public string ResimUrl { get; set; } = string.Empty; // Ürün görsel URL'si
        public int Stok { get; set; } // Ürün stok miktarı
        public DateTime EklenmeTarihi { get; set; } // Karşılaştırmaya eklenme tarihi
    }
}