using FluentValidation;
using Models;
using Requests;
using Servicios;

namespace APIController.Validadores
{
    public class EdicionValidator : AbstractValidator<EdicionPostRequests>
    {
        public EdicionValidator()
        {
            RuleFor(edicion => edicion.Año).Must(AñoValido).WithMessage("Solo se admiten años entre 2020 y 2025");

            RuleFor(edicion => edicion.FechaFin).Must((edicion, fechaFin) => fechaFin > edicion.FechaInicio).WithMessage("La fecha de fin debe ser superior a fecha inicio")
                                                .Must((edicion, fechaFin) => ValidarDuracion(edicion.FechaInicio, fechaFin)).WithMessage("Un evento no puede durar más de un mes");
            //si hay una fecha de Inicio, no puede no haber una fecha de fin

            //si hay una fecha de fin, no puede no haber una fecha de inicio
        }

        public bool AñoValido(int año)
        {
            return año >= 2020 && año <= 2025;
        }

        public bool FechaFinValido(DateOnly fechaFin, DateOnly fechaInicio)
        {
            return fechaFin > fechaInicio;
        }

        public bool ValidarDuracion(DateOnly fechaIncio, DateOnly fechaFin)
        {
            if (fechaFin.CompareTo(fechaIncio) <= 30)
                { 
                return true;
                }
            else
                  {
                return false;
            }
            
        }
    }
}
