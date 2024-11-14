using System.Collections.Generic;
using Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Contexts
{

    public class MyIdentityDBContext : IdentityDbContext<MyUser, MyRol, String>
    {
        public MyIdentityDBContext(DbContextOptions options) : base(options)
        {
            
        }
    }

}