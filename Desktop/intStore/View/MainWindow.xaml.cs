using intStore.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;





namespace intStore.View
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
            UpdatePlaceholderVisibility();
            UpdatePlaceholderVisibilityReg();
            authService = new AuthService();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string email = txtFilterEmail.Text;
            string password = passwordBox.Password;

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
                MessageBox.Show("Попробуйте войти заново, неправильный логин или пароль", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
         
        private void RegBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtFilterUsernameReg.Text;
            string email = txtFilterEmailReg.Text;
            string password = txtFilterPasswordReg.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля обязательны для заполнения!");
                return;
            }
            
            if(!IsValidUserName(username))
            {
                MessageBox.Show("Некорректное имя. Пример: Max");
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Некорректный email. Пример: max@gmail.com");
                return;
            }

            if (!IsValidPassword(password))
            {
                MessageBox.Show("Некорректный пароль. Пример: *Mm12311");
                return;
            }

            bool registrationSuccessful = authService.Register(username, email, password);
            if (registrationSuccessful)
            {
                MessageBox.Show("Регистрация прошла успешно!");
                ClearTextBox();
            }
        }

        private bool IsValidUserName(string username)
        {
            string pattern = @"^\S+$";
            return Regex.IsMatch(username, pattern);
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; 
            return Regex.IsMatch(email, pattern);
        }
        private bool IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"; 
            return Regex.IsMatch(password, pattern);
        }

        private void ClearTextBox()
        {
            txtFilterUsernameReg.Text = "";
            txtFilterEmailReg.Text = "";
            txtFilterPasswordReg.Password = "";
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
            UpdatePlaceholderVisibilityReg();
        }

        /// <summary>
        /// Обновляет видимость текста для поля ввода пароля при авторизации.
        /// Определяет, следует ли отображать текст в зависимости от того, пустое ли значение в поле ввода пароля.
        /// </summary>
        private void UpdatePlaceholderVisibility()
        {
            var placeholderText = (TextBlock)passwordBox.Template.FindName("PlaceholderText", passwordBox);
            if (placeholderText != null)
            {
                placeholderText.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Обновляет видимость текста для поля ввода пароля при регистрации.
        /// Определяет, следует ли отображать текст в зависимости от того, пустое ли значение в поле ввода пароля.
        /// </summary>
        private void UpdatePlaceholderVisibilityReg()
        {
            var placeholderText = (TextBlock)txtFilterPasswordReg.Template.FindName("PlaceholderText", txtFilterPasswordReg);
            if (placeholderText != null)
            {
                placeholderText.Visibility = string.IsNullOrEmpty(txtFilterPasswordReg.Password) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
