using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace RestIOS.Conexion
{
    public class DmlOracle
    {
        public string pConnectionString = "";

        public DataSet Consultar(string consulta, DataSet ds, string tabla, string xprefijo = "")
        {
            OracleCommand comando = new OracleCommand();
            OracleDataAdapter adapter = new OracleDataAdapter();
            OracleConnection conexion = new OracleConnection(pConnectionString); //new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ConexionOracle"].ConnectionString);
            DataTable dt = new DataTable();

            comando.CommandText = consulta;
            comando.Connection = conexion;
            conexion.Open();
            adapter.SelectCommand = comando;
            adapter.Fill(ds, tabla);
            conexion.Dispose();
            conexion.Close();
            OracleConnection.ClearPool(conexion);

            return ds;
        }
    }

}