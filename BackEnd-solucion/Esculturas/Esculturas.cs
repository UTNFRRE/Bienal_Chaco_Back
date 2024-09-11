using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace EsculturasModel
{
    public class EsculturasContext : DbContext
    {
        // Se espera que esta clase represente el contexto de datos, es la encargada de gestionar el objeto y la conexión a la base de datos.
        // Aquí se pueden definir las tablas y configuraciones de la base de datos utilizando Entity Framework.
    }

    public class EsculturasModel
    {
        // Se espera que esta clase represente una escultura individual.
        // Acá se definen las propiedades de la escultura específica.
    }

    public class EsculturasServices
    {
        // Se espera que esta clase proporcione servicios relacionados con las esculturas, conoce el contexto y es el intermediario entre la clase Esculturas y la Base de Datos. 
        // Aquí se definen los métodos para realizar operaciones CRUD (crear, leer, actualizar, eliminar) en las esculturas.
    }


}
