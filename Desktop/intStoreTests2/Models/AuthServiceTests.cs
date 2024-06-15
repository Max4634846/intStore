using Microsoft.VisualStudio.TestTools.UnitTesting;
using intStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using intStore.Data;

namespace intStore.Models.Tests
{
    [TestClass()]
    public class AuthServiceTests
    {
        
        [TestMethod()]
        public void Check_Symbols_ReturnRegisterTestTrue()
        {
            AuthService authService = new AuthService();
            //Предусловие 
            string name = "Elena";
            string email = "elena2345@gmail.com";
            string password = "*Mm1234511";
            bool expected = true;

            //Действие 
            bool actual = authService.Register(name, email, password);

            //Утверждение 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void Check_Symbols_ReturnRegisterTestFalse()
        {
            AuthService authService = new AuthService();
            //Предусловие 
            string name = "Elena";
            string email = "elena2345@";
            string password = "*Mm12311";

            //Действие 
            bool actual = authService.Register(name, email, password);

            //Утверждение 
            Assert.IsFalse(actual);
        }

        
        [TestMethod()]
        public void Login_WithValidCredentials_ShouldReturnTrue()
        {
            AuthService authService = new AuthService();
            //Предусловие 
            string email = "max@gmail.com";
            string password = "*Mm12311";
            Customers expected = new Customers { Email = email, Password = password };

            //Действие
            Customers actual = authService.Login(email, password);

            //Утверждение 
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Email, actual.Email);
            Assert.AreEqual(expected.Password, actual.Password);
        }
    }
}