using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Numerics;
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

namespace AppForCourseWork
{
    /// <summary>
    /// Логика взаимодействия для Results.xaml
    /// </summary>
    public partial class Results : Window
    {
        private bool isAdmin;
        private bool isUser;
        private string connectionString;
        private int userID;
        private string full_name;
        string departCity;
        string destCity;
        DateTime departTime;
        public Results(string depCity, string desCity, DateTime departureDate,bool isAdm, bool isUsr, int usrID, string fname)
        {
            departCity = depCity;
            destCity = desCity;
            departTime = departureDate;
            isAdmin = isAdm;
            isUser = isUsr;
            full_name = fname;
            userID = usrID;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hiring_date_dpicker.DisplayDateEnd = DateTime.Now.AddDays(-10);
            hiring_date_dpicker.SelectedDate = DateTime.Now.AddDays(-15);
            hiring_date_dpicker.DisplayDateStart = DateTime.Now.AddYears(-20);
            Flight_to_add_dp.DisplayDateStart = DateTime.Now.AddDays(10);
            foreach (Positions position in Enum.GetValues(typeof(Positions)))
            {
                Positions_combo.Items.Add(position);
            }
            string connectionString1 = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
            if (isUser)
            {
                panel_bar.Visibility = Visibility.Visible;
                this.Acc_name.Text = full_name;
            }
            else if (isAdmin)
            {
                Enter_to_interaction.Visibility = Visibility.Hidden;
                admin_panel_bar.Visibility= Visibility.Visible;
                connectionString1 = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
                this.Acc_name.Text = full_name;
            }
            using (OracleConnection connection = new OracleConnection(connectionString1))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SEARCH_FLIGHTS", connection))
                {
                    command.BindByName = true;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("departure_city", OracleDbType.NVarchar2).Value = departCity;
                    command.Parameters.Add("destination_city", OracleDbType.NVarchar2).Value = destCity;
                    command.Parameters.Add("dep_date", OracleDbType.Date).Value = departTime;

                    OracleParameter resultParam = new OracleParameter("p_resflights", OracleDbType.RefCursor);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);

                    command.ExecuteNonQuery();

                    using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                    {
                        if (reader.HasRows)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);
                            dg_search_result.ItemsSource = dataTable.DefaultView;
                        }
                        else
                        {
                            MessageBox.Show("Нет доступных рейсов");
                        }
                    }


                }
            }
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_PLANE", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_plane_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                int planeId = reader.GetInt32(0); // Получаем значение столбца plane_id

                                // Добавляем значение в ComboBox
                                Plane_id_to_add_combo.Items.Add(planeId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }

        }

        private void Show_ticket_history_Click(object sender, RoutedEventArgs e)
        {
                string connectionString1 = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
                
            if(isAdmin)
                connectionString1 = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;

            using (OracleConnection connection = new OracleConnection(connectionString1))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("System.GET_TICKETS_BY_CUSTOMER", connection))
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;
                        if (isAdmin)
                        {
                            command.Parameters.Add("p_customer_id", OracleDbType.Int64).Value = Convert.ToInt64(admin_ID_to_search_tb.Text);
                        }
                        else
                        {
                            command.Parameters.Add("p_customer_id", OracleDbType.Int64).Value = this.userID;
                        }

                        OracleParameter resultParam = new OracleParameter("p_tickets", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                if (isUser)
                                {
                                    this.dg_ticket_history.Visibility = Visibility.Visible;
                                    DataTable dataTable = new DataTable();
                                    dataTable.Load(reader);
                                    dg_ticket_history.ItemsSource = dataTable.DefaultView;
                                }

                                else if (isAdmin)
                                {
                                    this.dg_user_ticket_history.Visibility = Visibility.Visible;
                                    DataTable dataTable = new DataTable();
                                    dataTable.Load(reader);
                                    dg_user_ticket_history.ItemsSource = dataTable.DefaultView;

                                }
                            }
                            else
                            {
                                MessageBox.Show("Билетная история пуста");
                            }
                        }
                    }
                }
            
            

                

           
            
        }


        //registration/authorization
        private void Open_Register_Window_Click(object sender, RoutedEventArgs e)
        {
            this.panel_bar.Visibility = Visibility.Hidden;
            this.dg_search_result.Visibility = Visibility.Hidden;
            this.Registration_Window.Visibility = Visibility.Visible;
            this.Login_window.Visibility = Visibility.Hidden;
            Enter_to_interaction.Visibility = Visibility.Hidden;
            admin_panel_bar.Visibility = Visibility.Hidden;
        }

        private void Close_reg_window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Hidden;
            if(!isAdmin && !isUser)
            {
                Enter_to_interaction.Visibility = Visibility.Visible;
            }
            if (isUser)
            {
                this.panel_bar.Visibility = Visibility.Visible;
                Enter_to_interaction.Visibility = Visibility.Visible;
            }
            else if (isAdmin)
            {
                admin_panel_bar.Visibility = Visibility.Visible;
            }
            this.dg_search_result.Visibility = Visibility.Visible;

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Visible;
            this.panel_bar.Visibility = Visibility.Hidden;
            this.dg_search_result.Visibility = Visibility.Hidden;
            this.Enter_to_interaction.Visibility = Visibility.Hidden;
            this.admin_panel_bar.Visibility = Visibility.Hidden;
        }

        private void Enter_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isAdmin || isUser)
            {
                this.Acc_name.Text = "";
                isAdmin = false;
                isUser = false;
                userID = 0;
            }

            if (Login_tb.Text.ToLower() == "manager")
            {
                try
                {
                    string connectionString2 = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
                    string passwordFromConfig = new OracleConnectionStringBuilder(connectionString2).Password;
                    string enteredPassword = this.Passw_box.Password;
                    if (enteredPassword == passwordFromConfig)
                    {
                        isAdmin = true;
                        full_name = "Manager";
                        this.Acc_name.Text = full_name;
                        this.Login_window.Visibility = Visibility.Hidden;
                        this.dg_search_result.Visibility = Visibility.Visible;

                        panel_bar.Visibility = Visibility.Hidden;
                        Enter_to_interaction.Visibility = Visibility.Hidden;
                        admin_panel_bar.Visibility = Visibility.Visible;

                        MessageBox.Show("Добро пожаловать!", "Вход персонала");
                    }
                    else
                    {
                        MessageBox.Show("Неверный пароль!", "Внимание");
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Login_tb.Text = "";
                    Passw_box.Password = null;
                }
            }
            else
            {
                string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("System.CheckUserByPassportSeries", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Параметры процедуры
                        command.Parameters.Add("pPassportSeries", OracleDbType.NVarchar2).Value = this.Login_tb.Text;


                        // Выходной параметр
                        OracleParameter resultParam = new OracleParameter("pResult", OracleDbType.Int32);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        int result = 0;
                        if (resultParam.Value is OracleDecimal oracleDecimal)
                        {
                            result = oracleDecimal.ToInt32();
                        }

                        if (result == 1)
                        {
                            string connectionString2 = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
                            string passwordFromConfig = new OracleConnectionStringBuilder(connectionString2).Password;
                            string enteredPassword = this.Passw_box.Password;
                            if (enteredPassword == passwordFromConfig)
                            {
                                using (OracleCommand command2 = new OracleCommand("System.GetUserInfoByPassportSeries", connection))
                                {
                                    command2.CommandType = CommandType.StoredProcedure;

                                    // Параметры процедуры
                                    command2.Parameters.Add("pPassportSeries", OracleDbType.NVarchar2).Value = this.Login_tb.Text;

                                    // Выходные параметры
                                    OracleParameter customerIdParam = new OracleParameter("pCustomerId", OracleDbType.Int64);
                                    customerIdParam.Direction = ParameterDirection.Output;
                                    command2.Parameters.Add(customerIdParam);

                                    OracleParameter fullNameParam = new OracleParameter("pFullName", OracleDbType.NVarchar2, 100);
                                    fullNameParam.Direction = ParameterDirection.Output;
                                    command2.Parameters.Add(fullNameParam);

                                    command2.ExecuteNonQuery();

                                    if (customerIdParam.Value is OracleDecimal oracleDecimal1)
                                    {
                                        userID = oracleDecimal1.ToInt32();
                                    }

                                    full_name = "";

                                    if (fullNameParam.Value is OracleString oracleString)
                                    {
                                        full_name = oracleString.ToString();
                                    }
                                    //userID = customerIdParam.Value != DBNull.Value ? Convert.ToInt32(customerIdParam.Value) : 0;
                                    /* string full_name = fullNameParam.Value != DBNull.Value ? fullNameParam.Value.ToString() : string.Empty;*/
                                    isUser = true;
                                    this.Acc_name.Text = full_name;
                                    this.Login_tb.Text = "";
                                    this.Passw_box.Password = "";
                                    dg_ticket_history.ItemsSource = null;
                                    dg_ticket_history.Visibility = Visibility.Hidden;
                                    this.Login_window.Visibility = Visibility.Hidden;
                                    Enter_to_interaction.Visibility = Visibility.Hidden;
                                    this.panel_bar.Visibility = Visibility.Visible;
                                    this.dg_search_result.Visibility = Visibility.Visible;
                                    this.admin_panel_bar.Visibility = Visibility.Hidden;
                                    MessageBox.Show($"Вы успешно вошли как пользователь\n{this.userID} {this.full_name}", "Вход подтверждён");
                                }

                            }
                            else
                            {
                                MessageBox.Show("Неверный пароль", "Вход отклонён!");
                                this.Passw_box.Password = "";
                            }
                        }
                        else
                        {
                            MessageBox.Show("Такой пользователь не существует", "Проверьте ввод!");
                            this.Passw_box.Password = "";
                            this.Login_tb.Text = "";
                        }
                    }
                    connection.Close();
                }
            }
        }

        private void Close_login_window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Hidden;
            if(!isUser && !isAdmin)
            {
                this.Enter_to_interaction.Visibility = Visibility.Visible;
            }
            if (isUser)
            {
                this.panel_bar.Visibility = Visibility.Visible;
                this.Enter_to_interaction.Visibility = Visibility.Visible;
            }
            if (isAdmin)
            {
                this.admin_panel_bar.Visibility = Visibility.Visible;
            }
            this.dg_search_result.Visibility = Visibility.Visible;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.CheckUserByPassportSeries", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Параметры процедуры
                    command.Parameters.Add("pPassportSeries", OracleDbType.NVarchar2).Value = this.Passport_tb.Text;


                    // Выходной параметр
                    OracleParameter resultParam = new OracleParameter("pResult", OracleDbType.Int32);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);

                    command.ExecuteNonQuery();

                    int result = 0;
                    if (resultParam.Value is OracleDecimal oracleDecimal)
                    {
                        result = oracleDecimal.ToInt32();
                    }

                    if (result == 0)
                    {
                        try
                        {
                            using (OracleCommand cmd = new OracleCommand("System.ADD_CUSTOMER", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("p_cust_fullname", OracleDbType.NVarchar2).Value = Full_name_tb.Text;
                                cmd.Parameters.Add("p_birht_date", OracleDbType.Date).Value = Bday_registration.SelectedDate;
                                cmd.Parameters.Add("p_passport_series", OracleDbType.NVarchar2).Value = Passport_tb.Text;

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Вы успешно зарегистрировались", "Поздравляем!");
                                this.Registration_Window.Visibility = Visibility.Hidden;
                                if (isUser)
                                {
                                    this.panel_bar.Visibility = Visibility.Visible;
                                }
                                else if (isAdmin)
                                {
                                    this.admin_panel_bar.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    panel_bar.Visibility = Visibility.Hidden;
                                    this.admin_panel_bar.Visibility = Visibility.Hidden;
                                }
                               
                                this.dg_search_result.Visibility = Visibility.Visible;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Full_name_tb.Text = "";
                            Bday_registration.SelectedDate = DateTime.Now.AddYears(-18);
                            Passport_tb.Text = "";
                        }

                    }
                    else
                    {
                        MessageBox.Show("Такой пользователь уже существует", "В регистраии отказано!");
                    }
                }
                connection.Close();
            }

        }


        //end of registration/authorization



        private void Book_ticket_Click(object sender, RoutedEventArgs e)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.BookTicket", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("flight_num", OracleDbType.Int64).Value = Convert.ToInt64(Book_flight_tb.Text);
                        command.Parameters.Add("customer_id", OracleDbType.Int64).Value = Convert.ToInt64(this.userID);
                        if (Luggage_tb.Text == null || Luggage_tb.Text == " " || Luggage_tb.Text == "")
                        {
                            command.Parameters.Add("luggage_weight", OracleDbType.Int64).Value = DBNull.Value;
                        }
                        else
                            command.Parameters.Add("luggage_weight", OracleDbType.Int64).Value = Convert.ToInt64(Luggage_tb.Text);


                        command.ExecuteNonQuery();
                        MessageBox.Show($"Вы забронировали билет на рейс {Book_flight_tb.Text}");

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Luggage_tb.Text = "";
                        Book_flight_tb.Text = "";
                    }
                }
            }

        }

        private void Refund_ticket_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["User"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.CANCEL_TICKET", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_ticket_num", OracleDbType.Int64).Value = Convert.ToInt64(Refund_ticket_tb.Text);
                        command.Parameters.Add("p_customer_id", OracleDbType.Int64).Value = Convert.ToInt64(this.userID);

                        command.ExecuteNonQuery();
                        MessageBox.Show($"Вы оформили возврат билета");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Refund_ticket_tb.Text = "";

                    }
                }
            }

        }

        private void Cancel_flight_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.CANCEL_FLIGHT", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_flight_num", OracleDbType.Int64).Value = Convert.ToInt64(Flight_num_to_cancel_tb.Text);
                        command.ExecuteNonQuery();
                        MessageBox.Show($"Рейс был отменён");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Flight_num_to_cancel_tb.Text = "";
                    }
                }
            }
        }
        private void Check_flights_btn_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.CHECK_FLIGHTS", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                        MessageBox.Show("Рейсы были проверены");
                    }
                    catch(Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void Show_personel_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_PEROSNEL", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_personel_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица персонала пуста");
                            }
                        }
                    }
                    catch(  Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void slider_salary_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           /* string x = slider_salary.Value.ToString();
            this.Salary_tblock.Text = x;*/
        }

        private void Minus_employee_btn_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.REMOVE_EMPLOYEE", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_empl_id", OracleDbType.Int64).Value = Empl_id_to_minus_tb.Text.ToString();

                        command.ExecuteNonQuery();
                        MessageBox.Show("Работник был сокращён");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Empl_id_to_minus_tb.Text = "";
                    }
                }
            }
        }

        private void Hire_employee_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.ADD_EMPLOYEE", connection))
                {
                    try
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_empl_fname", OracleDbType.NVarchar2).Value = Employee_fname_tb.Text;
                        var selectedPosition = Positions_combo.SelectedItem as Positions?;
                        if (selectedPosition != null)
                        {
                            Int64 positionValue = (Int64)selectedPosition.Value; // Приведение к типу int
                            command.Parameters.Add("p_empl_position", OracleDbType.Int64).Value = positionValue;
                            // Используйте значение positionValue в вашем коде
                        }
                        else
                        {
                            command.Parameters.Add("p_empl_position", OracleDbType.Int64).Value = (Int64)3;
                        }
                        command.Parameters.Add("p_salary", OracleDbType.Decimal).Value = (decimal)slider_salary.Value;
                        command.Parameters.Add("p_start_work_date", OracleDbType.Date).Value = hiring_date_dpicker.SelectedDate;

                        command.ExecuteNonQuery();
                        MessageBox.Show($"Сотрудник {Employee_fname_tb.Text} успешно нанят на должность {Positions_combo.SelectedValue.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        Employee_fname_tb.Text = "";
                        slider_salary.Value = slider_salary.Minimum;
                    }
                }
            }
        }

        private void Show_planes_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_PLANE", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_plane_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица самолётов пуста");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void Show_client_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_CUSTOMER", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_customer_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица клиентов пуста");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void Show_completed_flights_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_COMPLETED_FLIGHTS", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_compl_flight_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица истории рейсов пуста");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }


        }

        private void Show_20_nearest_flights_Click(object sender, RoutedEventArgs e)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_NEAREST_FLIGHTS", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_nearest_flights_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица пуста");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void Show_airports_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.SHOW_AIRPORTS", connection))
                {
                    try
                    {
                        command.BindByName = true;
                        command.CommandType = CommandType.StoredProcedure;

                        OracleParameter resultParam = new OracleParameter("p_airports_list", OracleDbType.RefCursor);
                        resultParam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParam);

                        command.ExecuteNonQuery();

                        using (OracleDataReader reader = ((OracleRefCursor)resultParam.Value).GetDataReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                dg_search_result.ItemsSource = dataTable.DefaultView;
                            }
                            else
                            {
                                MessageBox.Show("Таблица пуста");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
        }

        private void Add_flight_btn_Click(object sender, RoutedEventArgs e)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["Manager"].ConnectionString;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("System.ADD_FLIGHT", connection))
                {
                    try
                    {
                        int planeId = 1001;
                        var selectedPlaneId = Plane_id_to_add_combo.SelectedItem as int?;
                        if (selectedPlaneId != null)
                        {
                            planeId = selectedPlaneId.Value;
                        }
                        DateTime? dt = Flight_to_add_dp.SelectedDate;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("p_plane_id", OracleDbType.Int32).Value = (Int32)planeId;
                        command.Parameters.Add("p_dep_airport_id", OracleDbType.Int64).Value = Convert.ToInt64(this.dep_airport_to_add_tb.Text);
                        command.Parameters.Add("p_dest_airport_id", OracleDbType.Int64).Value = Convert.ToInt64(this.dest_airport_to_add_tb.Text);
                        command.Parameters.Add("dest_airport_to_add_tb", OracleDbType.Date).Value = dt;
                        command.Parameters.Add("p_dest_datetime", OracleDbType.Date).Value = dt.GetValueOrDefault().AddHours(Convert.ToInt32(Dutration_of_flight_tb.Text));

                        command.ExecuteNonQuery();
                        MessageBox.Show($"Добавлен новый рейс");

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        dep_airport_to_add_tb.Text = "";
                        dep_airport_to_add_tb.Text = "";
                        Dutration_of_flight_tb.Text = "";
                    }
                }
            }
        }

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!Char.IsLetter(c) && c != ' ' && c != '-')
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void TextBlock_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!Char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }

        }
    }
}