using Demo_var_6;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MainWindowTests
{
    [TestMethod]
    public void Login_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        bool result = mainWindow.Login("validUsername", "validPassword");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Login_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        bool result = mainWindow.Login("invalidUsername", "invalidPassword");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Login_NullOrEmptyUsernameOrPassword_ReturnsFalse()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        bool resultEmptyUsername = mainWindow.Login("", "validPassword");
        bool resultEmptyPassword = mainWindow.Login("validUsername", "");

        // Assert
        Assert.IsFalse(resultEmptyUsername);
        Assert.IsFalse(resultEmptyPassword);
    }

    [TestMethod]
    public void Login_CorrectCredentials_ReturnsTrue()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        bool result = mainWindow.Login("correctUsername", "correctPassword");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Login_IncorrectCredentials_ReturnsFalse()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        bool result = mainWindow.Login("incorrectUsername", "incorrectPassword");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GenerateCaptcha_CalledTwice_CaptchaTextChanges()
    {
        // Arrange
        MainWindow mainWindow = new MainWindow();

        // Act
        mainWindow.GenerateCaptcha();
        string captchaText1 = GetCaptchaText(mainWindow);
        mainWindow.GenerateCaptcha();
        string captchaText2 = GetCaptchaText(mainWindow);

        // Assert
        Assert.AreNotEqual(captchaText1, captchaText2);
    }

   
    private string GetCaptchaText(MainWindow mainWindow)
    {
        
        var field = typeof(MainWindow).GetField("captchaText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field.GetValue(mainWindow) as string;
    }

}
