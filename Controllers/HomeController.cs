using Emgu.CV;
using Microsoft.AspNetCore.Mvc;
using RestIOS.Conexion;
using RestIOS.Models;
using RestIOS.ParametrosBody;
using static RestIOS.Clases.ValidaRostro;
using System.Data;

namespace RestIOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HomeController : Controller
    {
        // POST api/Datos
        [HttpPost]
        public IActionResult Home([FromBody] RegistraAsistencia registraAsistencia)
        {
            if (User == null)
            {
                return BadRequest();
            }

            // Aquí puedes agregar lógica para guardar el usuario etc.

            // Crear una instancia de DatosEmpleado
            DatosEmpleado empleado = new DatosEmpleado
            {
                empleado = "1234",
                nombre = "Juan Pérez",
                organismo = "GOB",
                foto_url = null,
                foto_base64 = registraAsistencia.valor,
                status = "A"
            };

            // Crear una instancia de Datos y asignar la instancia de DatosEmpleado
            Datos datos = new Datos
            {
                status = "0",
                resultado = "Éxito",
                usuario = "Admin",
                empleado = empleado // Asignar el objeto empleado
            };

            return Ok(new { datos }); //regresa el json con datos :{ }
            //return Ok( datos ); //regresa el json { }
        }
    }

}
