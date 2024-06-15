using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using intStore.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;


namespace intStore.Models
{
    public class AuthService
    {
        public bool Register(string name, string email, string password)
        {
            using (var connection = new InternetStoreEntities1())
            {
                if (connection.Customers.Any(c => c.Email == email))
                {
                    MessageBox.Show("Данная почта уже используется");
                    return false;
                }

                if (!IsValidUserName(name))
                {
                    MessageBox.Show("Некорректное имя. Пример: Max");
                    return false;
                }

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Некорректный email. Пример: max@gmail.com");
                    return false;
                }

                if (!IsValidPassword(password))
                {
                    MessageBox.Show("Некорректный пароль. Пример: *Mm12311");
                    return false;
                }

                var customer = new Customers
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    RegisterDate = DateTime.Now
                };

                connection.Customers.Add(customer);
                connection.SaveChanges();

                return true;
            }
        }

        public  Customers Login(string email, string password)
        {
            using(var connection = new InternetStoreEntities1())
            {
                return connection.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
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
    }
}
