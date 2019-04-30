using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Xunit;
//using Xunit.Abstractions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;   
using OpenQA.Selenium.Chrome;    
using OpenQA.Selenium.IE;
//using Xunit.Sdk;

[TestClass]
public class SeleniumTests// : IDisposable
{
    private IWebDriver driver;
    //private readonly ITestOutputHelper output;

    public SeleniumTests()
    {

    }

    [TestInitialize()]
    public void TestInitialize()//ITestOutputHelper output)
    {
        //this.output = output;

        string appURL = "https://sandylinux.azurewebsites.net";


        var options = new FirefoxOptions();
        string geckoPath = Environment.GetEnvironmentVariable("GeckoWebDriver");
        //output.WriteLine(geckoPath);                // comment out deze als je zelf wil testen
        driver = new FirefoxDriver(geckoPath);      // deze ook
        //driver = new FirefoxDriver();             // en gebruik deze
        driver.Navigate().GoToUrl(appURL);

        waitUntilCountElementEquals(10, "sandy_incoming_msg", 2);
    }

    [TestMethod]
    public void GreetingsTest()
    {
        string[] msgs = GetResponse("hoi");

        string expected = "Hallo!";
        string actual = msgs[0];
        Assert.AreEqual(expected, actual);

        UnknownQuestionTest();
    }

    [TestMethod]
    public void UnknownQuestionTest()
    {
        string[] msgs = GetResponse("Hoe heet ik?", 4);

        string expected = "Wil je een email adres opgeven?";
        string actual = msgs[3];
        Assert.AreEqual(expected, actual);
    }

    private string[] GetResponse(string message, int nExpectedResponseMsgs = 1)
    {
        int sandyMsgs = driver.FindElements(By.ClassName("sandy_incoming_msg")).Count;
        int totalMsgsExpected = sandyMsgs + nExpectedResponseMsgs;

        driver.FindElement(By.Id("messageInput")).SendKeys(message);
        driver.FindElement(By.Id("sendButton")).Click();

        waitUntilCountElementEquals(10, "sandy_incoming_msg", totalMsgsExpected);

        // test if we received the expected amount of response messages
        int totalMsgs = driver.FindElements(By.ClassName("sandy_incoming_msg")).Count;
        Assert.AreEqual(totalMsgs, totalMsgsExpected);

        string[] responses = new string[nExpectedResponseMsgs];
        for (int i = 0; i < responses.Length; i++)
        {
            IWebElement rWE = driver.FindElements(By.ClassName("sandy_incoming_msg"))[sandyMsgs + i];
            responses[i] = rWE.FindElement(By.XPath("div[2]/div/p")).Text;
        }

        return responses;
    }

    private void waitUntilCountElementEquals(int time, string element, int number) 
    {
        try 
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            wait.Until(drvr => drvr.FindElements(By.ClassName(element)).Count >= number);
        }
        catch (Exception e)
        {
            driver.Quit();
            throw new Exception("Error in waitUntil, probably a timeout", e);
        }
    }

    [TestCleanup()]
    public void Dispose()
    {
        driver.Quit();
    }
}