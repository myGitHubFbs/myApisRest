using RestIOS.Models;

namespace RestIOS.ParametrosBody
{
    public class RegistraAsistencia
    {
        public string? usuario { get; set; }
        public string? dispositivo { get; set; }
        public string? valor { get; set; }
        public double latitud { get; set; }
        public double? longitud { get; set; }
        public string? tipo_medio { get; set; }
        public string? tipo_registro { get; set; }
    }

    public class AgregaFoto
    {
        public string? empleado { get; set; }
        public string? fotoBase64 { get; set; }
    }

}
