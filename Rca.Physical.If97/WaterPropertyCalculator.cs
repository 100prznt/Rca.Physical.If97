/*using Rca.Physical.Helpers;
using Rca.Physical.If97.SeuIf97;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Rca.Physical.Calculators
{
    public class WaterPropertyCalculator
    {
        public WaterPropertyCalculator() {
            
        }

        /// <summary>
        /// Range of validity of IAPWS-IF97 - Temperature
        /// see also: https://web1.hszg.de/thermo_fpc/range_of_validity/range_of_validity_water.htm?choose_fluid=1&
        /// </summary>
        private static PhysicalRange validTemperatureRange = new(0, 800, PhysicalUnits.Celsius);

        /// <summary>
        /// Range of validity of IAPWS-IF97 - Pressure
        /// see also: https://web1.hszg.de/thermo_fpc/range_of_validity/range_of_validity_water.htm?choose_fluid=1&
        /// </summary>
        private static PhysicalRange validPressureRange = new(0.00611, 1000, PhysicalUnits.Bar);

        /// <summary>
        /// Caluculate the destiny of water using the IAPWS-IF97 model
        /// </summary>
        /// <param name="pressure">Pressure</param>
        /// <param name="temperature">Temperature</param>
        /// <returns>Destiny</returns>
        /// <exception cref="ArgumentException">Invalid dimesion of passed values.</exception>
        public static PhysicalValue GetDestinyByPressureAndTemperature(PhysicalValue pressure, PhysicalValue temperature)
        {
            CheckPressureAndTemperature(pressure, temperature);

            var p = pressure.ValueAs(PhysicalUnits.Megapascal);
            var t = temperature.ValueAs(PhysicalUnits.Celsius);

            //TODO: Prüfen ob Werte im gültigen Bereich liegen
            var d = SeuIf97Wrapper.seupt(p, t, PropertyIds.Density);

            return new(d, PhysicalUnits.KilogramPerCubicMetre);
        }

        /// <summary>
        /// Caluculate the dynamic viscosity of water using the IAPWS-IF97 model
        /// </summary>
        /// <param name="pressure">Pressure</param>
        /// <param name="temperature">Temperature</param>
        /// <returns>Dynamic viscosity</returns>
        /// <exception cref="ArgumentException">Invalid dimesion of passed values.</exception>
        public static PhysicalValue GetDynamicViscosityByPressureAndTemperature(PhysicalValue pressure, PhysicalValue temperature)
        {
            CheckPressureAndTemperature(pressure, temperature);

            var p = pressure.ValueAs(PhysicalUnits.Megapascal);
            var t = temperature.ValueAs(PhysicalUnits.Celsius);

            //TODO: Prüfen ob Werte im gültigen Bereich liegen
            var dv = SeuIf97Wrapper.seupt(p, t, PropertyIds.DynamicViscosity);

            return new(dv, PhysicalUnits.KilogramPerMetreSecond);
        }

        /// <summary>
        /// Caluculate the kinetic viscosity of water using the IAPWS-IF97 model
        /// </summary>
        /// <param name="pressure">Pressure</param>
        /// <param name="temperature">Temperature</param>
        /// <returns>Kinetic viscosity</returns>
        /// <exception cref="ArgumentException">Invalid dimesion of passed values.</exception>
        public static PhysicalValue GetKineticViscosityByPressureAndTemperature(PhysicalValue pressure, PhysicalValue temperature)
        {
            CheckPressureAndTemperature(pressure, temperature);

            var p = pressure.ValueAs(PhysicalUnits.Megapascal);
            var t = temperature.ValueAs(PhysicalUnits.Celsius);

            //TODO: Prüfen ob Werte im gültigen Bereich liegen
            var kv = SeuIf97Wrapper.seupt(p, t, PropertyIds.KinematicViscosity);

            return new(kv, PhysicalUnits.SquareMetrePerSecond);
        }

    }
}
*/
