using System;
using System.Collections.Generic;
using System.Data;
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
using System.Globalization;


namespace Actividad_12.Usuario
{
    /// <summary>
    /// Lógica de interacción para PanelUsuario.xaml
    /// </summary>
    public partial class PanelUsuario : Window
    {
        SqlConnection miConexionSql;

        public PanelUsuario (SqlConnection conexionSql)
        {
            InitializeComponent();
            MessageBox.Show("Tienes Nivel Usuario : Estas entrando en un área en construcción");
            this.miConexionSql = conexionSql;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            
            MessageBox.Show("Te avisaremos cuando el área esté terminada");
            MainWindow newVovler = new MainWindow ();
            newVovler.Show();

            
            return;
        }
    }
}
