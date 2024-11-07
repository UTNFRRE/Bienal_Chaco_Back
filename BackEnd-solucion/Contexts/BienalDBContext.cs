using System.Collections.Generic;
using Entidades;
using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class BienalDbContext : DbContext
    {

        public DbSet<Escultores> Escultores { get; set; }
        public DbSet<Esculturas> Esculturas { get; set; }
        public DbSet<Eventos> Eventos { get; set; }

        public DbSet<MyUser> Users { get; set; }

        public BienalDbContext(DbContextOptions<BienalDbContext> options)
            : base(options)
        {
        }
        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        // InMemoryDatabase para pruebas, después cambiar a SQL Server
        //optionsBuilder.UseInMemoryDatabase("BienalDB");
        // optionsBuilder.UseSqlServer(@"Server=.\;Database=BienalDB;Trusted_Connection=True;");
        //}

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configuración de restricciones al crear la base de datos

            modelBuilder.Entity<Escultores>(entity =>
            {
                entity.Property(e => e.EscultorId)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre)
                    .HasMaxLength(30)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Apellido)
                    .HasMaxLength(30)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.DNI)
                    .HasMaxLength(10)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Pais)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .IsUnicode(false);

                entity.Property(e => e.Biografia)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Foto)
                    .IsUnicode(false);
            });

        }
    }
}
