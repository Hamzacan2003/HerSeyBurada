using Microsoft.EntityFrameworkCore;
using HerSeyBurada.Core.Entities;

namespace HerSeyBurada.DataAccess.Concrete
{
    public class HerSeyBuradaDbContext : DbContext
    {
        public HerSeyBuradaDbContext(DbContextOptions<HerSeyBuradaDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Magaza> Magazalar { get; set; }
        public DbSet<Adres> Adresler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<UrunVaryant> UrunVaryantlari { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<Kupon> Kuponlar { get; set; }
        public DbSet<UrunYorum> UrunYorumlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Precision Ayarları (Para Ve Oran Birimleri)
            modelBuilder.Entity<Urun>().Property(u => u.PiyasaFiyati).HasPrecision(18, 2);
            modelBuilder.Entity<Urun>().Property(u => u.SatisFiyati).HasPrecision(18, 2);
            modelBuilder.Entity<UrunVaryant>().Property(uv => uv.EkFiyat).HasPrecision(18, 2);
            modelBuilder.Entity<Siparis>().Property(s => s.ToplamTutar).HasPrecision(18, 2);
            modelBuilder.Entity<Siparis>().Property(s => s.KargoTutari).HasPrecision(18, 2);
            modelBuilder.Entity<Siparis>().Property(s => s.IndirimTutari).HasPrecision(18, 2);
            modelBuilder.Entity<SiparisDetay>().Property(sd => sd.BirimFiyat).HasPrecision(18, 2);
            modelBuilder.Entity<Kupon>().Property(k => k.IndirimTutari).HasPrecision(18, 2);
            modelBuilder.Entity<Kupon>().Property(k => k.MinimumSepetTutari).HasPrecision(18, 2);
            modelBuilder.Entity<Magaza>().Property(m => m.KomisyonOrani).HasPrecision(5, 2);

            // İlişkiler Ve Silme Kuralları (Delete Behavior)
            modelBuilder.Entity<Magaza>()
                .HasOne(m => m.Kullanici)
                .WithOne(k => k.Magaza)
                .HasForeignKey<Magaza>(m => m.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SiparisDetay>()
                .HasOne(sd => sd.Siparis)
                .WithMany(s => s.SiparisDetaylari)
                .HasForeignKey(sd => sd.SiparisId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}