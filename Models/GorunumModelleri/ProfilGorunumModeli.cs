using System.ComponentModel.DataAnnotations;

namespace AlasTech.Models.GorunumModelleri
{
    public class ProfilGorunumModeli
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı 50 karakterden uzun olamaz.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [StringLength(100, ErrorMessage = "E-posta 100 karakterden uzun olamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Eposta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad soyad gereklidir.")]
        [StringLength(100, ErrorMessage = "Ad soyad 100 karakterden uzun olamaz.")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mevcut şifreyi giriniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string MevcutSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifreyi giriniz.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string YeniSifre { get; set; } = string.Empty;

        [Compare("YeniSifre", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string YeniSifreTekrar { get; set; } = string.Empty;
    }
}