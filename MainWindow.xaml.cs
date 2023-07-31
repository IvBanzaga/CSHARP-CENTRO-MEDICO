using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace Actividad_12
{

    public partial class MainWindow : Window
    {
        SqlConnection miConexionSql;
        int IdUsuario;
        public MainWindow()
        {
            InitializeComponent();

            TxtUser.Focus();

            /*conexion base de datos*/

            /*string miConexion = ConfigurationManager.ConnectionStrings["Actividad_12.Properties.Settings.HospitalDBConnectionString"].ConnectionString;

            miConexionSql = new SqlConnection(miConexion);*/

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();


            miConexionSql = new SqlConnection(miConexion);




        }

        /* mover la ventana sin necesidad de hacer clic en el título de la ventana.*/

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private bool login(string usuario, string password)
        {
            bool existe = false;


            string consulta = "SELECT Usuario,Password,Id, Rol FROM Usuario";

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);



            using (adaptadorSql)
            {

                DataTable datosUsuario = new DataTable();

                adaptadorSql.Fill(datosUsuario);

                for (int i = 0; i < datosUsuario.Rows.Count; i++)
                {
                    string rol = datosUsuario.Rows[i]["ROL"].ToString();

                    if (datosUsuario.Rows[i]["USUARIO"].ToString() == TxtUser.Text && datosUsuario.Rows[i]["PASSWORD"].ToString() == TxtPass.Password)
                    {
                        existe = true;
                        IdUsuario = (int)datosUsuario.Rows[i]["Id"];
                    }
                }

                return existe;

            }

        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUser.Text == "admin" && TxtPass.Password == "admin")
            {
                // Si el usuario es administrador, muestra la ventana de administrador
                Cpanel.Principal newCpanel = new Cpanel.Principal(miConexionSql);
                newCpanel.Show();
                this.Close();
            }
            else if (login(TxtUser.Text, TxtPass.Password))
            {
                // Obtén el rol del usuario autenticado
                string rol = GetRolUsuario(TxtUser.Text);

                if (rol.Equals("Usuario"))
                {
                    // Si el usuario tiene el rol de usuario normal, muestra la ventana correspondiente
                    Usuario.PanelUsuario newUsuario = new Usuario.PanelUsuario(miConexionSql);
                    newUsuario.Show();
                    this.Close();
                }
                else if (rol.Equals("Administrador"))
                {
                    // Si el usuario tiene el rol de administrador, muestra la ventana correspondiente
                    Cpanel.Principal newCpanel = new Cpanel.Principal(miConexionSql);
                    newCpanel.Show();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("El usuario o la contraseña no son correctos");
            }


        }

        private string GetRolUsuario(string usuario)
        {
            string rol = "";

            string consulta = "SELECT ROL FROM Usuario WHERE USUARIO = @Usuario";

            /*using (SqlConnection conexion = new SqlConnection("Data Source=DESKTOP-DJ6QD24;Initial Catalog=HospitalDB;Integrated Security=True"))
            {*/
            using (SqlCommand comando = new SqlCommand(consulta, miConexionSql))
            {
                comando.Parameters.AddWithValue("@Usuario", usuario);

                try
                {
                    miConexionSql.Open();
                    SqlDataReader lector = comando.ExecuteReader();

                    if (lector.Read())
                    {
                        rol = lector["ROL"].ToString();
                    }

                    lector.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al obtener el rol del usuario: " + ex.Message);
                }
            }


            return rol;
        }


        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            //OCULTAR LA VENTANA ACTUAL
            this.Hide();

            //ABRIR NUEVA VENTANA

            Registro.RegistroDoctor Ventana = new Registro.RegistroDoctor(miConexionSql);
            Ventana.Show();

        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación de Iván Bazaga ");
            App.Current.Shutdown();
        }

        private void CerrarVentana(object sender, System.ComponentModel.CancelEventArgs e)
        {
                // Si hay cambios sin guardar, mostrar un cuadro de diálogo de confirmación
                /*MessageBoxResult result = MessageBox.Show("¿Deseas guardar los cambios antes de cerrar?", "Guardar cambios", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }

                else (result == MessageBoxResult.Cancel)
                {
                    // Si el usuario cancela el cierre, cancelar el evento de cierre de la ventana
                    e.Cancel = true;
                }*/
            }
        }
    }
