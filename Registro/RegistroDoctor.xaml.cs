using Microsoft.Win32;
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

namespace Actividad_12.Registro
{
    /// <summary>
    /// Lógica de interacción para RegistroDoctor.xaml
    /// </summary>
    public partial class RegistroDoctor : Window
    {
        SqlConnection miConexionSql;
        
        public RegistroDoctor(SqlConnection conexionSql)
        {
            InitializeComponent();
            this.miConexionSql = conexionSql;
        }

        private void nuevoRegistro()
        {
            string consulta = "INSERT INTO Usuario (Nombre,Apellido1,Apellido2,Nif,Usuario,Password,Rol) VALUES(@Nombre,@Apellido1,@Apellido2,@Nif,@Usuario,@Password,@Rol)";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            miConexionSql.Open();

            comandoSql.Parameters.AddWithValue("@Nombre", txbNombre.Text);
            comandoSql.Parameters.AddWithValue("@Apellido1", txbApellido1.Text);
            comandoSql.Parameters.AddWithValue("@Apellido2", txbApellido2.Text);
            comandoSql.Parameters.AddWithValue("@Nif", txbNif.Text);
            comandoSql.Parameters.AddWithValue("@Usuario", txbUsuario.Text);
            comandoSql.Parameters.AddWithValue("@Password", txbPassword.Password);
            comandoSql.Parameters.AddWithValue("@Rol", cbRol.Text);

            comandoSql.ExecuteNonQuery();

            miConexionSql.Close();

            txbNombre.Text = "";
            txbApellido1.Text = "";
            txbApellido2.Text = "";
            txbNif.Text = "";
            txbUsuario.Text = "";
            txbPassword.Password = "";
            txbConfPassword.Password = "";
            cbRol.Text = "";

            MessageBox.Show("El usuario se ha creado correctamente");

            this.Close();
            MainWindow login = new MainWindow();
            login.Show();
        }


        private bool usuarioExistente()
        {
            bool existe = false;

            string consulta = "SELECT Usuario, Password FROM Usuario";

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, miConexionSql);

            using (sqlDataAdapter)
            {

                DataTable datosUsuario = new DataTable();

                sqlDataAdapter.Fill(datosUsuario);

                for (int i = 0; i < datosUsuario.Rows.Count; i++)
                {
                    if (datosUsuario.Rows[i]["Usuario"].ToString() == txbUsuario.Text && datosUsuario.Rows[i]["Password"].ToString() == txbPassword.Password)
                    {
                        existe = true;
                        
                    }
                }

                return existe;
            }
        }


        private void btnRegistro_Click(object sender, RoutedEventArgs e)
        {
            if (txbPassword.Password != txbConfPassword.Password)
            {
                MessageBox.Show("Las contraseñas deben ser iguales");
                return;
            }

            if (usuarioExistente())
            {
                MessageBox.Show("El usuario ya existe");
                txbApellido1.Clear();
                txbApellido2.Clear();
                txbNombre.Clear();
                txbNif.Clear();
                txbPassword.Clear();
                txbConfPassword.Clear();
                txbUsuario.Clear();
                cbRol.SelectedIndex = 0;
                return;
            }

            if (string.IsNullOrWhiteSpace(txbNombre.Text) || string.IsNullOrWhiteSpace(txbApellido1.Text) || string.IsNullOrWhiteSpace(txbApellido2.Text) || string.IsNullOrWhiteSpace(txbNif.Text) || string.IsNullOrWhiteSpace(txbUsuario.Text) || string.IsNullOrWhiteSpace(txbPassword.Password))
            {
                MessageBox.Show("Por favor rellene todos los campos");
                return;
            }

            nuevoRegistro();
        }



        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            //OCULTAR LA VENTANA ACTUAL
            this.Hide();

            //ABRIR NUEVA VENTANA

            MainWindow Ventana = new MainWindow();
            Ventana.Show();
        }

        private void BtnSeleccionImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de imagen (*.png;*.jpg)|*.png;*.jpg|Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                imagenSeleccionada.Source = new BitmapImage(new Uri(filePath));
                //Aquí puedes mostrar la imagen seleccionada en un control de imagen si lo deseas
            }
        }
    }
}
