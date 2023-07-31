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

namespace Actividad_12.Cpanel
{
    public partial class Consulta : Window
    {
        SqlConnection miConexionSql;
        public Consulta(SqlConnection miConexionSql)
        {
            InitializeComponent();

            InitializeComponent();
            this.miConexionSql = miConexionSql;

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();
            miConexionSql = new SqlConnection(miConexion);

            MuestraPaciente();
            MuestraDoctor();
        }

        private void listaFechaConsulta_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MuestraDiagnostico();
        }

        private void ListaPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraDoctoresAsignados();
            MuestraConsulta();
            MuestraDiagnostico();

        }

        private void ListaDoctores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraPacientesAsignados();
        }

        private void BtnAgregraPaciente_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Paciente newPaciente = new Cpanel.Paciente(miConexionSql);
            newPaciente.Show();
        }

        private void BtnAgregarDoctor_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Doctor newDoctor = new Cpanel.Doctor(miConexionSql);
            newDoctor.Show();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Principal ventanaPrincipal = new Cpanel.Principal(miConexionSql);
            ventanaPrincipal.Show();
        }

        private void BtnCerrarAplicacion_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación de Iván Bazaga ");
            App.Current.Shutdown();
        }


        /****** MUESTRA LISTA DE PACIENTES *******/
        private void MuestraPaciente()
        {
            string consulta = "SELECT * FROM PACIENTE";
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

            using (adaptadorSql)
            {
                DataTable pacienteTabla = new DataTable();
                adaptadorSql.Fill(pacienteTabla);


                ListaPacientes.DisplayMemberPath = "NOMBRE";
                ListaPacientes.SelectedValuePath = "ID";
                ListaPacientes.ItemsSource = pacienteTabla.DefaultView;
            }
        }

        /***** MUESTRA LISTA DE DOCTORES **************/
        private void MuestraDoctor()
        {

            string consulta = "SELECT * FROM DOCTOR";
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
            using (adaptadorSql)
            {
                DataTable doctorTabla = new DataTable();
                adaptadorSql.Fill(doctorTabla);


                ListaDoctores.DisplayMemberPath = "NOMBRE";

                ListaDoctores.SelectedValuePath = "ID";
                ListaDoctores.ItemsSource = doctorTabla.DefaultView;
            }
        }

        /***** MUESTRA DOCTORES ASIGNADOS AL PACIENTE **************/
        private void MuestraDoctoresAsignados()
        {

            string consulta = "SELECT DISTINCT D.NOMBRE FROM DOCTOR D JOIN CONSULTA C ON D.ID = C.ID_DOCTOR WHERE C.ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable doctoresAsignadosTabla = new DataTable();
                adaptadorSql.Fill(doctoresAsignadosTabla);

                // Configurar el control para mostrar los doctores asignados
                ListaDoctoresAsignados.DisplayMemberPath = "NOMBRE";
                ListaDoctoresAsignados.SelectedValuePath = "ID";
                ListaDoctoresAsignados.ItemsSource = doctoresAsignadosTabla.DefaultView;
            }
        }

        /***** MUESTRA PACIENTES ASIGNADOS AL DOCTOR **************/
        private void MuestraPacientesAsignados()
        {
            string consulta = "SELECT DISTINCT P.NOMBRE FROM PACIENTE P JOIN CONSULTA C ON P.ID = C.ID_PACIENTE WHERE C.ID_DOCTOR = @idDoctor";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idDoctor", ListaDoctores.SelectedValue);

                DataTable pacientesAsignadosTabla = new DataTable();
                adaptadorSql.Fill(pacientesAsignadosTabla);

                // Configurar el control para mostrar los pacientes asignados
                ListaPacientesAsignados.DisplayMemberPath = "NOMBRE";
                ListaPacientesAsignados.SelectedValuePath = "ID";
                ListaPacientesAsignados.ItemsSource = pacientesAsignadosTabla.DefaultView;
            }
        }
        /***** MUESTRA CONSULTA DEL PACIENTE **************/

        private void MuestraConsulta()
        {
            string consulta = "SELECT *, CONCAT ('ID : ',ID, ' -- ' ,FECHA_CONSULTA) as fechaConsulta FROM CONSULTA WHERE ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                listaFechaConsulta.DisplayMemberPath = "fechaConsulta";
                listaFechaConsulta.SelectedValuePath = "ID";
                listaFechaConsulta.ItemsSource = tratamientoTabla.DefaultView;
            }
        }

        /**** MUESTRA DIAGNOSTICO DEL PACIENTE ****/
        private void MuestraDiagnostico()
        {
            string consulta = "SELECT *, CONCAT ('ID : ',ID, '  PACIENTE : ',ID_PACIENTE, '  DIAGNOSTICO : ',DIAGNOSTICO, '  FECHA : ',FECHA_CONSULTA) as infoConsulta FROM CONSULTA WHERE ID_PACIENTE = @idPaciente";
            
            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                listaDiagnostico.DisplayMemberPath = "infoConsulta";
                listaDiagnostico.SelectedValue = "ID";
                listaDiagnostico.ItemsSource = tratamientoTabla.DefaultView;
            }
        }

        /***** BORRAR CONSULTA ASIGNADO AL PACIENTE **************/
        private void BtnBorrarDiagnostico_Click(object sender, RoutedEventArgs e)
        {
            if (listaDiagnostico.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una FECHA PARA BORRAR LA CONSULTA para borrar.");
                return;
            }
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "DELETE FROM CONSULTA WHERE ID =@IDconsulta";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            miSqlCommand.Parameters.AddWithValue("@IDconsulta", Convert.ToInt32(((DataRowView)listaDiagnostico.SelectedItem)["ID"]));
            //miSqlCommand.Parameters.AddWithValue("@IDconsulta", listaDiagnostico.SelectedValue);
            miSqlCommand.ExecuteNonQuery();

            miConexionSql.Close();
            MuestraConsulta();
            MuestraDiagnostico();
            MessageBox.Show("Se borranron los datos correctamente. Gracias por útilizar la aplicación");
        }

        /***** DOUBLE CLICK SOBRE UNA CONSULTA, MUESTRA LOS DATOS PARA MODIFICAR ****/
        private void listaDiagnostico_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Verificar que haya un OBJETO seleccionado en TodosTratamientos
            if (listaDiagnostico.SelectedItem == null || !(listaDiagnostico.SelectedItem is DataRowView selectedRow))
            {
                MessageBox.Show("Por favor, selecciona un TRATAMIENTO para ver los detalles.");
                return;
            }

            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "SELECT DIAGNOSTICO, FECHA_CONSULTA FROM CONSULTA WHERE ID = @idTratamiento";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

            try
            {
                //PARA OBTENER LA ID DEL OBJETO SELECCIONADO EN UNA CADENA DE VALORES DOS FORMAS , LA PRIMERA ESTÁ COMENTADA
                //Y LA VERIFICACION DE LA LINEA 355 " (TodosTratamientos.SelectedItem is DataRowView selectedRow) " HACE REFERENCIA AL CONTENIDO DE ESTA VARIABLE

                //int idTratamiento = Convert.ToInt32(selectedRow["ID"]);
                //miSqlCommand.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                miSqlCommand.Parameters.AddWithValue("@IdTratamiento", Convert.ToInt32(((DataRowView)listaDiagnostico.SelectedItem)["ID"]));
                

                DataTable tratamientoTabla = new DataTable();

                miAdaptadorSql.Fill(tratamientoTabla);

                txbDiagnostico.Text = tratamientoTabla.Rows[0]["DIAGNOSTICO"].ToString();
                TxbFecha.Text = tratamientoTabla.Rows[0]["FECHA_CONSULTA"].ToString();


                MessageBox.Show("Se mostrarán los detalles de la CONSULTUA. Puedes modificar el DIAGNOSTICO Y LA FECHA. Luego pulsar MODIFICAR. Gracias por utilizar la aplicación.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles del tratamiento: " + ex.Message);
            }
            finally
            {
                miConexionSql.Close();
            }
        }

        /***** MODIFICAR  **************/
        private void BtnModificarDiagnostico_Click(object sender, RoutedEventArgs e)
        {
            {

                // Verificar que la conexión esté abierta antes de ejecutar la consulta
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                // Verificar que haya un OBJETO seleccionado en TodosTratamientos
                if (listaDiagnostico.SelectedItem == null || !(listaDiagnostico.SelectedItem is DataRowView selectedRow))
                {
                    MessageBox.Show("Por favor, selecciona una CONSULTA para modificar. CLICK DOS VECES ENCIMA DE LA CONSULTA.");
                    return;
                }

                // ID del OBJETO seleccionado
                int idTratamiento = Convert.ToInt32(selectedRow["ID"]);

                string consulta = "UPDATE CONSULTA SET DIAGNOSTICO = @Diagnostico, FECHA_CONSULTA = @Fecha WHERE ID = @IdTratamiento";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                // Obtener los valores de los campos de texto de la interfaz
                comandoSql.Parameters.AddWithValue("@Diagnostico", txbDiagnostico.Text);
                comandoSql.Parameters.AddWithValue("@Fecha", TxbFecha.SelectedDate);

                comandoSql.Parameters.AddWithValue("@IdTratamiento", idTratamiento);

                comandoSql.ExecuteNonQuery();

                miConexionSql.Close();

                MessageBox.Show("Se modificaron los datos correctamente. Gracias por utilizar la aplicación");

                // Actualizar Metodos

                MuestraDiagnostico();
                MuestraConsulta();
                MuestraDoctoresAsignados();

                // Limpiar los campos después de modificar el tratamiento
                txbDiagnostico.Text = "";
                TxbFecha.Text = "";
            }
        }

        /***** AGREGAR  CONSULTA **************/
        private void BtnAgregarDiagnostico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar que la conexión esté abierta antes de ejecutar la consulta
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                // Verificar que se hayan seleccionado los pacientes y doctores
                if (ListaPacientes.SelectedItem == null || ListaDoctores.SelectedItem == null ||
                    string.IsNullOrEmpty(txbDiagnostico.Text) || string.IsNullOrEmpty(TxbFecha.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos y seleccione un PACIENTE de la lista y un DOCTOR de la lista ,para agregar una nueva CONSULTA.");
                    return;
                }

                string consulta = "INSERT INTO CONSULTA (ID_PACIENTE, ID_DOCTOR, DIAGNOSTICO, FECHA_CONSULTA) VALUES (@IdPaciente, @IdDoctor, @Diagnostico, @Fecha)";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                // Obtener los valores de los campos de texto de la interfaz
                comandoSql.Parameters.AddWithValue("@IdPaciente", ListaPacientes.SelectedValue);
                comandoSql.Parameters.AddWithValue("@IdDoctor", ListaDoctores.SelectedValue);
                comandoSql.Parameters.AddWithValue("@Diagnostico", txbDiagnostico.Text);
                comandoSql.Parameters.AddWithValue("@Fecha", TxbFecha.SelectedDate);

                comandoSql.ExecuteNonQuery();

                miConexionSql.Close();

                // Actualizar Metodos
                MuestraDiagnostico();
                MuestraConsulta();
                MuestraDoctoresAsignados();
                MuestraPacientesAsignados();

                txbDiagnostico.Text = "";
                TxbFecha.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el tratamiento: " + ex.Message);
            }
        }
    }
}
