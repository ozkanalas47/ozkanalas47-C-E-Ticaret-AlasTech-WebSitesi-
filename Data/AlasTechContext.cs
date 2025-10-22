using AlasTech.Models.Varliklar;
using Microsoft.EntityFrameworkCore;

namespace AlasTech.Data
{
    public class AlasTechContext : DbContext
    {
        public AlasTechContext(DbContextOptions<AlasTechContext> options) : base(options)
        {
        }

        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<SaticiProfili> SaticiProfilleri { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisKalemi> SiparisKalemleri { get; set; }
        public DbSet<SepetKalemi> SepetKalemleri { get; set; }
        public DbSet<Yorum> Yorumlar { get; set; }
        public DbSet<Favori> Favoriler { get; set; }
        public DbSet<Karsilastirma> Karsilastirmalar { get; set; }
        public DbSet<Puanlama> Puanlamalar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kullanici>()
                .HasIndex(u => u.KullaniciAdi)
                .IsUnique();

            modelBuilder.Entity<Kullanici>()
                .HasIndex(u => u.Eposta)
                .IsUnique();

            modelBuilder.Entity<SaticiProfili>()
                .HasOne(sp => sp.Kullanici)
                .WithOne(k => k.SaticiProfili)
                .HasForeignKey<SaticiProfili>(sp => sp.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Urun>()
                .HasOne(u => u.SaticiProfili)
                .WithMany(sp => sp.Urunler)
                .HasForeignKey(u => u.SaticiProfilId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Urun)
                .WithMany(u => u.Favoriler)
                .HasForeignKey(f => f.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favori>()
                .HasOne(f => f.Kullanici)
                .WithMany(k => k.Favoriler)
                .HasForeignKey(f => f.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Karsilastirma>()
                .HasOne(k => k.Urun)
                .WithMany(u => u.Karsilastirmalar)
                .HasForeignKey(k => k.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Karsilastirma>()
                .HasOne(k => k.Kullanici)
                .WithMany(ku => ku.Karsilastirmalar)
                .HasForeignKey(k => k.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.Urun)
                .WithMany(u => u.Yorumlar)
                .HasForeignKey(y => y.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Yorum>()
                .HasOne(y => y.Kullanici)
                .WithMany()
                .HasForeignKey(y => y.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SepetKalemi>()
                .HasOne(sk => sk.Urun)
                .WithMany(u => u.SepetKalemleri)
                .HasForeignKey(sk => sk.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SepetKalemi>()
                .HasOne(sk => sk.Kullanici)
                .WithMany()
                .HasForeignKey(sk => sk.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SiparisKalemi>()
                .HasOne(sk => sk.Urun)
                .WithMany(u => u.SiparisKalemleri)
                .HasForeignKey(sk => sk.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiparisKalemi>()
                .HasOne(sk => sk.Siparis)
                .WithMany(s => s.SiparisKalemleri)
                .HasForeignKey(sk => sk.SiparisId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany(k => k.Siparisler)
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Puanlama>()
                .HasOne(p => p.Urun)
                .WithMany(u => u.Puanlamalar)
                .HasForeignKey(p => p.UrunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Puanlama>()
                .HasOne(p => p.Kullanici)
                .WithMany()
                .HasForeignKey(p => p.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade); // Değişiklik burada: Kullanıcı silindiğinde puanlamalar da silinsin

            modelBuilder.Entity<Puanlama>()
                .HasIndex(p => new { p.UrunId, p.KullaniciId })
                .IsUnique();

            modelBuilder.Entity<Kullanici>().HasData(
                new Kullanici
                {
                    Id = 1,
                    KullaniciAdi = "admin",
                    Sifre = "admin123",
                    Rol = "Yonetici",
                    Eposta = "admin@alastech.com",
                    AdSoyad = "Admin Kullanıcı"
                },
                new Kullanici
                {
                    Id = 2,
                    KullaniciAdi = "satici1",
                    Sifre = "satici123",
                    Rol = "Satici",
                    Eposta = "satici1@alastech.com",
                    AdSoyad = "Satıcı Bir"
                },
                new Kullanici
                {
                    Id = 3,
                    KullaniciAdi = "musteri1",
                    Sifre = "musteri123",
                    Rol = "Musteri",
                    Eposta = "musteri1@alastech.com",
                    AdSoyad = "Müşteri Bir"
                }
            );

            modelBuilder.Entity<SaticiProfili>().HasData(
                new SaticiProfili
                {
                    Id = 1,
                    KullaniciId = 2,
                    MagazaAdi = "Birinci Mağaza",
                    Adres = "İstanbul, Türkiye",
                    Telefon = "555-123-4567"
                }
            );

            modelBuilder.Entity<Kategori>().HasData(
                new Kategori
                {
                    Id = 1,
                    Adi = "Elektronik",
                    UstKategoriId = null
                },
                new Kategori
                {
                    Id = 2,
                    Adi = "Giyim",
                    UstKategoriId = null
                }
            );

            modelBuilder.Entity<Urun>().HasData(
                new Urun
                {
                    Id = 1,
                    Ad = "Akıllı Telefon",
                    Aciklama = "Son model akıllı telefon, 128GB depolama",
                    Fiyat = 999.99m,
                    Stok = 50,
                    ResimUrl = "/resimler/urunler/telefon.jpg",
                    KategoriId = 1,
                    SaticiProfilId = 1
                },
                new Urun
                {
                    Id = 2,
                    Ad = "Tişört",
                    Aciklama = "Pamuklu tişört, M beden",
                    Fiyat = 29.99m,
                    Stok = 100,
                    ResimUrl = "/resimler/urunler/tisort.jpg",
                    KategoriId = 2,
                    SaticiProfilId = 1
                }
            );
        }
    }
}