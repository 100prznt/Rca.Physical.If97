using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Physical.If97.SeuIf97
{
    /// <summary>
    /// SEUIF97 library wrapper
    /// </summary>
    internal class SeuIf97Wrapper
    {
        /// <summary>
        /// Inputfunction (p,t)
        /// </summary>
        /// <param name="pressure">Value fpr pressure in [MPa]</param>
        /// <param name="temperature">Value for temperature in [°C]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupt(double pressure, double temperature, Properties pid);

        /// <summary>
        /// Inputfunction (p,h)
        /// </summary>
        /// <param name="pressure">Value fpr pressure in [MPa]</param>
        /// <param name="specEnthalpy">Value for specific enthalpy in [kJ/kg]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuph(double pressure, double specEnthalpy, Properties pid);

        /// <summary>
        /// Inputfunction (p,s)
        /// </summary>
        /// <param name="pressure">Value fpr pressure in [MPa]</param>
        /// <param name="specEntropy">Value for specific entropy in [kJ/(kg·K)]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seups(double pressure, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (p,v)
        /// </summary>
        /// <param name="pressure">Value fpr pressure in [MPa]</param>
        /// <param name="specVolume">Value for specific volume in [m^3/kg]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupv(double pressure, double specVolume, Properties pid);

        /// <summary>
        /// Inputfunction (t,h)
        /// </summary>
        /// <param name="temperature">Value for temperature in [°C]</param>
        /// <param name="specEnthalpy">Value for specific enthalpy in [kJ/kg]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuth(double temperature, double specEnthalpy, Properties pid);

        /// <summary>
        /// Inputfunction (t,s)
        /// </summary>
        /// <param name="temperature">Value for temperature in [°C]</param>
        /// <param name="specEntropy">Value for specific entropy in [kJ/(kg·K)]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuts(double temperature, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (t,v)
        /// </summary>
        /// <param name="temperature">Value for temperature in [°C]</param>
        /// <param name="specVolume">Value for specific volume in [m^3/kg]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seutv(double temperature, double specVolume, Properties pid);

        /// <summary>
        /// Inputfunction (h,s)
        /// </summary>
        /// <param name="specEnthalpy">Value for specific enthalpy in [kJ/kg]</param>
        /// <param name="specEntropy">Value for specific entropy in [kJ/(kg·K)]</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuhs(double specEnthalpy, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (p,x)
        /// </summary>
        /// <param name="pressure">Value for pressure in [MPa]</param>
        /// <param name="steamQuality">Steam quality value</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupx(double pressure, double steamQuality, Properties pid);

        /// <summary>
        /// Inputfunction (t,x)
        /// </summary>
        /// <param name="temperature">Value for temperature in [°C]</param>
        /// <param name="steamQuality">Steam quality value</param>
        /// <param name="pid">ID of the property to be calculated</param>
        /// <returns>Value of the property, specified with <paramref name="pid"/></returns>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seutx(double temperature, double steamQuality, Properties pid);

        /// <summary>
        /// Isentropic Enthalpy Drop
        /// </summary>
        /// <param name="pressureInlet">Inlet pressure in [MPa]</param>
        /// <param name="temperatureInlet">Inlet temperature in [°C]</param>
        /// <param name="pressureOutlet">Outlet pressure in [MPa]</param>
        /// <returns>Isentropic Enthalpy Drop in [kg]</returns> //TODO: check unit
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuishd(double pressureInlet, double temperatureInlet, double pressureOutlet);

        /// <summary>
        /// Isentropic Efficiency (0..100)
        /// </summary>
        /// <param name="pressureInlet">Inlet pressure in [MPa]</param>
        /// <param name="temperatureInlet">Inlet temperature in [°C]</param>
        /// <param name="pressureOutlet">Outlet pressure in [MPa]</param>
        /// <param name="temperatureOutlet">Outlet temperature in [°C]</param>
        /// <returns>Isentropic Efficiency in [%]</returns> //TODO: check unit
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuief(double pressureInlet, double temperatureInlet, double pressureOutlet, double temperatureOutlet);

    }
}