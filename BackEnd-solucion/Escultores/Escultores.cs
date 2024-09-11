using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace EscultoresModel
{
    public class EscultoresContext : DbContext
    {
        // Se espera que esta clase represente el contexto de datos, es la encargada de gestionar el objeto y la conexión a la base de datos.
        // Aquí se pueden definir las tablas y configuraciones de la base de datos utilizando Entity Framework.
    }

    public class Escultores
    {
        // Se espera que esta clase represente un escultor individual.
        // Acá se definen las propiedades de escultor específico.
    }

    public class EscultoresServices
    {
        // Se espera que esta clase proporcione servicios relacionados con los escultores, conoce el contexto y es el intermediario entre la clase Escultores y la Base de Datos. 
        // Aquí se definen los métodos para realizar operaciones CRUD (crear, leer, actualizar, eliminar) en los escultores.
    }


}
