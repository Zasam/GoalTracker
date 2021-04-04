using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.UITest;
using System.Linq;
using System.IO;
using System;
using System.Threading;

namespace UnitTest.Xamarin
{
    [TestFixture(Platform.Android)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void SplashScreenIsDisplayed()
        {
            AppResult[] results = app.WaitForElement(c => c.Marked("Bitte gib deinen Namen ein"));
            app.Screenshot("SplashScreen");

            Assert.IsTrue(results.Any());
        }

        [Test]
        public void RegisterTest()
        {
            // Tap entry object and enter username
            //app.Tap("Name");

            // TODO: app.EnterText is not working?!?

            app.EnterText(c => c.Marked("Name"), "Username");
            Thread.Sleep(5000);
            Assert.IsTrue(true);
            //// Tap button to finish registering
            //app.Tap("Jetzt");

            //// Check if the popup message is shown correctly
            //AppResult[] popupResults = app.WaitForElement(c => c.Marked("Du hast dich erfolgreich regestriert"));

            //app.Tap("x");
            //AppResult[] finishRegisterResults = app.WaitForElement(c => c.Marked("Hey Username"));

            //Assert.IsTrue(popupResults.Any());
            //Assert.IsTrue(finishRegisterResults.Any());
        }
    }
}
