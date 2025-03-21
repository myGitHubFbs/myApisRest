using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using Microsoft.AspNetCore.Mvc;
using RestIOS.Conexion;
using RestIOS.Models;
using RestIOS.ParametrosBody;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using static RestIOS.Clases.ValidaRostro;

namespace RestIOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegistraAsistenciaController : Controller
    {
        private readonly IConfiguration _configuration;

        public RegistraAsistenciaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        DmlOracle conexion = new DmlOracle();
        DataSet cDs = new DataSet();
        DataTable cDt = new DataTable();

        // POST api/Datos
        [HttpPost]
        public IActionResult RegistraAsistencia([FromBody] RegistraAsistencia registraAsistencia)
        {
            Crostro objetoValidar = new Crostro();

            // Crear una instancia de DatosEmpleado
            DatosEmpleado empleado = new DatosEmpleado
            {
                empleado = null,
                nombre = null,
                organismo = null,
                foto_url = null,
                foto_base64 = null,
                status = null
            };

            // Crear una instancia de Datos
            Datos datos = new Datos
            {
                status = null,
                resultado = null,
                usuario = null,
                empleado = null
            };

            try
            {
                //string status = "0";
                //string resultado = "";

                if (User == null)
                {
                    return BadRequest();
                }

                // Aquí puedes agregar lógica para guardar el usuario etc.

                var facialRecognition = new FacialRecognition();

                // String Base64 (esto debe ser una cadena Base64 válida)
                string rutaImage1 = facialRecognition.ConvertBase64ToArchivo(registraAsistencia.valor!);

                if (rutaImage1 != "Error")
                {
                    // Convertir las imágenes Base64 a objetos Mat
                    Mat image1 = facialRecognition.ConvertBase64ToMat(rutaImage1);

                    // Validar rostros                    
                    objetoValidar = facialRecognition.ValidaRostro(image1);

                    //Datos Oracle
                    if (objetoValidar.status == "0")
                    {
                        conexion.pConnectionString = _configuration.GetConnectionString("ConexionOracle")!;

                        DataSet vDs = conexion.Consultar("select empl.empleado, trim(empl.nombre || ' ' || empl.apellido_paterno || ' ' || empl.apellido_materno) nombre, empl.organismo, empl.status, area.descripcion area, ep.descripcion entidad, ei.imagen " +
                                                        "from empleado empl, " +
                                                        "plaza_empleado plem, " +
                                                        "entidad_publica ep, " +
                                                        "area_2_Vw area, " +
                                                        "empleado_imagen ei " +
                                                        "where empl.empleado = " + objetoValidar.archivo +
                                                        " and empl.organismo = plem.organismo " +
                                                        "and empl.empleado = plem.empleado " +
                                                        "and plem.status = 'A' " +
                                                        "and plem.fecha_efectiva = plaza_empleado_fe(plem.empleado, plem.registro, sysdate) and plem.secuencia = plaza_empleado_sc(plem.empleado, plem.registro, plem.fecha_efectiva) " +
                                                        "and ep.organismo = empl.organismo and ep.entidad_publica = plem.entidad_publica and ep.fecha_efectiva = entidad_publica_fe(plem.organismo, plem.entidad_publica, sysdate) " +
                                                        "and area.usuario_ap = 'NFCEDILL' and area.status = 'A' and area.fecha_efectiva = area_fe(area.organismo, area.area, sysdate) and area.area = plem.area and area.organismo = empl.organismo " +
                                                        "and ei.empleado = empl.empleado and ei.tipo_imagen = 'FO' and ei.fecha_efectiva = empleado_imagen_fe(ei.empleado, ei.tipo_imagen, sysdate)", cDs, "myTabla");

                        DataTable vDt = vDs.Tables["myTabla"]!;

                        byte[] imgdata = (byte[])vDs.Tables["myTabla"]!.Rows[0]["imagen"];

                        string base64String = Convert.ToBase64String(imgdata);

                        empleado = new DatosEmpleado
                        {
                            empleado = vDt.Rows[0]["empleado"].ToString(),
                            nombre = vDt.Rows[0]["nombre"].ToString(),
                            organismo = vDt.Rows[0]["organismo"].ToString(),
                            foto_url = "",
                            foto_base64 = base64String,
                            status = vDt.Rows[0]["status"].ToString()
                        };

                        // Mostrar el resultado
                        string rutaImageregreso = facialRecognition.ConvertBase64ToArchivo(base64String!);
                        Mat imageRegreso = facialRecognition.ConvertBase64ToMat(rutaImageregreso);

                        CvInvoke.Imshow("Imagen", imageRegreso);
                        CvInvoke.WaitKey(0);
                        CvInvoke.DestroyAllWindows();

                        System.IO.File.Delete(rutaImageregreso);
                    }

                    /*
                    // Detectar rostros en ambas imágenes
                    Rectangle face1 = facialRecognition.DetectFace(image1);
                    Rectangle face2 = facialRecognition.DetectFace(image2);

                    if (!face1.IsEmpty && !face2.IsEmpty)
                    {
                        // Extraer los rostros de ambas imágenes                    
                        Mat faceImage1 = facialRecognition.ExtractFace(image1, face1);
                        Mat faceImage2 = facialRecognition.ExtractFace(image2, face2);
                        //Las caras han sido detectadas y extraídas

                        // Comparar las caras
                        bool facesAreSimilar = facialRecognition.CompareFaces(faceImage1, faceImage2);

                        if (facesAreSimilar)
                        {
                            status = "0";
                            resultado = "La cara coincide con la del archivo: " + Path.GetFileName(archivo).Substring(0, Path.GetFileName(archivo).LastIndexOf("."));

                            break;
                        }
                        else
                        {
                            status = "1";
                            resultado = "Ninguna cara coincide.";
                        }
                    }
                    else
                    {
                        status = "1";
                        resultado = "No se detectaron rostros en una o ambas imágenes.";
                    }*/
                }
                else
                {
                    objetoValidar.status = "1";
                    objetoValidar.resultado = "Parámetro valor no corresponde al tipo byte64.";
                }

                System.IO.File.Delete(rutaImage1);

                //Asignar la instancia de DatosEmpleado
                datos = new Datos
                {
                    status = objetoValidar.status,
                    resultado = objetoValidar.resultado,
                    usuario = registraAsistencia.usuario,
                    empleado = empleado // Asignar el objeto empleado
                };
            }
            catch(Exception ex) {
                objetoValidar.status = "1";
                objetoValidar.resultado = ex.Message;
            }

            return Ok(new { datos }); //regresa el json con datos :{ }
            //return Ok( datos ); //regresa el json { }
        }
    }

}