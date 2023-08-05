using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actividad_12
{
    internal class miConexion
    {
        SqlConnection miConexionSql;
        public miConexion() { }

        public string crearConexion () 
        {
            return ConfigurationManager.ConnectionStrings["Actividad_12.Properties.Settings.HospitalDBConnectionString"].ConnectionString;

           
        }


    }
}
