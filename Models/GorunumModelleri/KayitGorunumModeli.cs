namespace AlasTech.Models.GorunumModelleri
{
    public class KayitGorunumModeli
    {
        public string KullaniciAdi { get; set; } = string.Empty; // Kullanıcı adı
        public string Sifre { get; set; } = string.Empty; // Şifre
        public string Eposta { get; set; } = string.Empty; // E-posta
        public string AdSoyad { get; set; } = string.Empty; // Ad ve soyad
        public string Rol { get; set; } = "Musteri"; // Rol: Musteri veya Satici
        public string? MagazaAdi { get; set; } // Satıcı için mağaza adı (opsiyonel)
        public string? Adres { get; set; } // Satıcı için adres (opsiyonel)
        public string? Telefon { get; set; } // Satıcı için telefon (opsiyonel)
    }
}