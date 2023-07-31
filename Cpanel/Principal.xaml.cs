using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Actividad_12.Cpanel
{
    /// <summary>
    /// Lógica de interacción para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        SqlConnection miConexionSql;

        public Principal (SqlConnection conexionSql)
        {
            InitializeComponent();
            this.miConexionSql = conexionSql;
        }

        private void btn_doctor_Click(object sender, RoutedEventArgs e)
        {

            this.Hide();

            Cpanel.Doctor NewDoctor = new Cpanel.Doctor (miConexionSql);
            NewDoctor.Show();
        }

        private void btn_paciente_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Cpanel.Paciente NewPaciente = new Cpanel.Paciente(miConexionSql);
            NewPaciente.Show();
        }

        private void btn_volver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow vovler = new MainWindow ();
            vovler.Show();
        }

        private void btn_tratamiento_Click(object sender, RoutedEventArgs e)
        {
            Cpanel.Tratamientonew newTratamiento = new Cpanel.Tratamientonew(miConexionSql);
            newTratamiento.Show();
            this.Close();
        }

        private void btn_consulta_Click(object sender, RoutedEventArgs e)
        {
            Cpanel.Consulta newConsulta = new Cpanel.Consulta(miConexionSql);
            newConsulta.Show();
            this.Close();
        }

        private void btn_hospitalizacion_Click(object sender, RoutedEventArgs e)
        {
            Cpanel.Hospitalizacion newHospitalizacion = new Cpanel.Hospitalizacion(miConexionSql);
            newHospitalizacion.Show();
            this.Close();
        }
    }
}
