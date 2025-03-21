namespace RestIOS.Models
{
    public class Datos
    {
        public string? status { get; set; }
        public string? resultado { get; set; }
        public string? usuario { get; set; }
        public DatosEmpleado? empleado { get; set; }
    }

    public class DatosEmpleado
    {
        public string? empleado { get; set; }
        public string? nombre { get; set; }
        public string? organismo { get; set; }
        public string? foto_url { get; set; }
        public string? foto_base64 { get; set; }
        public string? status { get; set; }
    }

}