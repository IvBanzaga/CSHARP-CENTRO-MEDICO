using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Actividad_12.Cpanel
{
    /// <summary>
    /// Lógica de interacción para personalEnfermeria.xaml
    /// </summary>
    public partial class personalEnfermeria : Window
    {
        SqlConnection miConexionSql;

        public string ListaIsla { get; set; }

        public personalEnfermeria(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();


            miConexionSql = new SqlConnection(miConexion);
            MuestraIsla();
            MuestraSupervisor();
            MuestraPersonalEnfermeria();
        }

        /***** MUESTA EL CAMPO ESPECIALIDAD EN UN COMOBOX CON LA INFORMACION DE LA BASE DE DATOS *****/

        private void MuestraIsla()
        {

            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            try
            {
                string consulta = "SELECT * FROM ISLA";


                SqlCommand miComandoSql = new SqlCommand(consulta, miConexionSql);

                SqlDataReader miLectorSql = miComandoSql.ExecuteReader();

                using (miLectorSql)
                {

                    //SeleccionIsla.Items.Clear();


                    while (miLectorSql.Read())
                    {
                        int idIsla = miLectorSql.GetInt32(0);
                        string nombreIsla = miLectorSql.GetString(1);

                        ComboBoxItem nuevoItem = new ComboBoxItem();
                        nuevoItem.Content = nombreIsla;
                        nuevoItem.Tag = idIsla;

                        SeleccionIsla.Items.Add(nuevoItem);
                    }


                    if (SeleccionIsla.Items.Count > 0)
                    {
                        SeleccionIsla.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en combox islas: " + ex.Message);
            }
        }

        /*
         * 
         *  private void MuestraIsla()
        {
            try
            {
                string consulta = "SELECT * FROM ISLA";

                SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (adaptadorSql)
                {
                    DataTable islasCanarias = new DataTable();
                    adaptadorSql.Fill(islasCanarias);

                    SeleccionIsla.ItemsSource = "NOMBRE";
                    SeleccionIsla.SelectedValuePath = "ID";
                    SeleccionIsla.ItemsSource = islasCanarias.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en combox islas: " + ex.Message);
            }
        }*/


        /********************************************************************/
        /********************************************************************/

        /************* MUESTRA SUPERVISOR *****************/

        private void MuestraSupervisor()
        {
            string consulta = "SELECT * FROM DOCTOR";
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
            using (adaptadorSql)
            {
                DataTable doctorTabla = new DataTable();
                adaptadorSql.Fill(doctorTabla);


                ListaSupervisor.DisplayMemberPath = "NOMBRE";

                ListaSupervisor.SelectedValuePath = "ID";
                ListaSupervisor.ItemsSource = doctorTabla.DefaultView;
            }
        }

        /************* MUESTRA PERSONAL ENFERMERIA *****************/

        private void MuestraPersonalEnfermeria()
        {
            try
            {
                string consulta = "SELECT *, CONCAT(ID, ' NOM. ', NOMBRE, ' APEL1. ', APELLIDO1, ' APEL2 ', APELLIDO2) AS infoEnfermera FROM Pesonal_Enfermeria";


                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                using (miAdaptadorSql)
                {


                    DataTable doctorTabla = new DataTable();
                    miAdaptadorSql.Fill(doctorTabla);

                    ListaEnfermera.DisplayMemberPath = "infoEnfermera";
                    ListaEnfermera.SelectedValuePath = "ID";
                    ListaEnfermera.ItemsSource = doctorTabla.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al mostrar los datos de la enfermera: " + ex.Message);
            }
        }
        /********************************************************************/
        /********************************************************************/

        /************* BOTON BORRAR *****************/

        private void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar si se seleccionó una hospitalización en la lista TodosHospitalizacion
            if (ListaEnfermera.SelectedItem == null && ListaEnfermera.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una HOSPITALIZACIÓN para borrar.");
                return;
            }

            try
            {
                if (ListaEnfermera.SelectedItem != null)
                {
                    string consulta = "DELETE FROM Pesonal_Enfermeria WHERE ID=@idEnfermera";

                    SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                    comandoSql.Parameters.AddWithValue("@idEnfermera", Convert.ToInt32(((DataRowView)ListaEnfermera.SelectedItem)["ID"]));

                    comandoSql.ExecuteNonQuery();

                    MuestraPersonalEnfermeria();
                }


                miConexionSql.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar la enfermera: " + ex.Message);
            }
        }

        /********************************************************************/
        /********************************************************************/

        /************* BOTON AGREGAR *****************/

        private void BtnAgregarEnfermera_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                // Verificar que la conexión esté abierta antes de ejecutar la consulta
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                // Verificar que todos los campos obligatorios estén llenos
                if (string.IsNullOrEmpty(txbnombreEnfermero.Text) || string.IsNullOrEmpty(txbapellido1.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios antes de insertar.");
                    return;
                }

                //string consulta = "INSERT INTO Pesonal_Enfermeria (NOMBRE, APELLIDO1, APELLIDO2, TELEFONO, NIF_NIE, FECHA_ALTA, Isla_Residencia, ID_Supervisor ) values (@Nombre, @Apellido1, @Apellido2, @telefono, @nie, @alta, @isla, @supervisor)";
                string islaSelected = SeleccionIsla.SelectionBoxItem.ToString();
                string subConsulta = "Select ID from Isla where Nombre = '" + islaSelected + "'";
                string consulta = "INSERT INTO Pesonal_Enfermeria (NOMBRE, APELLIDO1, APELLIDO2, TELEFONO, NIF_NIE, FECHA_ALTA, Isla_Residencia, ID_Supervisor ) values (@Nombre, @Apellido1, @Apellido2, @telefono, @nie, @alta, (" + subConsulta + ") , @supervisor)";
                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                ;
                miSqlCommand.Parameters.AddWithValue("@Nombre", txbnombreEnfermero.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido1", txbapellido1.Text);
                miSqlCommand.Parameters.AddWithValue("@Apellido2", txbapellido2.Text);
                miSqlCommand.Parameters.AddWithValue("@telefono", txbTelefono.Text);
                miSqlCommand.Parameters.AddWithValue("@nie", txbNie.Text);
                miSqlCommand.Parameters.AddWithValue("@alta", TxbFechaAlta.Text);
                miSqlCommand.Parameters.AddWithValue("@supervisor", ListaSupervisor.SelectedValue);
                //miSqlCommand.Parameters.AddWithValue("@isla", SeleccionIsla.SelectedIndex + 10);



                miSqlCommand.ExecuteNonQuery();

                miConexionSql.Close();

                MuestraSupervisor();
                MuestraPersonalEnfermeria();

                MessageBox.Show("Bienvenido, te has dado de alta en Enfermería en la aplicación de Iván Bazaga");
                // Limpiar los campos de entrada después de agregar un nuevo registro
                txbnombreEnfermero.Clear();
                txbTelefono.Clear();
                txbapellido2.Clear();
                txbapellido1.Clear();
                txbNie.Clear();

                SeleccionIsla.SelectedIndex = 1;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar los datos de enfermera: " + ex.Message);
            }
        }


        /********************************************************************/
        /********************************************************************/

        /************* BOTON MODIFICAR *****************/

        private void BtnModificarEnfermera_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar que haya un OBJETO seleccionado en TodosTratamientos
            if (ListaEnfermera.SelectedItem == null || !(ListaEnfermera.SelectedItem is DataRowView selectedRow) || ListaSupervisor.SelectedItem == null || !(ListaSupervisor.SelectedItem is DataRowView selectedRowSupervisor))
            {
                MessageBox.Show("Por favor, selecciona una Enfermera y/o Supervisor asignado para modificar.");
                return;
            }

            // ID del OBJETO seleccionado
            int idenfermero = Convert.ToInt32(selectedRow["ID"]);
            int idsupervisor = Convert.ToInt32(selectedRowSupervisor["ID"]);


            string islaSelected = SeleccionIsla.SelectionBoxItem.ToString();
            string subConsulta = "Select ID from Isla where Nombre = '" + islaSelected + "'";



            /**********************************************************************************************/
            /*********************************************************************************************/


            //string doctorSupervisor = ListaSupervisor.SelectedIndex.ToString();
            //string subSupervisor = "Select ID from Doctor where Nombre = '" + doctorSupervisor + "'";

            //string subSupervisor = "Select NOMBRE from DOCTOR where ID = (select ID_SUPERVISOR FROM Pesonal_Enfermeria WHERE ID = @idEnfermeria)";







            /**********************************************************************************************/
            /**********************************************************************************************/

            string consulta = "UPDATE Pesonal_Enfermeria SET NOMBRE = @Nombre, APELLIDO1 = @Apellido1, APELLIDO2 = @Apellido2, TELEFONO = @telefono, NIF_NIE = @nie, FECHA_ALTA = @alta, Isla_Residencia = (" + subConsulta + "), ID_Supervisor = @supervisor WHERE ID = @idenfermero";

            //string consulta = "UPDATE Pesonal_Enfermeria SET NOMBRE = @Nombre, APELLIDO1 = @Apellido1, APELLIDO2 = @Apellido2, TELEFONO = @telefono, NIF_NIE = @nie, FECHA_ALTA = @alta, Isla_Residencia = (" + subConsulta + "), ID_Supervisor = (" + subSupervisor + ") WHERE ID = @idenfermero";


            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            // Obtener los valores de los campos de texto de la interfaz
            comandoSql.Parameters.AddWithValue("@Nombre", txbnombreEnfermero.Text);
            comandoSql.Parameters.AddWithValue("@Apellido1", txbapellido1.Text);
            comandoSql.Parameters.AddWithValue("@Apellido2", txbapellido2.Text);
            comandoSql.Parameters.AddWithValue("@telefono", txbTelefono.Text);
            comandoSql.Parameters.AddWithValue("@nie", txbNie.Text);
            comandoSql.Parameters.AddWithValue("@alta", TxbFechaAlta.Text);
            comandoSql.Parameters.AddWithValue("@supervisor", idsupervisor);
            //comandoSql.Parameters.AddWithValue("@supervisor", ListaSupervisor.SelectedIndex +1);

            

            comandoSql.Parameters.AddWithValue("@idenfermero", idenfermero);


            comandoSql.ExecuteNonQuery();

            miConexionSql.Close();

            MessageBox.Show("Se modificaron correctamente. Gracias por utilizar la aplicación");

            // Actualizar Metodos

            MuestraPersonalEnfermeria();
            MuestraSupervisor();

            txbnombreEnfermero.Text = "";
            txbapellido1.Text = "";
            txbapellido2.Text = "";
            txbTelefono.Text = "";
            TxbFechaAlta.Text = "";
            txbNie.Text = "";

        }



        /********************************************************************/
        /********************************************************************/
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Principal ventanaPrincipal = new Cpanel.Principal(miConexionSql);
            ventanaPrincipal.Show();
        }
        /********************************************************************/
        /********************************************************************/
        private void BtnCerrarAplicacion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación de Iván Bazaga ");
            App.Current.Shutdown();
        }

        /********************************************************************/
        /********************************************************************/

        /************  LISTAS ***********************************************/

        /************  LISTA SUPERVISOR  ************************************/

        private void ListaSupervisor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /********************************************************************/
        /********************************************************************/

        /************  LISTA ENFERMERIA  ************************************/
        private void ListaEnfermera_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            {
                // Verificar que haya un OBJETO seleccionado en TodosHospitalizacion
                if (ListaEnfermera.SelectedItem == null || !(ListaEnfermera.SelectedItem is DataRowView selectedRow))
                {
                    // Si no hay un elemento seleccionado o el tipo de elemento no es DataRowView, limpiar los campos y salir del evento.
                    txbnombreEnfermero.Text = "";
                    txbapellido1.Text = "";
                    txbapellido2.Text = "";
                    TxbFechaAlta.Text = "";
                    txbNie.Text = "";
                    txbTelefono.Text = "";

                    return;
                }

                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }


                //string islaSelected = SeleccionIsla.Text;

                string subConsulta = "Select NOMBRE from ISLA where ID = (select ISLA_RESIDENCIA FROM Pesonal_Enfermeria WHERE ID = @idEnfermeria)";

                string subConsultaSupervisor  = "Select NOMBRE from DOCTOR where ID = (select ID_SUPERVISOR FROM Pesonal_Enfermeria WHERE ID = @idEnfermeria)";

                string consulta = "SELECT NOMBRE, APELLIDO1, APELLIDO2, TELEFONO, NIF_NIE, FECHA_ALTA, ISLA_RESIDENCIA = (" + subConsulta + "), ID_SUPERVISOR = (" + subConsultaSupervisor + ") FROM Pesonal_Enfermeria WHERE ID = @idEnfermeria";

               
                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

                try
                {
                    int idTratamiento = Convert.ToInt32(selectedRow["ID"]);
                    miSqlCommand.Parameters.AddWithValue("@idEnfermeria", idTratamiento);

                    DataTable tratamientoTabla = new DataTable();

                    miAdaptadorSql.Fill(tratamientoTabla);

                    txbnombreEnfermero.Text = tratamientoTabla.Rows[0]["NOMBRE"].ToString();
                    txbapellido1.Text = tratamientoTabla.Rows[0]["APELLIDO1"].ToString();
                    txbapellido2.Text = tratamientoTabla.Rows[0]["APELLIDO2"].ToString();
                    txbTelefono.Text = tratamientoTabla.Rows[0]["TELEFONO"].ToString();
                    TxbFechaAlta.Text = tratamientoTabla.Rows[0]["FECHA_ALTA"].ToString();
                    txbNie.Text = tratamientoTabla.Rows[0]["NIF_NIE"].ToString();
                    SeleccionIsla.Text = tratamientoTabla.Rows[0]["ISLA_RESIDENCIA"].ToString();
                    txbSupervisor.Text = tratamientoTabla.Rows[0]["ID_SUPERVISOR"].ToString();






                    MessageBox.Show("Se mostrarán los detalles de la ENFERMERA. Puedes modificarlos y luego pulsar MODIFICAR. Gracias por utilizar la aplicación.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener detalles de la ENFERMERA: " + ex.Message);
                }
                finally
                {
                    miConexionSql.Close();
                }
            }
        }

        /********************************************************************************************/
        /********************************************************************************************/
        /********************************************************************************************/

    }
}

