using Rca.Physical.Helpers;

namespace Rca.Physical.If97.Tests
{
    [TestClass]
    public class Water_Tests
    {
        [TestMethod]
        public void InitWater_Test()
        {
            var water = new Water();
        }

        [TestMethod]
        public void CalcDesity_Test()
        {
            var water = new Water();
            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));
        }


    }
}