using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    public partial class Hospitalizacion : Window
    {
        SqlConnection miConexionSql;
        public Hospitalizacion(SqlConnection miConexionSql)
        {
            InitializeComponent();

            InitializeComponent();
            this.miConexionSql = miConexionSql;

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();
            miConexionSql = new SqlConnection(miConexion);

            // Llamar a MuestraPaciente para cargar la lista inicial de pacientes
            MuestraPaciente();
            MuestraDoctor();
        }

        /********************************************************************/
        /********************************************************************/

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

        /********************************************************************/
        /********************************************************************/

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

        /********************************************************************/
        /********************************************************************/

        /***** MUESTRA DOCTORES ASIGNADOS AL PACIENTE **************/

        private void MuestraDoctoresAsignados()
        {

            string consulta = "SELECT DISTINCT D.NOMBRE FROM DOCTOR D JOIN HOSPITALIZACION H ON D.ID = H.ID_DOCTORRESPONSABLE WHERE H.ID_PACIENTE = @idPaciente";

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

        /********************************************************************/
        /********************************************************************/

        /***** MUESTRA DATOS HOSPITALIZACION ASIGNADO AL PACIENTE DEL DOCTOR DESDE SU CAMA **************/
        private void MuestraTodosHospitalizacionDoctor()
        {
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar si se seleccionó una cama válida
            if (ListaCamaAsignadaPacienteDoctor.SelectedItem != null)
            {
                // Cama seleccionada 
                DataRowView selectedCamaRow = ListaCamaAsignadaPacienteDoctor.SelectedItem as DataRowView;
                string camaSeleccionada = selectedCamaRow["CAMA"].ToString();

                // Paciente seleccionado 
                int idPacienteSeleccionado = 0;
                if (ListaPacientesAsignados.SelectedItem != null)
                {
                    DataRowView selectedPacienteRow = ListaPacientesAsignados.SelectedItem as DataRowView;
                    idPacienteSeleccionado = Convert.ToInt32(selectedPacienteRow["ID"]);
                }

                string consulta = "SELECT H.ID, P.NOMBRE, CONCAT (' DOCTOR ' , H.ID_DOCTORRESPONSABLE, ' PACIENTE ', H.ID_PACIENTE, ' HAB. ', H.HABITACION, ' CAMA ', H.CAMA, ' F.ingreso ', H.FECHA_INGRESO, ' F.alta ', H.FECHA_ALTA) as infoPaciente FROM HOSPITALIZACION H INNER JOIN PACIENTE P ON H.ID_PACIENTE = P.ID WHERE H.CAMA = @cama";

                // FILTRO PACIENTE SELECCIONADO
                if (idPacienteSeleccionado != 0)
                {
                    consulta += " AND H.ID_PACIENTE = @idPaciente";
                }

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

                using (adaptadorSql)
                {
                    comandoSql.Parameters.AddWithValue("@cama", camaSeleccionada);

                    // FILTRAR POR EL PACIENTE SELECCIONADO
                    if (idPacienteSeleccionado != 0)
                    {
                        comandoSql.Parameters.AddWithValue("@idPaciente", idPacienteSeleccionado);
                    }

                    DataTable camasTabla = new DataTable();
                    adaptadorSql.Fill(camasTabla);

                    TodosHospitalizacionDoctor.DisplayMemberPath = "infoPaciente";
                    TodosHospitalizacionDoctor.SelectedValuePath = "ID";
                    TodosHospitalizacionDoctor.ItemsSource = camasTabla.DefaultView;
                }
            }
            else
            {
                // Si no se seleccionó una cama válida, limpiamos el control TodosHospitalizacionDoctor
                //TodosHospitalizacionDoctor.ItemsSource = new object[0];
            }

            miConexionSql.Close();
        }


        /**********************************************************************************/
        /**********************************************************************************/

        /***** MUESTRA PACIENTES ASIGNADOS AL DOCTOR **************/
        private void MuestraPacientesAsignados()
        {
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }
            
            // Verificar si se seleccionó un doctor válido
            if (TodosHospitalizacionDoctorAsignado.SelectedItem != null)
            {
                // Obtener el ID del paciente seleccionado en el control TodosHospitalizacionDoctorAsignado
                DataRowView selectedDataRow = TodosHospitalizacionDoctorAsignado.SelectedItem as DataRowView;
                int idPacienteSeleccionado = Convert.ToInt32(selectedDataRow["ID_PACIENTE"]);

                string consulta = "SELECT NOMBRE, ID FROM PACIENTE WHERE ID = @idPaciente";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

                using (adaptadorSql)
                {
                    comandoSql.Parameters.AddWithValue("@idPaciente", idPacienteSeleccionado);

                    DataTable pacientesAsignadosTabla = new DataTable();
                    adaptadorSql.Fill(pacientesAsignadosTabla);

                    ListaPacientesAsignados.DisplayMemberPath = "NOMBRE";
                    ListaPacientesAsignados.SelectedValuePath = "ID";
                    ListaPacientesAsignados.ItemsSource = pacientesAsignadosTabla.DefaultView;
                }
            }
            else
            {
                // Si no se seleccionó un doctor válido, limpiamos el control ListaPacientesAsignados
                //ListaPacientesAsignados.ItemsSource = null;
            }

            miConexionSql.Close();
        }



        /********************************************************************/
        /********************************************************************/


        /***** MUESTRA CAMAS ASIGNADOS AL PACIENTE **************/

        private void MuestraCama()
        {
            string consulta = "SELECT DISTINCT CAMA FROM HOSPITALIZACION WHERE ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                ListaCamaAsignada.DisplayMemberPath = "CAMA";
                ListaCamaAsignada.SelectedValuePath = "ID";
                ListaCamaAsignada.ItemsSource = tratamientoTabla.DefaultView;
            }
        }

        /********************************************************************/
        /********************************************************************/

        /******     MUESTRA CAMAS ASIGNADOS A LOS PACIENTES DEL AL DOCTOR  ***********/

        private void MuestraCamaParaPacientesAsignados()
        {
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar si se seleccionó un paciente y un doctor válidos
            if (ListaPacientesAsignados.SelectedItem != null && ListaDoctores.SelectedItem != null)
            {

                string consulta = "SELECT H.CAMA FROM HOSPITALIZACION H WHERE H.ID_PACIENTE = @idPaciente AND H.ID_DoctorResponsable = @idDoctor";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
                SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

                using (adaptadorSql)
                {
                    comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientesAsignados.SelectedValue);
                    comandoSql.Parameters.AddWithValue("@idDoctor", ListaDoctores.SelectedValue);

                    DataTable camasParaPacientesAsignadosTabla = new DataTable();
                    adaptadorSql.Fill(camasParaPacientesAsignadosTabla);

                    // Configurar el control para mostrar las camas para el paciente con el doctor seleccionado
                    ListaCamaAsignadaPacienteDoctor.DisplayMemberPath = "CAMA";
                    ListaCamaAsignadaPacienteDoctor.SelectedValuePath = "ID";
                    ListaCamaAsignadaPacienteDoctor.ItemsSource = camasParaPacientesAsignadosTabla.DefaultView;
                }
            }
            else
            {
                // Si no se seleccionó un paciente válido o un doctor válido, limpiamos el control ListaCamaAsignadaPacienteDoctor
                //ListaCamaAsignadaPacienteDoctor.ItemsSource = null;
            }

            miConexionSql.Close();
        }

        /************************************************************************/
        /***********************************************************************/

        /***** MUESTRA DATOS HOSPITALIZACION ASIGNADO AL PACIENTE **************/

        private void MuestraTodosHospitalizacion()
        {
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "SELECT *, CONCAT (ID, ' DOCTOR ' ,ID_DOCTORRESPONSABLE, ' - ',ID_PACIENTE, ' HAB. ' ,HABITACION, ' CAMA ' ,CAMA, ' F.ingreso ' , FECHA_INGRESO, ' F.alta ', FECHA_ALTA) as infoPaciente FROM HOSPITALIZACION WHERE ID_PACIENTE = @idPaciente";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaPacientes.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                TodosHospitalizacion.DisplayMemberPath = "infoPaciente";
                TodosHospitalizacion.SelectedValue = "ID";
                TodosHospitalizacion.ItemsSource = tratamientoTabla.DefaultView;


            }
            miConexionSql.Close();
        }

        /**********************************************************************************/
        /**********************************************************************************/

        /***** MUESTRA DATOS HOSPITALIZACION ASIGNADO AL DOCTOR **************/
        private void MuestraTodosHospitalizacionDoctorAsignado()
        {
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "SELECT *, CONCAT (ID, ' - ',ID_PACIENTE, ' HAB. ' ,HABITACION, ' CAMA ' ,CAMA, ' F.ingreso ' , FECHA_INGRESO, ' F.alta ', FECHA_ALTA) as infoPaciente FROM HOSPITALIZACION WHERE ID_DOCTORRESPONSABLE = @idDoctor";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);

            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idDoctor", ListaDoctores.SelectedValue);

                DataTable camasTabla = new DataTable();
                adaptadorSql.Fill(camasTabla);

                TodosHospitalizacionDoctorAsignado.DisplayMemberPath = "infoPaciente";
                TodosHospitalizacionDoctorAsignado.SelectedValuePath = "ID";
                TodosHospitalizacionDoctorAsignado.ItemsSource = camasTabla.DefaultView;


            }
            miConexionSql.Close();
        }

        /********************************************************************/
        /********************************************************************/

        /***** BORRAR HOSPITALIZACION ASIGNADO AL PACIENTE **************/
        private void BtnBorrar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar si se seleccionó una hospitalización en la lista TodosHospitalizacion
            if (TodosHospitalizacion.SelectedItem == null && TodosHospitalizacionDoctor.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione una HOSPITALIZACIÓN para borrar.");
                return;
            }

            try
            {
                if (TodosHospitalizacion.SelectedItem != null)
                {
                    string consulta = "DELETE FROM HOSPITALIZACION WHERE ID=@IdTratamiento";

                    SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                    comandoSql.Parameters.AddWithValue("@IdTratamiento", Convert.ToInt32(((DataRowView)TodosHospitalizacion.SelectedItem)["ID"]));

                    comandoSql.ExecuteNonQuery();

                    MuestraTodosHospitalizacion();
                }

                if (TodosHospitalizacionDoctor.SelectedItem != null)
                {
                    string consultaDoctor = "DELETE FROM HOSPITALIZACION WHERE ID=@IdTratamientoDoctor";

                    SqlCommand comandoSqlDoctor = new SqlCommand(consultaDoctor, miConexionSql);

                    comandoSqlDoctor.Parameters.AddWithValue("@IdTratamientoDoctor", Convert.ToInt32(((DataRowView)TodosHospitalizacionDoctor.SelectedItem)["ID"]));

                    comandoSqlDoctor.ExecuteNonQuery();

                    MuestraTodosHospitalizacionDoctor();
                }

                miConexionSql.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar el HOSPITALIZACIÓN: " + ex.Message);
            }
        }

        /********************************************************************/
        /********************************************************************/

        /***** AGREGAR HOSPITALIZACION **************/
        private void BtnAgregarHospitalizacion_Click(object sender, RoutedEventArgs e)
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
                    string.IsNullOrEmpty(txbHabitacion.Text) || string.IsNullOrEmpty(txbCama.Text) || string.IsNullOrEmpty(TxbFechaIngreso.Text) || string.IsNullOrEmpty(TxbHoraIngreso.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos y seleccione un paciente DE LA LISTA DE PACIENTES y un doctor DE LA LISTA DE DOCTORES para agregar una NUEVA HOSPITALIZACIÓN.");
                    return;
                }

                string consulta = "INSERT INTO HOSPITALIZACION (ID_PACIENTE, ID_DoctorResponsable, HABITACION, CAMA, FECHA_INGRESO, FECHA_ALTA) VALUES (@IdPaciente, @DoctorResponsable, @Habitacion, @Cama, @Fechaingreso, @Fechaalta)";

                SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

                // Obtener los valores de los campos de texto de la interfaz
                comandoSql.Parameters.AddWithValue("@IdPaciente", ListaPacientes.SelectedValue);
                comandoSql.Parameters.AddWithValue("@DoctorResponsable", ListaDoctores.SelectedValue);
                comandoSql.Parameters.AddWithValue("@Habitacion", txbHabitacion.Text);
                comandoSql.Parameters.AddWithValue("@Cama", txbCama.Text);

                /**********************************************************************/
                /**********************************************************************/

                // Obtener la fecha y hora de TxbFechaAlta y TxbHoraAlta
                DateTime? fechaAlta = DateTime.TryParse(TxbFechaAlta.Text, out var fechaAltaValue) ? fechaAltaValue : (DateTime?)null;
                if (fechaAlta.HasValue)
                {
                    TimeSpan? horaAlta = TxbHoraAlta.SelectedTime?.TimeOfDay;

                    // Combinar la fecha y hora en un solo valor DateTime
                    DateTime fechaAltaCompleta = fechaAlta.Value.Date + (horaAlta ?? TimeSpan.Zero);

                    comandoSql.Parameters.AddWithValue("@FechaAlta", fechaAltaCompleta);
                }
                else
                {
                    comandoSql.Parameters.AddWithValue("@FechaAlta", DBNull.Value);
                }

                // Obtener la fecha y hora de TxbFechaIngreso y TxbHoraIngreso
                DateTime? fechaIngreso = DateTime.TryParse(TxbFechaIngreso.Text, out var fechaIngresoValue) ? fechaIngresoValue : (DateTime?)null;
                if (fechaIngreso.HasValue)
                {
                    TimeSpan? horaIngreso = TxbHoraIngreso.SelectedTime?.TimeOfDay;

                    // Combinar la fecha y hora en un solo valor DateTime
                    DateTime fechaIngresoCompleta = fechaIngreso.Value.Date + (horaIngreso ?? TimeSpan.Zero);

                    comandoSql.Parameters.AddWithValue("@Fechaingreso", fechaIngresoCompleta);
                }
                else
                {
                    comandoSql.Parameters.AddWithValue("@Fechaingreso", DBNull.Value);
                }


                /**********************************************************************/
                /**********************************************************************/

                comandoSql.ExecuteNonQuery();

                miConexionSql.Close();

                // Actualizar Metodos
                MuestraTodosHospitalizacion();
                MuestraCama();
                MuestraDoctoresAsignados();
                MuestraPacientesAsignados();

                txbCama.Text = "";
                txbHabitacion.Text = "";
                TxbFechaAlta.Text = "";
                TxbFechaIngreso.Text = "";
                TxbHoraAlta.Text = "";
                TxbHoraIngreso.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el HOSPITALIZACIÓN: " + ex.Message);
            }
        }

        /********************************************************************/
        /********************************************************************/


        /***** MODIFICAR HOSPITALIZACIÓN **************/
        private void BtnModificarHopitalizacion_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que la conexión esté abierta antes de ejecutar la consulta
            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            // Verificar que haya un OBJETO seleccionado en TodosTratamientos
            if (TodosHospitalizacionDoctor.SelectedItem == null || !(TodosHospitalizacionDoctor.SelectedItem is DataRowView selectedRow))
            {
                MessageBox.Show("Por favor, selecciona una HOSPITALIZACION para modificar.");
                return;
            }

            // ID del OBJETO seleccionado
            int idHospitalizacionPaciente = Convert.ToInt32(selectedRow["ID"]);

            string consulta = "UPDATE HOSPITALIZACION SET HABITACION = @Habitacion, CAMA = @Cama, FECHA_INGRESO = @Fechaingreso, FECHA_ALTA = @FechaAlta WHERE ID = @IdTratamiento";

            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            // Obtener los valores de los campos de texto de la interfaz
            comandoSql.Parameters.AddWithValue("@Habitacion", txbHabitacion.Text);
            comandoSql.Parameters.AddWithValue("@Cama", txbCama.Text);

            comandoSql.Parameters.AddWithValue("@IdTratamiento", idHospitalizacionPaciente);

            /**********************************************************************/
            /**********************************************************************/
            DateTime fechaAlta = TxbFechaAlta.SelectedDate.Value;
            DateTime horaAlta = TxbHoraAlta.SelectedTime.Value;

            // Combinar la fecha y la hora en un solo valor DateTime
            DateTime fechaAltaCompleta = fechaAlta.Date.Add(horaAlta.TimeOfDay);

            comandoSql.Parameters.AddWithValue("@FechaAlta", fechaAltaCompleta);
            /**********************************************************************/
            /**********************************************************************/

            DateTime fechaIngreso = TxbFechaIngreso.SelectedDate.Value;
            DateTime horaIngreso = TxbHoraIngreso.SelectedTime.Value;

            // Combinar la fecha y la hora en un solo valor DateTime
            DateTime fechaIngresoCompleta = fechaIngreso.Date.Add(horaIngreso.TimeOfDay);

            comandoSql.Parameters.AddWithValue("@Fechaingreso", fechaIngresoCompleta);
            /**********************************************************************/
            /**********************************************************************/


            comandoSql.ExecuteNonQuery();

            miConexionSql.Close();

            MessageBox.Show("Se modificaron los datos correctamente. Gracias por utilizar la aplicación");

            // Actualizar Metodos

            MuestraTodosHospitalizacionDoctor();
            MuestraCamaParaPacientesAsignados();
            MuestraPacientesAsignados();
            //MuestraTodosHospitalizacion();
            //MuestraCama();

            // Limpiar los campos después de modificar el tratamiento
            txbCama.Text = "";
            txbHabitacion.Text = "";
            TxbFechaAlta.Text = "";
            TxbFechaIngreso.Text = "";
            TxbHoraAlta.Text = "";
            TxbHoraIngreso.Text = "";
        }

        /********************************************************************/
        /********************************************************************/

        /**** LISTAS ****/


        /***** DATOS DE HOSPITALIZACION DE PACIENTES PARA EDITARLOS *********************/
        /*private void TodosHospitalizacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verificar que haya un OBJETO seleccionado en TodosHospitalizacion
            if (TodosHospitalizacion.SelectedItem == null || !(TodosHospitalizacion.SelectedItem is DataRowView selectedRow))
            {
                // Si no hay un elemento seleccionado o el tipo de elemento no es DataRowView, limpiar los campos y salir del evento.
                txbHabitacion.Text = "";
                txbCama.Text = "";
                TxbFechaIngreso.Text = "";
                TxbFechaAlta.Text = "";
                return;
            }

            if (miConexionSql.State != ConnectionState.Open)
            {
                miConexionSql.Open();
            }

            string consulta = "SELECT HABITACION, CAMA, FECHA_INGRESO, FECHA_ALTA FROM HOSPITALIZACION WHERE ID = @idTratamiento";

            SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

            try
            {
                int idTratamiento = Convert.ToInt32(selectedRow["ID"]);
                miSqlCommand.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                DataTable tratamientoTabla = new DataTable();

                miAdaptadorSql.Fill(tratamientoTabla);

                txbHabitacion.Text = tratamientoTabla.Rows[0]["HABITACION"].ToString();
                txbCama.Text = tratamientoTabla.Rows[0]["CAMA"].ToString();
                TxbFechaIngreso.Text = tratamientoTabla.Rows[0]["FECHA_INGRESO"].ToString();
                TxbFechaAlta.Text = tratamientoTabla.Rows[0]["FECHA_ALTA"].ToString();

                MessageBox.Show("Se mostrarán los detalles de la HOSPITALIZACIÓN. Puedes modificarlos y luego pulsar MODIFICAR. Gracias por utilizar la aplicación.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener detalles de la HOSPITALIZACIÓN: " + ex.Message);
            }
            finally
            {
                miConexionSql.Close();
            }
        }*/

        /***** DATOS DE HOSPITALIZACION DE PACIENTES DE DOCTORES PARA EDITARLOS *********************/
        private void TodosHospitalizacionDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                // Verificar que haya un OBJETO seleccionado en TodosHospitalizacion
                if (TodosHospitalizacionDoctor.SelectedItem == null || !(TodosHospitalizacionDoctor.SelectedItem is DataRowView selectedRow))
                {
                    // Si no hay un elemento seleccionado o el tipo de elemento no es DataRowView, limpiar los campos y salir del evento.
                    txbHabitacion.Text = "";
                    txbCama.Text = "";
                    TxbFechaIngreso.Text = "";
                    TxbFechaAlta.Text = "";
                    TxbHoraIngreso.Text = "";
                    TxbHoraAlta.Text = "";
                    return;
                }

                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                string consulta = "SELECT HABITACION, CAMA, FECHA_INGRESO, FECHA_ALTA FROM HOSPITALIZACION WHERE ID = @idTratamiento";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

                try
                {
                    int idTratamiento = Convert.ToInt32(selectedRow["ID"]);
                    miSqlCommand.Parameters.AddWithValue("@idTratamiento", idTratamiento);

                    DataTable tratamientoTabla = new DataTable();

                    miAdaptadorSql.Fill(tratamientoTabla);

                    txbHabitacion.Text = tratamientoTabla.Rows[0]["HABITACION"].ToString();
                    txbCama.Text = tratamientoTabla.Rows[0]["CAMA"].ToString();
                    //TxbFechaIngreso.Text = tratamientoTabla.Rows[0]["FECHA_INGRESO"].ToString();
                    //TxbFechaAlta.Text = tratamientoTabla.Rows[0]["FECHA_ALTA"].ToString();

                    /*****************************************************************************************/
                    /*****************************************************************************************/

                    TxbFechaIngreso.SelectedDate = ((DateTime)tratamientoTabla.Rows[0]["FECHA_INGRESO"]).Date;
                    TxbHoraIngreso.SelectedTime = ((DateTime)tratamientoTabla.Rows[0]["FECHA_INGRESO"]);

                    TxbFechaAlta.SelectedDate = ((DateTime)tratamientoTabla.Rows[0]["FECHA_ALTA"]).Date;
                    TxbHoraAlta.SelectedTime = ((DateTime)tratamientoTabla.Rows[0]["FECHA_ALTA"]);

                    /*****************************************************************************************/
                    /*****************************************************************************************/

                    MessageBox.Show("Se mostrarán los detalles de la HOSPITALIZACIÓN. Puedes modificarlos y luego pulsar MODIFICAR. Gracias por utilizar la aplicación.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener detalles de la HOSPITALIZACIÓN: " + ex.Message);
                }
                finally
                {
                    miConexionSql.Close();
                }
            }
        }

        /********************************************************************/
        /********************************************************************/
        private void ListaPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListaCamaAsignadaPacienteDoctor.ItemsSource = null;
            //ListaPacientesAsignados.ItemsSource = null;
            TodosHospitalizacion.ItemsSource = null;
            MuestraTodosHospitalizacion();
            //MuestraCama();
            //MuestraDoctoresAsignados();
        }
        /********************************************************************/
        /********************************************************************/
        private void ListaDoctores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListaDoctoresAsignados.ItemsSource = null;
            //ListaCamaAsignada.ItemsSource = null;
            MuestraTodosHospitalizacionDoctorAsignado();
            TodosHospitalizacionDoctor.ItemsSource = null;
            //MuestraCamaParaPacientesAsignados();
            //MuestraPacientesAsignados();

        }
        /********************************************************************/
        /********************************************************************/

        private void TodosHospitalizacionDoctorAsignado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraPacientesAsignados();
        }
        /********************************************************************/
        /********************************************************************/

        private void ListaCamaAsignada_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            //TodosHospitalizacion.ItemsSource = null;
            //MuestraTodosHospitalizacion();
        }
        /********************************************************************/
        /********************************************************************/
        private void ListaPacientesAsignados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TodosHospitalizacionDoctor.ItemsSource = null;

            MuestraCamaParaPacientesAsignados();
        }
        /********************************************************************/
        /********************************************************************/
        private void ListaCamaAsignadaPacienteDoctor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //ListaDoctoresAsignados.ItemsSource = null;
            //ListaCamaAsignada.ItemsSource = null;
            TodosHospitalizacionDoctor.ItemsSource = null;
            MuestraTodosHospitalizacionDoctor();
        }

        /********************************************************************/
        /********************************************************************/

        private void TodosHospitalizacion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraDoctoresAsignados();
            MuestraCama();
        }

        /********************************************************************/
        /********************************************************************/

        /**** BOTONES ****/

        private void BtnAgregraPaciente_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Paciente newPaciente = new Cpanel.Paciente(miConexionSql);
            newPaciente.Show();
        }
        /********************************************************************/
        /********************************************************************/
        private void BtnAgregarDoctor_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Cpanel.Doctor newDoctor = new Cpanel.Doctor(miConexionSql);
            newDoctor.Show();
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
    }
}
