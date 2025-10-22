namespace AlasTech.Models.GorunumModelleri
{
    public class SaticiPanelGorunumModeli
    {
        public int ToplamUrunSayisi { get; set; } // Satıcının ürün sayısı
        public int ToplamSiparisSayisi { get; set; } // Satıcının sipariş sayısı
        public decimal ToplamCiro { get; set; } // Satıcının cirosu
    }
}