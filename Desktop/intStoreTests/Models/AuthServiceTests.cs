using Microsoft.VisualStudio.TestTools.UnitTesting;
using intStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intStore.Models.Tests
{
    [TestClass()]
    public class AuthServiceTests
    {
        [TestMethod()]
        public void RegisterTest()
        {
            AuthService authService = new AuthService();
            //Предусловие 
            string name = "Elena";
            string email = "eleds2345@gmail.com";
            string password = "*Mm1234511";
            bool expected = true;

            //Действие 
            bool actual = authService.Register(name, email, password);

            //Утверждение 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void LoginTest()
        {
            Assert.Fail();
        }
    }
}