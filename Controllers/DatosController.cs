using Microsoft.AspNetCore.Mvc;
using RestIOS.Conexion;
using RestIOS.Models;
using System.Data;

namespace RestIOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DatosController : Controller
    {
        private readonly IConfiguration _configuration;

        public DatosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DmlOracle conexion = new DmlOracle();
        DataSet cDs = new DataSet();
        DataTable cDt = new DataTable();

        // POST api/Datos
        [HttpPost]
        public IActionResult Post([FromBody] Datos datos)
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
            return Ok(new { Mensaje = "Datos recibidos", datos });
        }

    }
}
