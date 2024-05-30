using intStore.Models;
using intStore.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using System.Web.UI.WebControls;

namespace intStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuthService authService;
        public MainWindow()
        {
            InitializeComponent();
            authService = new AuthService();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = txtFilterEmail.Text;
            string password = txtFilterPassword.Text;

            var customer = authService.Login(email, password);
            if (customer != null)
            {
                MessageBox.Show("Вход в систему прошел успешно!");
                var internetStore = new InternetStoreWindow(customer);
                internetStore.NameUserTextBlock.Text = customer.Name;
                internetStore.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный адрес электронной почты или пароль.");
            }
        }
         
        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtFilterUsernameReg.Text;
            string email = txtFilterEmailReg.Text;
            string password = txtFilterPasswordReg.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля обязательны для заполнения!");
                return;
            }

            bool registrationSuccessful = authService.Register(username, email, password);
            if (registrationSuccessful)
            {
                MessageBox.Show("Регистрация прошла успешно!");
                ClearTextBox();
            }
        }

        private void ClearTextBox()
        {
            txtFilterUsernameReg.Text = "";
            txtFilterEmailReg.Text = "";
            txtFilterPasswordReg.Text = "";
        }


        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }

        private void NextReg_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Reg";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }

        private void BackAuthor_Click(object sender, RoutedEventArgs e)
        {
            string tabName = "Author";

            foreach (TabItem tabItem in mainTabControl.Items)
            {
                if (tabItem.Name == tabName)
                {
                    mainTabControl.SelectedItem = tabItem;
                    break;
                }
            }
        }


    }
}
