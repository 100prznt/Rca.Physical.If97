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

            Assert.IsNotNull(water);
        }

        [TestMethod]
        public void CalcDesityFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            var density = water.Density.ValueAs(PhysicalUnits.KilogramPerCubicMetre);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(997.047, density, 1E-3);
        }

        [TestMethod]
        public void CalcPrandtlNumberFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(6.12663, water.PrandtlNumber, 1E-2);
        }

        [TestMethod]
        public void CalcRegionFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(1, water.Region);
        }

        [TestMethod]
        public void CalcVolumeFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            var volume = water.SpecificVolume.ValueAs(PhysicalUnits.CubicMetrePerKilogram);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(0.00100296, volume, 1E-8);
        }

        [TestMethod]
        public void CalcDynamicViscosityFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            var viscosity = water.DynamicViscosity.ValueAs(PhysicalUnits.PascalSecond);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(0.000890023, viscosity, 1E-7);
        }

        [TestMethod]
        public void CalcKineticViscosityFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            var viscosity = water.KineticViscosity.ValueAs(PhysicalUnits.SquareMetrePerSecond);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(8.927E-7, viscosity, 1E-10);
        }

        [TestMethod]
        public void CalcEnthalpyFromPT_Tests()
        {
            var water = new Water();

            water.UpdatePT(Pressure.FromBar(1), ThermodynamicTemperature.FromCelsius(25));

            var enthalpy = water.SpecificEnthalpy.ValueAs(PhysicalUnits.KilojoulePerKilogram);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(104.928, enthalpy, 1E-4);
        }

        [TestMethod]
        public void CalcSaturationPressureFromTX_Tests()
        {
            var water = new Water();

            water.UpdateTX(ThermodynamicTemperature.FromCelsius(25), 1);

            var saturationPressure = water.Pressure.ValueAs(PhysicalUnits.Millibar);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(31.6975, saturationPressure, 1E-4);
        }

        [TestMethod]
        public void CalcSaturationTemperatureFromPX_Tests()
        {
            var water = new Water();

            water.UpdatePX(Pressure.FromStandardAtmosphere(1), 1);

            var saturationTemperature = water.Temperature.ValueAs(PhysicalUnits.Celsius);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(99.9743, saturationTemperature, 1E-4);
        }



        [TestMethod]
        public void CalcSaturationPressureFromT_Tests()
        {
            var water = new Water();

            water.UpdateT(ThermodynamicTemperature.FromCelsius(25));

            var saturationPressure = water.SaturationPressure.ValueAs(PhysicalUnits.Millibar);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(31.6975, saturationPressure, 1E-4);
        }


        [TestMethod]
        public void CalcSaturationTemperatureFromP_Tests()
        {
            var water = new Water();

            water.UpdateP(Pressure.FromStandardAtmosphere(1));

            var saturationTemperature = water.SaturationTemperature.ValueAs(PhysicalUnits.Celsius);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(99.9743, saturationTemperature, 1E-4);
        }

        [TestMethod]
        public void CalcSaturationPressureFromTUpdate_Tests()
        {
            var water = new Water();

            //initial conditions
            water.UpdateTH(ThermodynamicTemperature.FromCelsius(100), SpecificEnergy.FromJoulePerKilogram(1));

            //updated conditions
            water.UpdateT(ThermodynamicTemperature.FromCelsius(25));

            var saturationPressure = water.SaturationPressure.ValueAs(PhysicalUnits.Millibar);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(31.6975, saturationPressure, 1E-4);
        }


        [TestMethod]
        public void CalcSaturationTemperatureFromPUpdate_Tests()
        {
            var water = new Water();

            //initial conditions
            water.UpdateTH(ThermodynamicTemperature.FromCelsius(100), SpecificEnergy.FromJoulePerKilogram(1));

            //updated conditions
            water.UpdateP(Pressure.FromStandardAtmosphere(1));

            var saturationTemperature = water.SaturationTemperature.ValueAs(PhysicalUnits.Celsius);

            //Referenc value from: https://thermofluidprop.com/stoffwerte-online/fluid-property-calculator
            Assert.AreEqual(99.9743, saturationTemperature, 1E-4);
        }
    }
}