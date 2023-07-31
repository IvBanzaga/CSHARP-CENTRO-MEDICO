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
    public partial class Tratamientonew : Window
    {
        SqlConnection miConexionSql;
        public Tratamientonew(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();
            miConexionSql = new SqlConnection(miConexion);

            // Llamar a MuestraPaciente para cargar la lista inicial de pacientes
            MuestraPaciente();
            MuestraDoctor();
        }

        private void ListaPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TodosTratamientos.ItemsSource = null;
            MuestraTratamiento();
            MuestraDoctoresAsignados();
        }

        private void ListaTratamientos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TodosTratamientos.ItemsSource = null;
            MuestraTodosTratamientos();
        }

        private void ListaPacientesAsignados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TodosTratamientos.ItemsSource = null;
            MuestraTodosTratamientos();
        }

        private void ListaDoctores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraPacientesAsignados();
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

            string consulta = "SELECT DISTINCT D.NOMBRE FROM DOCTOR D JOIN TRATAMIENTO T ON D.ID = T.ID_DOCTOR WHERE T.ID_PACIENTE = @idPaciente";

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
            string consulta = "SELECT DISTINCT P.NOMBRE FROM PACIENTE P " +
                              "JOIN TRATAMIENTO T ON P.ID = T.ID_PACIENTE " +
                              "WHERE T.ID_DOCTOR = @idDoctor";

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

        /***** MUESTRA MEDICAMENTOS ASIGNADOS AL PACIENTE **************/

        private void MuestraTratamiento()
        {
            string consulta = "SELECT DISTINCT MEDICAMENTO FROM TRATAMIENTO WHERE ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                ListaTratamientos.DisplayMemberPath = "MEDICAMENTO";
                ListaTratamientos.SelectedValuePath = "ID";
                ListaTratamientos.ItemsSource = tratamientoTabla.DefaultView;
            }
        }

        /***** MUESTRA TRATAMIENTO DEL MEDICAMENTO ASIGNADO AL PACIENTE **************/

        private void MuestraTodosTratamientos()
        {
            string consulta = "SELECT *, CONCAT (ID, ' - ',ID_PACIENTE, ' ' ,MEDICAMENTO, ' ' ,DOSIS, ' ' , DURACION) as infoPaciente FROM TRATAMIENTO WHERE ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                TodosTratamientos.DisplayMemberPath = "infoPaciente";
                TodosTratamientos.SelectedValue = "ID";
                TodosTratamientos.ItemsSource = tratamientoTabla.DefaultView;
            }
        }

        /***** BORRAR TRATAMIENTO ASIGNADO AL PACIENTE **************/
        private void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (TodosTratamientos.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione un tratamiento para borrar.");
                return;
            }

            try
            {

                string consulta = "DELETE FROM TRATAMIENTO WHERE ID=@IdTratamiento";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                comandoSql.Parameters.AddWithValue("@IdTratamiento", Convert.ToInt32(((DataRowView)TodosTratamientos.SelectedItem)["ID"]));

                comandoSql.ExecuteNonQuery();

                miConexionSql.Close();

                MuestraTodosTratamientos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el TRATAMIENTO: " + ex.Message);
            }
        }
        /***** AGREGAR **************/
        private void BtnAgregarTratamiento_Click(object sender, RoutedEventArgs e)
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
                    string.IsNullOrEmpty(txbMedicamento.Text) || string.IsNullOrEmpty(txbDosis.Text) || string.IsNullOrEmpty(txbDuracion.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos y seleccione un paciente y un doctor para agregar un nuevo tratamiento.");
                    return;
                }

                string consulta = "INSERT INTO TRATAMIENTO (ID_PACIENTE, ID_DOCTOR, MEDICAMENTO, DOSIS, DURACION) VALUES (@IdPaciente, @IdDoctor, @Medicamento, @Dosis, @Duracion)";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                // Obtener los valores de los campos de texto de la interfaz
                comandoSql.Parameters.AddWithValue("@IdPaciente", ListaPacientes.SelectedValue);
                comandoSql.Parameters.AddWithValue("@IdDoctor", ListaDoctores.SelectedValue);
                comandoSql.Parameters.AddWithValue("@Medicamento", txbMedicamento.Text);
                comandoSql.Parameters.AddWithValue("@Dosis", txbDosis.Text);
                comandoSql.Parameters.AddWithValue("@Duracion", Convert.ToInt32(txbDuracion.Text));

                comandoSql.ExecuteNonQuery();

                miConexionSql.Close();

                // Actualizar Metodos
                MuestraTodosTratamientos();
                MuestraTratamiento();
                MuestraDoctoresAsignados();
                MuestraPacientesAsignados();

                txbMedicamento.Text = "";
                txbDosis.Text = "";
                txbDuracion.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el tratamiento: " + ex.Message);
            }
        }

        /***** MODIFICAR TRATAMIENTO **************/
        private void BtnModificarTratamiento_Click(object sender, RoutedEventArgs e)
        {

            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar que haya un OBJETO seleccionado en TodosTratamientos
            if (TodosTratamientos.SelectedItem == null || !(TodosTratamientos.SelectedItem is DataRowView selectedRow))
            {
                MessageBox.Show("Por favor, selecciona un TRATAMIENTO para modificar.");
                return;
            }

            // ID del OBJETO seleccionado
            int idTratamiento = Convert.ToInt32(selectedRow["ID"]);

            string consulta = "UPDATE TRATAMIENTO SET MEDICAMENTO = @Medicamento, DOSIS = @Dosis, DURACION = @Duracion WHERE ID = @IdTratamiento";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            // Obtener los valores de los campos de texto de la interfaz
            comandoSql.Parameters.AddWithValue("@Medicamento", txbMedicamento.Text);
            comandoSql.Parameters.AddWithValue("@Dosis", txbDosis.Text);

            int duracion;
            if (int.TryParse(txbDuracion.Text, out duracion))
            {
                // Conversión correcta
                comandoSql.Parameters.AddWithValue("@Duracion", duracion);
            }
            else
            {
                MessageBox.Show("Por favor, ingresa una duración válida (un número entero) para el tratamiento.");
                return;
            }

            comandoSql.Parameters.AddWithValue("@IdTratamiento", idTratamiento);

            comandoSql.ExecuteNonQuery();

            miConexionSql.Close();

            MessageBox.Show("Se modificaron los datos correctamente. Gracias por utilizar la aplicación");

            // Actualizar Metodos

            MuestraTodosTratamientos();
            MuestraTratamiento();
            MuestraDoctoresAsignados();

            // Limpiar los campos después de modificar el tratamiento
            txbMedicamento.Text = "";
            txbDosis.Text = "";
            txbDuracion.Text = "";

        }



        /*** DOUBLE CLICK SOBRE UN TRATAMIENTO , MUESTRA LOS DATOS PARA MODIFICAR *****/

        private void ListaTratamientos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TodosTratamientos.ItemsSource = null;
            MuestraTodosTratamientos();
        }

        private void TodosTratamientos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            // Verificar que haya un OBJETO seleccionado en TodosTratamientos
            if (TodosTratamientos.SelectedItem == null || !(TodosTratamientos.SelectedItem is DataRowView selectedRow))
            {
                MessageBox.Show("Por favor, selecciona un TRATAMIENTO para ver los detalles.");
                return;
            }

            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "SELECT MEDICAMENTO, DOSIS, DURACION FROM TRATAMIENTO WHERE ID = @idTratamiento";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

            try
            {
                //PARA OBTENER LA ID DEL OBJETO SELECCIONADO EN UNA CADENA DE VALORES DOS FORMAS , LA PRIMERA ESTÁ COMENTADA
                //Y LA VERIFICACION DE LA LINEA 355 " (TodosTratamientos.SelectedItem is DataRowView selectedRow) " HACE REFERENCIA AL CONTENIDO DE ESTA VARIABLE

                //int idTratamiento = Convert.ToInt32(selectedRow["ID"]);
                //miSqlCommand.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                miSqlCommand.Parameters.AddWithValue("@IdTratamiento", Convert.ToInt32(((DataRowView)TodosTratamientos.SelectedItem)["ID"]));


                DataTable tratamientoTabla = new DataTable();

                miAdaptadorSql.Fill(tratamientoTabla);

                txbMedicamento.Text = tratamientoTabla.Rows[0]["MEDICAMENTO"].ToString();
                txbDosis.Text = tratamientoTabla.Rows[0]["DOSIS"].ToString();
                txbDuracion.Text = tratamientoTabla.Rows[0]["DURACION"].ToString();

                MessageBox.Show("Se mostrarán los detalles del TRATAMIENTO. Puedes modificarlos y luego pulsar MODIFICAR. Gracias por utilizar la aplicación.");
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

        /**** BOTONES ****/
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
    }
}


