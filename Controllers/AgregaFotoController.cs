using Microsoft.AspNetCore.Mvc;
using RestIOS.Models;
using RestIOS.ParametrosBody;

namespace RestIOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AgregaFotoController : Controller
    {
        // POST api/Datos
        [HttpPost]
        public IActionResult AgregaFoto([FromBody] AgregaFoto agregarFoto)
        {
            if (User == null)
            {
                return BadRequest();
            }

            /*conexion.pConnectionString = _configuration.GetConnectionString("ConexionOracle")!;

            DataSet vDs = conexion.Consultar("select '" + datos.byte64!.Substring(1, 250) + "' byte64, " + datos.numero + " numero from dual", cDs, "myTabla");
            DataTable vDt = vDs.Tables["myTabla"]!;

            datos.byte64 = vDt.Rows[0]["byte64"].ToString();
            datos.numero = Convert.ToInt16(vDt.Rows[0]["numero"].ToString());*/

            // Aquí puedes agregar lógica para guardar el usuario etc.
            //return Ok(new { Mensaje = "Datos recibidos", datos });

            // Crear una instancia de DatosEmpleado
            DatosEmpleado empleado = new DatosEmpleado
            {
                empleado = "1234",
                nombre = "Juan Perez",
                organismo = "BUR",
                foto_url = "http://example.com/foto.jpg",
                foto_base64 = null,
                status = "A"
            };

            // Crear una instancia de Datos y asignar la instancia de DatosEmpleado
            Datos datos = new Datos
            {
                status = "0",
                resultado = "Éxito",
                usuario = "admin",
                empleado = empleado // Asignar el objeto empleado
            };

            //return Ok(new { datos }); //regresa el json con datos :{ }
            return Ok( datos ); //regresa el json { }
        }
    }

}