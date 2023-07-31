using Actividad_12.HospitalDBDataSet1TableAdapters;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace Actividad_12.Cpanel
{
    public partial class Doctor : Window
    {
        SqlConnection miConexionSql;

        public string ListaEspecialidades { get; set; }

        public Doctor(SqlConnection miConexionSql)
        {
            InitializeComponent();

            this.miConexionSql = miConexionSql;

            /*---- CLASE MI CONEXION CON LOS DATOS DE CONEXION A BASE DE DATOS ******/

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();


            miConexionSql = new SqlConnection(miConexion);

            MuestraDoctor();
            //MuestraEspecialidad();
            Especialidad.SelectedIndex = 3;
        }

        /***** MUESTA EL CAMPO ESPECIALIDAD EN UN COMOBOX CON LA INFORMACION DE LA BASE DE DATOS *****/

        /*private void MuestraEspecialidad()
        {

            string consulta = "SELECT ID, Especialidad FROM DOCTOR";

            SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataReader miLectorSql = miComandoSql.ExecuteReader();

            using (miLectorSql)
            {
                // Limpiar cualquier valor existente en el ComboBox
                Especialidad.Items.Clear();

                // Agregar los datos al ComboBox
                while (miLectorSql.Read())
                {
                    int id = miLectorSql.GetInt32(0);
                    string siglasEspecialidad = miLectorSql.GetString(1);

                    ComboBoxItem nuevoItem = new ComboBoxItem();
                    nuevoItem.Content = siglasEspecialidad;
                    nuevoItem.Tag = id;

                    Especialidad.Items.Add(nuevoItem);
                }

                // Seleccionar el primer item en el ComboBox
                if (Especialidad.Items.Count > 0)
                {
                    Especialidad.SelectedIndex = 0;
                }
            }
        }********************************************************************************************/
        /********************************************************************************************/
        /********************************************************************************************/

        /***** RESULTADO EN UNA LISTA DE DOCTORES *****/

        private void MuestraDoctor()
        {

            string consulta = "SELECT Nombre AS infoDoctor, ID FROM DOCTOR";

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
            using (adaptadorSql)
            {
                DataTable doctorTabla = new DataTable();
                adaptadorSql.Fill(doctorTabla);

                ListaDoctores.DisplayMemberPath = "infoDoctor";
                ListaDoctores.SelectedValuePath = "ID";
                ListaDoctores.ItemsSource = doctorTabla.DefaultView;
            }
        }


        /***** MUESTRA DATOS  DOCTORES ****/

        private void MuestraDatosDoctor()

        {
            try
            {
                string consulta = "SELECT *, CONCAT (ID, ' - ',NOMBRE, ' ' ,APELLIDO1, ' ' ,APELLIDO2) as infoDoctor FROM DOCTOR WHERE ID = @idDoctor";

                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {
                        sqlComando.Parameters.AddWithValue("@idDoctor", ListaDoctores.SelectedValue);

                        DataTable doctorTabla = new DataTable();
                        miAdaptadorSql.Fill(doctorTabla);

                        DatosDoctores.DisplayMemberPath = "infoDoctor";
                        DatosDoctores.SelectedValuePath = "ID";
                        DatosDoctores.ItemsSource = doctorTabla.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar los datos del Doctor: " + ex.Message);
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
                //Aquí puedes mostrar la imagen seleccionada en un control de imagen si lo deseas
            }
        }

       /**** BOTON INSERTAR DOCTOR ****/

        private void Btn_insertar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar que la conexión esté abierta antes de ejecutar la consulta
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                // Verificar que todos los campos obligatorios estén llenos
                if (string.IsNullOrEmpty(Id_Doctor.Text) || string.IsNullOrEmpty(txbNombre.Text) || string.IsNullOrEmpty(txbApellido1.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios antes de insertar.");
                    return;
                }

                string consulta = "INSERT INTO DOCTOR (ID, NOMBRE, APELLIDO1, APELLIDO2, ESPECIALIDAD) values (@ID, @Nombre, @Apellido1, @Apellido2, @Especialidad)";
                //string consulta = "INSERT INTO DOCTOR (" + Id_Doctor.Text + ",'" + txbNombre.Text + "','" + txbApellido1.Text + "','" + txbApellido2.Text + "','" + Especialidad.SelectionBoxItem + "');";
                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                miSqlCommand.Parameters.AddWithValue("@ID", Id_Doctor.Text);
                miSqlCommand.Parameters.AddWithValue("@Nombre", txbNombre.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido1", txbApellido1.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido2", txbApellido2.Text);
                miSqlCommand.Parameters.AddWithValue("@Especialidad", Especialidad.SelectionBoxItem);

                /***** ESTE ES PARA EL DESPLEGABLE CON INFORMACION DE LA BASE DE DATOS *********/

                //miSqlCommand.Parameters.AddWithValue("@Especialidad", ((ComboBoxItem)Especialidad.SelectedItem).Content.ToString());

                /*******************************************************************************/


                // Agregar la ruta de la imagen seleccionada al parámetro @RutaImagen
                // string rutaImagen = imagenSeleccionada.Source?.ToString();
                // miSqlCommand.Parameters.AddWithValue("@RutaImagen", rutaImagen);

                miSqlCommand.ExecuteNonQuery();

                miConexionSql.Close();

                MuestraDoctor();


                MessageBox.Show("Bienvenido, te has dado de alta como DOCTOR en la aplicación de Iván Bazaga");
                // Limpiar los campos de entrada después de agregar un nuevo registro
                Id_Doctor.Clear();
                txbNombre.Clear();
                txbApellido1.Clear();
                txbApellido2.Clear();
                imagenSeleccionada = null;
                //rutaImagen = null;
                Especialidad.SelectedIndex = 1;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar los datos: " + ex.Message);
            }
        }

        /**** SELECCIONAR PARA ACTUALIZAR ****/

        private void Btn_actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar que haya un DOCTOR seleccionado en LISTADOCTORES
                if (ListaDoctores.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, seleccione un Doctor para actualizar.");
                    return;
                }

                string consulta = "SELECT ID,NOMBRE,APELLIDO1,APELLIDO2,ESPECIALIDAD FROM DOCTOR where ID = @idDoctor";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

                using (miAdaptadorSql)
                {
                    miSqlCommand.Parameters.AddWithValue("@idDoctor", ListaDoctores.SelectedValue);
                    DataTable doctorTabla = new DataTable();

                    miAdaptadorSql.Fill(doctorTabla);

                    txbNombre.Text = doctorTabla.Rows[0]["NOMBRE"].ToString();
                    txbApellido1.Text = doctorTabla.Rows[0]["APELLIDO1"].ToString();
                    txbApellido2.Text = doctorTabla.Rows[0]["Apellido2"].ToString();
                    Especialidad.Text = doctorTabla.Rows[0]["ESPECIALIDAD"].ToString();
                    Id_Doctor.Text = doctorTabla.Rows[0]["ID"].ToString();

                    MessageBox.Show("Se van actualizar los datos del DOCTOR, después de cambiar los datos púlsa modificar. Gracias por útilizar la aplicación");

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error al Actualizar DOCTOR: " + ex.Message);
            }
        }

        /**** BOTON BORRAR ****/
        private void Btn_borrar_Click(object sender, RoutedEventArgs e)
        {

            if (ListaDoctores.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un Paciente para borrar.");
                return;
            }
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }


            string consulta = "DELETE FROM DOCTOR WHERE ID =@ID";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            miSqlCommand.Parameters.AddWithValue("@ID", ListaDoctores.SelectedValue);

            miSqlCommand.ExecuteNonQuery();

            miConexionSql.Close();

            MuestraDoctor();
            MessageBox.Show("Se borranron los datos correctamente. Gracias por útilizar la aplicación");
            // Limpiar los campos de entrada después de agregar un nuevo registro
            Id_Doctor.Clear();
            txbNombre.Clear();
            txbApellido1.Clear();
            txbApellido2.Clear();
            imagenSeleccionada = null;
            //rutaImagen = null;
            Especialidad.SelectedIndex = 0;
            DatosDoctores.ItemsSource = null;

        }

        /**** MODIFICAR ****/

        private void Btn_modifica_Click(object sender, RoutedEventArgs e)
        {
            {
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                string consulta = "UPDATE DOCTOR SET Nombre=@Nombre, Apellido1=@Apellido1, Apellido2=@Apellido2, ID=@IDDOCTOR, ESPECIALIDAD=@Especialidad WHERE ID=@IDDOCTOR";
               
                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                miSqlCommand.Parameters.AddWithValue("@Nombre", txbNombre.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido1", txbApellido1.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido2", txbApellido2.Text);
                miSqlCommand.Parameters.AddWithValue("@IDDOCTOR", Id_Doctor.Text);
                miSqlCommand.Parameters.AddWithValue("@Especialidad", Especialidad.SelectionBoxItem);
                //string rutaImagen = imagenSeleccionada.Source?.ToString();
                //miSqlCommand.Parameters.AddWithValue("@RutaImagen", rutaImagen);

                miSqlCommand.ExecuteNonQuery();
                miConexionSql.Close();
                MessageBox.Show("Se modificaron los datos correctamente. Gracias por utilizar la aplicación");
                // Limpiar los campos de entrada después de agregar un nuevo registro
                Id_Doctor.Clear();
                txbNombre.Clear();
                txbApellido1.Clear();
                txbApellido2.Clear();
                imagenSeleccionada = null;
                //rutaImagen = null;
                Especialidad.SelectedIndex = 0;
                MuestraDatosDoctor();
                MuestraDoctor();
                DatosDoctores.ItemsSource = null;
            }



        }

        /**** BOTONES ****/

        private void Btn_salir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación de Iván Bazaga ");
            App.Current.Shutdown();
            //this.Close();
        }

        private void Btn_volver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Principal ventanaPrincipal = new Cpanel.Principal(miConexionSql);
            ventanaPrincipal.Show();
        }

        private void ListaDoctores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraDatosDoctor();
        }
    }
}
