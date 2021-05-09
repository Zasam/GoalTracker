using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

namespace GoalTracker.UITest
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class GoalPageTests
    {
        private IApp app;
        private readonly Platform platform;

        public GoalPageTests(Platform platform)
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
            var results = app.WaitForElement(c => c.Marked("Welcome to Xamarin.Forms!"));
            app.Screenshot("Welcome screen.");

            Assert.IsTrue(results.Any());
        }
    }
}