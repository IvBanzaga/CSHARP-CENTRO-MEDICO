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
    /// <summary>
    /// Lógica de interacción para Tratamiento.xaml
    /// </summary>
    public partial class Tratamiento : Window
    {
        SqlConnection miConexionSql;
        public Tratamiento(SqlConnection miConexionSql)
        {
            InitializeComponent();
            this.miConexionSql = miConexionSql;

            miConexion newConexion = new miConexion();
            string miConexion = newConexion.crearConexion();
            miConexionSql = new SqlConnection(miConexion);

            //MuestraDoctor();

            // Asociar el evento SelectionChanged al método ListaPacientes_SelectionChanged
            ListaPacientes.SelectionChanged += ListaPacientes_SelectionChanged;

            // Llamar a MuestraPaciente para cargar la lista inicial de pacientes
            MuestraPaciente();
        }

        /* LISTA DE DOCTORES */
        private void ListaDoctores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MuestraDatosDoctor();
        }

        private void ListaPacientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraTratamientoPaciente();
        }

        private void ListaTratamientos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraDatosPaciente();
        }

        /****** MUESTRA LISTA DE PACIENTES *******/

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

        /***** MUESTRA TRATAMIENTOS DE LOS PACIENTES ************/

        private void MuestraTratamientoPaciente()
        {
            // Obtener el ID del paciente seleccionado en la lista
            // int idPaciente = (int)ListaPacientes.SelectedValue;

            //string consulta = "SELECT Medicamento FROM TRATAMIENTO WHERE ID_PACIENTE = @idPaciente";

            string consulta = "SELECT CONCAT (ID, ' - ' ,Medicamento, ' ' ,Dosis, ' ' , Duracion) as infoPaciente FROM TRATAMIENTO";


            SqlCommand comandoSql = new SqlCommand(consulta, miConexionSql);

            
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(comandoSql);
            using (adaptadorSql)
            {
                comandoSql.Parameters.AddWithValue("@idPaciente", ListaTratamientos.SelectedValue);

                DataTable tratamientoTabla = new DataTable();
                adaptadorSql.Fill(tratamientoTabla);

                // Configurar el control para mostrar los tratamientos
                ListaTratamientos.DisplayMemberPath = "infoPaciente";
                ListaTratamientos.ItemsSource = tratamientoTabla.DefaultView;


               
            }
        }



        /* RESULTADO EN UNA LISTA DE DOCTORES*/

        /*private void MuestraDoctor()
        {

            string consulta = "SELECT Nombre, ID FROM DOCTOR";
            SqlDataAdapter adaptadorSql = new SqlDataAdapter(consulta, miConexionSql);
            using (adaptadorSql)
            {
                DataTable doctorTabla = new DataTable();
                adaptadorSql.Fill(doctorTabla);


                ListaDoctores.DisplayMemberPath = "Nombre";

                ListaDoctores.SelectedValuePath = "ID";
                ListaDoctores.ItemsSource = doctorTabla.DefaultView;
            }
        }*/

        /* MUESTRA DATOS DOCTORES*/
        /*private void MuestraDatosDoctor()
        {
            string consulta = @" SELECT P.Nombre AS Nombre, P.Apellido1 AS Apellido1, D.Nombre AS NombreDoctor, T.MEDICAMENTO, T.DOSIS, T.DURACION
                                FROM DOCTOR D
                                LEFT JOIN TRATAMIENTO T ON D.ID = T.ID_DOCTOR
                                LEFT JOIN PACIENTE P ON T.ID_PACIENTE = P.ID
                                WHERE D.ID = @ID";

            //string consulta = "SELECT CONCAT (ID, ' - ' ,Nombre, ' ' ,Apellido1, ' ' , Apellido2) as infoDoctor FROM DOCTOR";

            SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

            using (miAdaptadorSql)
            {
                string idSeleccionado = ListaDoctores.SelectedValue?.ToString();

                if (!string.IsNullOrEmpty(idSeleccionado))
                {
                    sqlComando.Parameters.AddWithValue("@ID", idSeleccionado);

                    DataTable datosTabla = new DataTable();
                    miAdaptadorSql.Fill(datosTabla);

                    // Asigna el valor del medicamento al TextBox txbTratamiento si hay resultados
                    if (datosTabla.Rows.Count > 0)
                    {
                        DataRow fila = datosTabla.Rows[0];
                        // Datos del paciente
                        string nombrePaciente = fila["Nombre"].ToString();
                        string apellidoPaciente = fila["Apellido1"].ToString();
                        txbNombrePaciente.Text = nombrePaciente;
                        txbApellido1Paciente.Text = apellidoPaciente;

                        // Doctor asignado al paciente
                        string doctorAsignado = fila["NombreDoctor"].ToString();
                        txbDoctorAsignado.Text = doctorAsignado;



                        // Datos del tratamiento
                        string medicamento = fila["MEDICAMENTO"].ToString();
                        string dosis = fila["DOSIS"].ToString();
                        string duracion = fila["DURACION"].ToString();
                        txbTratamiento.Text = medicamento;
                        txbDosis.Text = dosis;
                        txbDuracion.Text = duracion;
                    }

                    //ListaDoctores.ItemsSource = datosTabla.DefaultView;
                }
            }
        }*/

        /****** MUESTRA DATOS DE PACIENTES *******************/

        private void MuestraDatosPaciente()
        {
            string consulta = @" SELECT P.Nombre AS Nombre, P.Apellido1 AS Apellido1, D.Nombre AS NombreDoctor, T.MEDICAMENTO, T.DOSIS, T.DURACION, T.ID
                                FROM PACIENTE P
                                LEFT JOIN TRATAMIENTO T ON P.ID = T.ID_PACIENTE
                                LEFT JOIN DOCTOR D ON T.ID_DOCTOR = D.ID
                                WHERE P.ID = @ID";

            SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);
            SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

            using (miAdaptadorSql)
            {
                string idSeleccionado = ListaPacientes.SelectedValue?.ToString();

                if (!string.IsNullOrEmpty(idSeleccionado))
                {
                    sqlComando.Parameters.AddWithValue("@ID", idSeleccionado);

                    DataTable datosTabla = new DataTable();
                    miAdaptadorSql.Fill(datosTabla);

                    if (datosTabla.Rows.Count > 0)
                    {
                        DataRow fila = datosTabla.Rows[0];

                        // Datos del paciente
                        string nombrePaciente = fila["Nombre"].ToString();
                        string apellidoPaciente = fila["Apellido1"].ToString();
                        string idpaciente = fila["ID"].ToString();
                        txbNombrePaciente.Text = nombrePaciente;
                        txbApellido1Paciente.Text = apellidoPaciente;
                        Id_Paciente.Text = idpaciente;
                        // Doctor asignado al paciente
                        string doctorAsignado = fila["NombreDoctor"].ToString();
                        txbDoctorAsignado.Text = doctorAsignado;

                        // Datos del tratamiento
                        string medicamento = fila["MEDICAMENTO"].ToString();
                        string dosis = fila["DOSIS"].ToString();
                        string duracion = fila["DURACION"].ToString();
                        txbTratamiento.Text = medicamento;
                        txbDosis.Text = dosis;
                        txbDuracion.Text = duracion;
                    }

                    //DatosPacienteMedicacion.ItemsSource = datosTabla.DefaultView;
                }
            }
        }

        private void btn_volver_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Principal vovlerTratamiento = new Principal(miConexionSql);
            vovlerTratamiento.Show();
        }

        private void btn_salir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gracias por utilizar la aplicación realizada por Iván Bazaga ");
            App.Current.Shutdown();
        }

        private void btn_actualizar_Click(object sender, RoutedEventArgs e)
        {
           /* try
            {
                //int idPaciente = (int)ListaTratamientos.SelectedValue;

                string consulta = "SELECT Medicamento,Dosis,Duracion,ID FROM TRATAMIENTO where ID = @ID";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

               miSqlCommand.Parameters.AddWithValue("@ID", ListaTratamientos.SelectedValue);

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(miSqlCommand);

                using (miAdaptadorSql)
                {

                    
                    DataTable pacienteTabla = new DataTable();

                    miAdaptadorSql.Fill(pacienteTabla);

                    txbTratamiento.Text = pacienteTabla.Rows[0]["Medicamento"].ToString();
                    txbDosis.Text = pacienteTabla.Rows[0]["Dosis"].ToString();
                    txbDuracion.Text = pacienteTabla.Rows[0]["Duracion"].ToString();
                    Id_Paciente.Text = pacienteTabla.Rows[0]["ID"].ToString();


                }

            }

            catch (Exception ex)
            {
                // Manejar la excepción como sea necesario
                MessageBox.Show(ex.ToString());
            }*/
        }

        private void btn_modificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (miConexionSql.State != ConnectionState.Open)
                {
                    miConexionSql.Open();
                }

                string consulta = "UPDATE TRATAMIENTO SET Medicamento=@Medicamento, Dosis=@Dosis, Duracion=@Duracion WHERE ID=@IdTratamiento";

                SqlCommand miSqlCommand = new SqlCommand(consulta, miConexionSql);

                miSqlCommand.Parameters.AddWithValue("@Medicamento", txbTratamiento.Text);
                miSqlCommand.Parameters.AddWithValue("@Dosis", txbDosis.Text);
                miSqlCommand.Parameters.AddWithValue("@Duracion", txbDuracion.Text);

                // Obtener el valor del ID del tratamiento seleccionado desde el ListBox

              
               
                    miSqlCommand.Parameters.AddWithValue("@IdTratamiento", ListaTratamientos.SelectedValue);

                    miSqlCommand.ExecuteNonQuery();
                    miConexionSql.Close();

                    MuestraDatosPaciente();
                    MuestraPaciente();
              
            }
            catch (Exception ex)
            {
                // Manejar la excepción como sea necesario
                MessageBox.Show("Error al modificar el tratamiento: " + ex.Message);
            }
        }

    }
}
