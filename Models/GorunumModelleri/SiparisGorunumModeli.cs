namespace AlasTech.Models.GorunumModelleri
{
    public class SiparisGorunumModeli
    {
        public int Id { get; set; } // Sipariş ID'si
        public DateTime Tarih { get; set; } // Sipariş tarihi
        public string Durum { get; set; } = string.Empty; // Sipariş durumu
        public decimal Toplam { get; set; } // Sipariş toplamı
        public List<SiparisKalemiGorunumModeli> Kalemler { get; set; } = new List<SiparisKalemiGorunumModeli>(); // Sipariş kalemleri
    }

    public class SiparisKalemiGorunumModeli
    {
        public int UrunId { get; set; } // Ürün ID'si
        public string UrunAdi { get; set; } = string.Empty; // Ürün adı
        public int Miktar { get; set; } // Ürün miktarı
        public decimal BirimFiyat { get; set; } // Ürün fiyatı
        public string ResimUrl { get; set; } = string.Empty; // Ürün görsel URL'si
    }
}