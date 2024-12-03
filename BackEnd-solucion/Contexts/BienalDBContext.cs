using System.Collections.Generic;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Contexts
{
    public class BienalDbContext : DbContext
    {

        public DbSet<Escultores> Escultores { get; set; }
        public DbSet<Esculturas> Esculturas { get; set; }
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<Imagen> Imagenes { get; set; }

        
        public DbSet<Votos> Votos { get; set; }
        public DbSet<Edicion> Edicion { get; set; }

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

            base.OnModelCreating(modelBuilder);
            // Configuración de la relación entre Escultura e Imagen (uno a muchos)
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Escultura)
                .WithMany(e => e.Imagenes)
                .HasForeignKey(i => i.EsculturaId);

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

