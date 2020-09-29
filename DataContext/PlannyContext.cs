using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Planny.Data.Models;

namespace Planny.DataContext
{
    public partial class PlannyContext : DbContext
    {
        public PlannyContext()
        {
        }

        public PlannyContext(DbContextOptions<PlannyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<StatusZadatka> StatusZadatka { get; set; }
        public virtual DbSet<Zadatak> Zadatak { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=BOJAN\\DLS;Database=Planny;Integrated Security=SSPI;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>(entity =>
            {
                entity.HasIndex(e => e.KorisnickoIme)
                    .HasName("UQ__Korisnik__992E6F9284DAB6BC")
                    .IsUnique();

                entity.HasIndex(e => e.LozinkaHash)
                    .HasName("UQ__Korisnik__5AC99AA118345301")
                    .IsUnique();

                entity.HasIndex(e => e.Salt)
                    .HasName("UQ__Korisnik__A152BCCE9FF22341")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.DatumRegistracije).HasColumnType("datetime");

                entity.Property(e => e.DatumRodenja).HasColumnType("datetime");

                entity.Property(e => e.Ime)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.KorisnickoIme)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LozinkaHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Prezime).HasMaxLength(100);

                entity.Property(e => e.Salt)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<StatusZadatka>(entity =>
            {
                entity.HasIndex(e => e.Naziv)
                    .HasName("UQ__StatusZa__603E814610020882")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Opis).HasMaxLength(100);
            });

            modelBuilder.Entity<Zadatak>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.KorisnikId).HasColumnName("KorisnikID");

                entity.Property(e => e.Kraj).HasColumnType("datetime");

                entity.Property(e => e.Naziv)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Opis).HasMaxLength(100);

                entity.Property(e => e.Pocetak).HasColumnType("datetime");

                entity.Property(e => e.StatusZadatkaId).HasColumnName("StatusZadatkaID");

                entity.HasOne(d => d.Korisnik)
                    .WithMany(p => p.Zadatak)
                    .HasForeignKey(d => d.KorisnikId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zadatak__Korisni__300424B4");

                entity.HasOne(d => d.StatusZadatka)
                    .WithMany(p => p.Zadatak)
                    .HasForeignKey(d => d.StatusZadatkaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Zadatak__StatusZ__2F10007B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Planny.Data.ViewModels.KorisnikDTO> KorisnikVM { get; set; }
    }
}
