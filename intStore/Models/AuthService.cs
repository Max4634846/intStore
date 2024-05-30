using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace intStore.Models
{
    public class AuthService
    {
        private EmailService emailService = new EmailService();
        public bool Register(string name, string email, string password)
        {
            using (var connection = new InternetStoreEntities1())
            {
                if (connection.Customers.Any(c => c.Email == email))
                {
                    MessageBox.Show("Данная почта уже используется");
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

                emailService.SendEmailAsync(email, "Добро пожаловать в Интернет-магазин", $"Здравствуйте {name},\n\nСпасибо за регистрацию в нашем Интернет-магазине!");
                return true;
            }
        }

        public Customers Login(string email, string password)
        {
            using(var connection = new InternetStoreEntities1())
            {
                return connection.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
            }
        }
    }
}
