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
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupt(double pressure, double temperature, Properties pid);

        /// <summary>
        /// Inputfunction (p,h)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuph(double pressure, double specEnthalpy, Properties pid);

        /// <summary>
        /// Inputfunction (p,s)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seups(double pressure, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (p,v)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupv(double pressure, double specVolume, Properties pid);

        /// <summary>
        /// Inputfunction (t,h)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuth(double temperature, double specEnthalpy, Properties pid);

        /// <summary>
        /// Inputfunction (t,s)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuts(double temperature, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (t,v)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seutv(double temperature, double specVolume, Properties pid);

        /// <summary>
        /// Inputfunction (h,s)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuhs(double enthalpy, double specEntropy, Properties pid);

        /// <summary>
        /// Inputfunction (p,x)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seupx(double pressure, double steamQuality, Properties pid);

        /// <summary>
        /// Inputfunction (t,x)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seutx(double temperature, double steamQuality, Properties pid);

        /// <summary>
        /// Isentropic Enthalpy Drop
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuishd(double pressureInlet, double temperatureInlet, double pressureOutlet);

        /// <summary>
        /// Isentropic Efficiency (0..100)
        /// </summary>
        [DllImport("libseuif97.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern double seuief(double pressureInlet, double temperatureInlet, double pressureOutlet, double temperatureOutlet);

    }
}