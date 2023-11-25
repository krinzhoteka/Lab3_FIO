using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Demo_var_6Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_ValidLogin_ValidPassword()
        {
            string username = "валидный_логин";
            string password = "валидный_пароль";
            Assert.IsTrue(Login(username, password));
        }
        [TestMethod]
        public void Test_InvalidLogin_ValidPassword()
        {
            string username = "неправильный_логин";
            string password = "валидный_пароль";
            Assert.IsFalse(Login(username, password));
        }
        [TestMethod]
        public void Test_ValidLogin_InvalidPassword()
        {
            string username = "валидный_логин";
            string password = "неправильный_пароль";
            Assert.IsFalse(Login(username, password));
        }
        [TestMethod]
        public void Test_InvalidLogin_InvalidPassword()
        {
            string username = "неправильный_логин";
            string password = "неправильный_пароль";
            Assert.IsFalse(Login(username, password));
        }

    }
}
