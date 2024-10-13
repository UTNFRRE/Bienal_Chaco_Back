using System.Collections.Generic;
using Entidades;
using Microsoft.EntityFrameworkCore;

namespace Contexts
{
    public class BienalDbContext : DbContext
    {

        // public DbSet<Escultores> Escultores { get; set; }
        public DbSet<Esculturas> Esculturas { get; set; }
        public DbSet<Eventos> Eventos { get; set; }

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

        };

      
    }


