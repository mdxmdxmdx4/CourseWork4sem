using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace AppForCourseWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isAdmin;
        private bool isUser;
        private string connectionString;
        private int userID = 0;
        private string full_name = "";
        string dep = "";
        string des = "";
        DateTime depTime;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dPicker.DisplayDateStart = DateTime.Now;
            dPicker.SelectedDate = DateTime.Today;
            Bday_registration.DisplayDateEnd = DateTime.Now.AddYears(-18);
            /*string connectionString = ConfigurationManager.ConnectionStrings["System"].ConnectionString;
            string query = "SELECT * FROM Personel";

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                OracleDataAdapter adapter = new OracleDataAdapter(query, connection);
                DataTable dataTable = new DataTable("Personel");
                adapter.Fill(dataTable);

                dgResult.ItemsSource = dataTable.DefaultView;
            }*/
        }

        private void departure_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!Char.IsLetter(c) && c != '-')
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void Open_Register_Window_Click(object sender, RoutedEventArgs e)
        {

            this.Registration_Window.Visibility = Visibility.Visible;
            this.Search_border.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Hidden;
        }

        private void Close_reg_window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Search_border.Visibility = Visibility.Visible;
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Hidden;

        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Search_border.Visibility = Visibility.Hidden;
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Visible;
        }

        private void Enter_btn_Click(object sender, RoutedEventArgs e)
        {
            if(isAdmin || isUser)
            {
                this.Acc_name.Text = "";
                isAdmin = false;
                isUser = false;
                userID = 0;
            }

            if (Login_tb.Text.ToLower() == "manager")
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
                    this.Search_border.Visibility = Visibility.Visible;
                    this.Login_tb.Text = "";
                    this.Passw_box.Password = null;
                    MessageBox.Show("Добро пожаловать!", "Вход персонала");
                }
                else
                {
                    MessageBox.Show("Неверный пароль!", "Внимание");
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
                                    this.Login_window.Visibility = Visibility.Hidden;
                                    this.Search_border.Visibility = Visibility.Visible;
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
            this.Search_border.Visibility = Visibility.Visible;
            this.Registration_Window.Visibility = Visibility.Hidden;
            this.Login_window.Visibility = Visibility.Hidden;
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
                                this.Search_border.Visibility = Visibility.Visible;
                            }
                        }
                        catch(Exception ex)
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            dep = departure.Text;
            des = destination.Text;
            depTime = Convert.ToDateTime(dPicker.Text);
            Results r = new Results(dep, des, depTime,isAdmin,isUser,userID,full_name);
            r.ShowDialog();
        }

        private void TextBlock_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
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
    }
}
