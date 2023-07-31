using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Actividad_12.Cpanel
{
    public partial class Paciente : Window
    {
        SqlConnection miConexionSql;

        public Paciente(SqlConnection conexionSql)
        {
            InitializeComponent();
            this.miConexionSql = conexionSql;

            /*---- CLASE MI CONEXION CON LOS DATOS DE CONEXION A BASE DE DATOS ******/

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();

            miConexionSql = new SqlConnection(miConexion);

            MuestraPaciente();
        }


        /* RESULTADO EN UNA LISTA DE PACIENTES*/

        private void MuestraPaciente()
        {
            string consulta = "SELECT Nombre, ID FROM PACIENTE";
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
            using (adaptadorSql)
            {
                DataTable doctorTabla = new DataTable();
                adaptadorSql.Fill(doctorTabla);

                ListaPacientes.DisplayMemberPath = "Nombre";
                ListaPacientes.SelectedValuePath = "ID";
                ListaPacientes.ItemsSource = doctorTabla.DefaultView;
            }


        }

        /*MUESTRA DATOS  PACIENTTES*/

        private void MuestraDatosPaciente()

        {
            try
            {
                string consulta = "SELECT *, CONCAT (ID, ' - ',NOMBRE, ' ' ,APELLIDO1, ' ' ,APELLIDO2, ' ' , TELEFONO, ' ' ,EMAIL) as infoPaciente FROM PACIENTE WHERE ID = @idPaciente";

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                    sqlComando.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                    DataTable doctorTabla = new DataTable();
                    miAdaptadorSql.Fill(doctorTabla);

                    DatosPacientes.DisplayMemberPath = "infoPaciente";
                    DatosPacientes.SelectedValuePath = "ID";
                    DatosPacientes.ItemsSource = doctorTabla.DefaultView;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar los datos del paciente: " + ex.Message);
            }
        }


        /**** METODO EN PROCESO DE CONSTRUCCION PARA AGREGAR UNA IMAGEN ****/

        private void BtnSeleccionImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de imagen (*.png;*.jpg)|*.png;*.jpg|Todos los archivos (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                imagenSeleccionada.Source = new BitmapImage(new Uri(filePath));
 
            }
        }

        /**** SELECCIONAR PARA ACTUALIZAR ****/

        private void Btn_actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar que haya un paciente seleccionado en ListaPacientes
                if (ListaPacientes.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un paciente para actualizar.");
                    return;
                }

                string consulta = "SELECT ID,NOMBRE,APELLIDO1,APELLIDO2,FECHA_NACIMIENTO,TELEFONO,EMAIL FROM PACIENTE WHERE ID = @idPaciente";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

                using (miAdaptadorSql)
                {
                    miSqlCommand.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                    DataTable pacientesTabla = new DataTable();

                    miAdaptadorSql.Fill(pacientesTabla);

                    txbNombre.Text = pacientesTabla.Rows[0]["NOMBRE"].ToString();
                    txbApellido1.Text = pacientesTabla.Rows[0]["APELLIDO1"].ToString();
                    txbApellido2.Text = pacientesTabla.Rows[0]["APELLIDO2"].ToString();
                    F_nacimiento.Text = pacientesTabla.Rows[0]["FECHA_NACIMIENTO"].ToString();
                    txbTelefono.Text = pacientesTabla.Rows[0]["TELEFONO"].ToString();
                    txbEmail.Text = pacientesTabla.Rows[0]["EMAIL"].ToString();
                    Id_Paciente.Text = pacientesTabla.Rows[0]["ID"].ToString();

                    MessageBox.Show("Se van actualizar los datos del PACIENTE, después de cambiar los datos púlsa modificar. Gracias por útilizar la aplicación");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Actualizar PACIENTE: " + ex.Message);
            }


        }

        /**** MODIFICAR ****/
        private void Btn_modifica_Click(object sender, RoutedEventArgs e)
        {

            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "UPDATE PACIENTE SET Nombre=@Nombre, Apellido1=@Apellido1, Apellido2=@Apellido2, Telefono=@Telefono, Email=@Email, ID=@IDPACIENTE WHERE ID=@IDPACIENTE";
            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            miSqlCommand.Parameters.AddWithValue("@Nombre", txbNombre.Text);
            miSqlCommand.Parameters.AddWithValue("@Apellido1", txbApellido1.Text);
            miSqlCommand.Parameters.AddWithValue("@Apellido2", txbApellido2.Text);
            miSqlCommand.Parameters.AddWithValue("@Telefono", txbTelefono.Text);
            miSqlCommand.Parameters.AddWithValue("@Email", txbEmail.Text);

            miSqlCommand.Parameters.AddWithValue("@IDPACIENTE", Id_Paciente.Text);

            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();

            MessageBox.Show("Se modificaron los datos correctamente. Gracias por utilizar la aplicación");
            // Limpiar los campos de entrada después de agregar un nuevo registro
            Id_Paciente.Clear();
            txbNombre.Clear();
            txbApellido1.Clear();
            txbApellido2.Clear();
            F_nacimiento.SelectedDate = null;
            txbEmail.Clear();
            txbTelefono.Clear();

            MuestraDatosPaciente();
            MuestraPaciente();
            DatosPacientes.ItemsSource = null;
        }



        /**** INSERTAR ****/
        private void Btn_insertar_Click(object sender, RoutedEventArgs e)
        {

            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar que todos los campos obligatorios estén llenos
            if (string.IsNullOrEmpty(Id_Paciente.Text) || string.IsNullOrEmpty(txbNombre.Text) || string.IsNullOrEmpty(txbApellido1.Text) || F_nacimiento.SelectedDate == null || string.IsNullOrEmpty(txbTelefono.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios antes de insertar.");
                return;
            }

            string consulta = "INSERT INTO PACIENTE (ID, NOMBRE, APELLIDO1, APELLIDO2, FECHA_NACIMIENTO, TELEFONO, EMAIL) VALUES (@ID, @Nombre, @Apellido1, @Apellido2, @FNacimiento, @Telefono, @Email)";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            miSqlCommand.Parameters.AddWithValue("@ID", Id_Paciente.Text);
            miSqlCommand.Parameters.AddWithValue("@Nombre", txbNombre.Text);
            miSqlCommand.Parameters.AddWithValue("@Apellido1", txbApellido1.Text);
            miSqlCommand.Parameters.AddWithValue("@Apellido2", txbApellido2.Text);
            miSqlCommand.Parameters.AddWithValue("@FNacimiento", F_nacimiento.SelectedDate);
            miSqlCommand.Parameters.AddWithValue("@Telefono", txbTelefono.Text);
            miSqlCommand.Parameters.AddWithValue("@Email", txbEmail.Text);

            miSqlCommand.ExecuteNonQuery();

            miConexionSql.Close();

            MuestraPaciente();


            MessageBox.Show("Bienvenido, te has dado de alta como PACIENTE en la aplicación de Iván Bazaga");
            // Limpiar los campos de entrada después de agregar un nuevo registro
            Id_Paciente.Clear();
            txbNombre.Clear();
            txbApellido1.Clear();
            txbApellido2.Clear();
            F_nacimiento.SelectedDate = null;
            txbEmail.Clear();
            txbTelefono.Clear();
            DatosPacientes.ItemsSource = null;

        }

        /**** BORRAR ****/
        private void Btn_borrar_Click(object sender, RoutedEventArgs e)
        {
            if (ListaPacientes.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un Paciente para borrar.");
                return;
            }
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "DELETE FROM PACIENTE WHERE ID=@ID";

            SqlCommand miSqlComand = new SqlCommand(consulta, miConexionSql);

            miSqlComand.Parameters.AddWithValue("@ID", ListaPacientes.SelectedValue);

            miSqlComand.ExecuteNonQuery();

            miConexionSql.Close();

            /*string seleccion = ((DataRowView)ListaPacientes.SelectedItem).Row.ItemArray[0].ToString();
            string consultaTratamiento = "DELETE FROM PACIENTE WHERE ID=" + seleccion + ";"; // sintaxis para el borrado del tratamiento
            SqlCommand miSqlCommand = new SqlCommand(consultaTratamiento, miConexionSql);
            miConexionSql.Open();
            miSqlCommand.ExecuteNonQuery();
            miConexionSql.Close();*/

            MuestraPaciente();
            MessageBox.Show("Se borranron los datos correctamente. Gracias por útilizar la aplicación");

            // Limpiar los campos de entrada después de agregar un nuevo registro
            Id_Paciente.Clear();
            txbNombre.Clear();
            txbApellido1.Clear();
            txbApellido2.Clear();
            F_nacimiento.SelectedDate = null;
            txbEmail.Clear();
            txbTelefono.Clear();
            DatosPacientes.ItemsSource = null;
        }

        /**** VOLVER ****/
        private void Btn_volver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Principal ventanaPrincipal = new Cpanel.Principal(miConexionSql);
            ventanaPrincipal.Show();
        }

        /**** SALIR ****/
        private void Btn_salir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación de Iván Bazaga ");
            App.Current.Shutdown();
        }

        private void ListaPacientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraDatosPaciente();
        }
    }
}

