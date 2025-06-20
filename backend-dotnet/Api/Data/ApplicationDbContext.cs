using Microsoft.EntityFrameworkCore;
using Api.Entities;
using Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tasinmazdb;Username=tasinmazdbkullanici;Password=supersifre123;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Il>(entity =>
            {
                entity.HasIndex(e => new { e.IlAdi })
                    .IsUnique()
                    .HasDatabaseName("IX_Il_IlAdi");
            });
            modelBuilder.Entity<Ilce>(entity =>
            {
                entity.HasIndex(e => new { e.IlceAdi, e.IlId })
                    .IsUnique()
                    .HasDatabaseName("IX_Ilce_IlceAdi_IlId");
            });
            modelBuilder.Entity<Mahalle>(entity =>
            {
                entity.HasIndex(e => new { e.MahalleAdi, e.IlceId })
                    .IsUnique()
                    .HasDatabaseName("IX_Mahalle_MahalleAdi_IlceId");
            });
            // modelBuilder.Entity<Kullanici>().HasAlternateKey(e => e.Email);
            modelBuilder.Entity<Tasinmaz>(entity =>
            {
                entity.HasIndex(e => new { e.Ada, e.Parsel, e.Nitelik, e.KoordinatBilgileri, e.MahalleId, e.KullaniciId }).IsUnique();
            });
        }
        public DbSet<Il> Iller { get; set; }
        public DbSet<Ilce> Ilceler { get; set; }
        public DbSet<Mahalle> Mahalleler { get; set; }
        public DbSet<Tasinmaz> Tasinmazlar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Log> Loglar { get; set; }
        public DbSet<Durum> Durumlar { get; set; }
        public DbSet<IslemTip> IslemTipleri { get; set; }

        
    }
}