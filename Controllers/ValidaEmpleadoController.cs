using Microsoft.AspNetCore.Mvc;
using RestIOS.Models;

namespace RestIOS.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ValidaEmpleadoController : Controller
    {
        [HttpGet("{empleado}")]
        public IActionResult ValidaEmpleado(string parametroEmpelado)
        {
 
            // Crear una instancia de DatosEmpleado
            DatosEmpleado empleado = new DatosEmpleado
            {
                empleado = parametroEmpelado,
                nombre = "Juan Perez",
                organismo = "BUR",
                foto_url = "http://example.com/foto.jpg",
                foto_base64 = null,
                status = "A"
            };

            // Crear una instancia de Datos
            Datos datos = new Datos
            {
                status = "0",
                resultado = "Éxito",
                usuario = "admin",
                empleado = empleado // Asignar el objeto empleado
            };

            return Ok(new { datos }); //regresa el json con datos :{ }
            //return Ok( datos ); //regresa el json { }
        }
    }
}
