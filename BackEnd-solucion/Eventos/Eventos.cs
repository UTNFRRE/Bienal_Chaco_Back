using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace EventosModel
{
    public class EventosContext : DbContext
    {
        // Se espera que esta clase represente el contexto de datos, es la encargada de gestionar el objeto y la conexión a la base de datos.
        // Aquí se pueden definir las tablas y configuraciones de la base de datos utilizando Entity Framework.
    }

    public class Eventos
    {
        // Se espera que esta clase represente un evento individual.
        // Acá se definen las propiedades del evento específico.
    }

    public class EventosServices
    {
        // Se espera que esta clase proporcione servicios relacionados con los eventos, conoce el contexto y es el intermediario entre la clase Eventos y la Base de Datos. 
        // Aquí se definen los métodos para realizar operaciones CRUD (crear, leer, actualizar, eliminar) en los eventos.
    }
}
