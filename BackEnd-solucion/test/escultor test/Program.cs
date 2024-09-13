using System;
using System.Data.Entity;
using System.Linq;
using EscultorModel;

namespace BienalConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instanciar el servicio de escultores
            var escultorService = new EscultorService();

            // 1. Crear un nuevo escultor
            for (int i=1; i<20; i++)
            {
                var nuevoEscultor = new Escultor
                {
                    Nombre = $"Juan{i}",
                    Apellido = "Pérez",
                    DNI = "12345678",
                    Email = "juanperez@mail.com",
                    Contraseña = "password123",
                    Telefono = "555-5555",
                    Biografia = "Escultor reconocido..."
                };
                var escultorCreado = escultorService.Create(nuevoEscultor);
                Console.WriteLine($"Escultor Creado: {escultorCreado.Nombre} {escultorCreado.Apellido}");
            }
  
            // 2. Obtener todos los escultores
            var escultores = escultorService.GetAll();
            Console.WriteLine("\nListado de Escultores:");
            foreach (var escultor in escultores)
            {
                Console.WriteLine($"{escultor.EscultorId}: {escultor.Nombre} {escultor.Apellido}");
            }

            // 3. Obtener un escultor por ID
            var escultorId = 5;
            var escultorObtenido = escultorService.GetById(escultorId);
            if (escultorObtenido != null)
            {
                Console.WriteLine($"\nEscultor Obtenido por ID: {escultorObtenido.Nombre} {escultorObtenido.Apellido}");
            }

            // 4. Actualizar el escultor
            escultorObtenido.Nombre = "Juan Carlos";
            var escultorActualizado = escultorService.Update(escultorObtenido);
            Console.WriteLine($"\nEscultor Actualizado: {escultorActualizado.Nombre} {escultorActualizado.Apellido}");

            // 5. Eliminar un escultor
            var escultorEliminado = escultorService.Delete(escultorId);
            if (escultorEliminado != null)
            {
                Console.WriteLine($"\nEscultor Eliminado: {escultorEliminado.Nombre} {escultorEliminado.Apellido}");
            }

            // Comprobar que el escultor fue eliminado
            escultores = escultorService.GetAll();
            Console.WriteLine("\nListado de Escultores después de la eliminación:");
            foreach (var escultor in escultores)
            {
                Console.WriteLine($"{escultor.EscultorId}: {escultor.Nombre} {escultor.Apellido}");
            }
        }
    }
}