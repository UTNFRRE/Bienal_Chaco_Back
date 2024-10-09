using System.Collections.Generic;
using Entidades;
using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class BienalDbContext : DbContext
    {

        // public DbSet<Escultores> Escultores { get; set; }
        public DbSet<Esculturas> Esculturas { get; set; }
        // public DbSet<Eventos> Eventos { get; set; }

        public BienalDbContext(DbContextOptions<BienalDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // InMemoryDatabase para pruebas, después cambiar a SQL Server
            optionsBuilder.UseInMemoryDatabase("BienalDB");
            // optionsBuilder.UseSqlServer(@"Server=.\;Database=BienalDB;Trusted_Connection=True;");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configuración de restricciones al crear la base de datos
            modelBuilder.Entity<Esculturas>(entity =>
            {
                //genera solo la id
                entity.Property(e => e.EsculturaId)
                    .ValueGeneratedOnAdd();


                entity.Property(e => e.Nombre)
                    .HasMaxLength(20)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.EscultorID)
                    .IsRequired();

                entity.Property(e => e.EventoID)
                    .IsRequired();

                entity.Property(e => e.Imagenes)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }

      
    }

}
