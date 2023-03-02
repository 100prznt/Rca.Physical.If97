using Rca.Physical.Helpers;
using Rca.Physical.If97.SeuIf97;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Physical.If97
{
    public class Water
    {
        /// <summary>
        /// Range of validity of IAPWS-IF97 - Temperature
        /// see also: https://web1.hszg.de/thermo_fpc/range_of_validity/range_of_validity_water.htm?choose_fluid=1&
        /// </summary>
        private static readonly PhysicalRange validTemperatureRange = new(0, 800, PhysicalUnits.Celsius);

        /// <summary>
        /// Range of validity of IAPWS-IF97 - Pressure
        /// see also: https://web1.hszg.de/thermo_fpc/range_of_validity/range_of_validity_water.htm?choose_fluid=1&
        /// </summary>
        private static readonly PhysicalRange validPressureRange = new(0.00611, 1000, PhysicalUnits.Bar);


        private delegate double SeuIf97FunctionDelegate(double parameter1, double parameter2, Properties property);

        #region Members
        /// <summary>
        /// Input variables have changed, recalculation required.
        /// </summary>
        private bool m_CalculationRequired;

        /// <summary>
        /// Handler to hold the current SeuIf97 calculation function
        /// </summary>
        private SeuIf97FunctionDelegate? m_SeuIf97Function_Handler;

        /// <summary>
        /// First parameter for current SeuIf97 calculation function 
        /// </summary>
        private double m_Parameter1;

        /// <summary>
        /// Second parameter for current SeuIf97 calculation function 
        /// </summary>
        private double m_Parameter2;

        /// <summary>
        /// Pressure p in [MPa]
        /// </summary>
        private double m_Pressure;

        /// <summary>
        /// Temperature t in [°C]
        /// </summary>
        private double m_Temperature;

        /// <summary>
        /// Destiny d in [kg/m^3]
        /// </summary>
        private double m_Density;

        /// <summary>
        /// Specific volume v in [m^3/kg]
        /// </summary>
        private double m_Volume;

        /// <summary>
        /// Specific enthalpy h in [kJ/kg]
        /// </summary>
        private double m_Enthalpy;

        /// <summary>
        /// Specific entropy s in [kJ/(kg·K)]
        /// </summary>
        private double m_Entropy;

        /// <summary>
        /// Specific exergy e in [kJ/kg]	
        /// </summary>
        private double m_Exergy;

        /// <summary>
        /// Specific internal energy u in [kJ/kg] 
        /// </summary>
        private double m_InternalEnergy;

        /// <summary>
        /// Specific isobaric heat capacity cp in [kJ/(kg·K)]
        /// </summary>
        private double m_IsobaricHeatCapacity;

        /// <summary>
        /// Specific isochoric heat capacity cv in [kJ/(kg·K)]
        /// </summary>
        private double m_IsochoricHeatCapacity;

        /// <summary>
        /// Speed of sound w in [m/s] 
        /// </summary>
        private double m_SpeedOfSound;

        /// <summary>
        /// Isentropic exponent ks
        /// </summary>
        private double m_IsentropicExponent;

        /// <summary>
        /// Specific Helmholtz free energy f in [kJ/kg] 
        /// </summary>
        private double m_HelmholtzFreeEnergy;

        /// <summary>
        /// Specific Gibbs free energy g in [kJ/kg]  
        /// </summary>
        private double m_GibbsFreeEnergy;

        /// <summary>
        /// CompressibilityFactor z
        /// </summary>
        private double m_CompressibilityFactor;

        /// <summary>
        /// SteamQuality x
        /// </summary>
        private double m_SteamQuality;

        /// <summary>
        /// IF97 Model-Region r 
        /// </summary>
        private double m_Region;

        /// <summary>
        /// Isobaric volume expansion coefficient ec in [1/K] 
        /// </summary>
        private double m_IsobaricVolumeExpansionCoefficient;

        /// <summary>
        /// Isothermal compressibility kt in [1/MPa] 
        /// </summary>
        private double m_IsothermalCompressibility;

        /// <summary>
        /// Partial derivative(dV/dT)p dvdt in [m3/(kg·K)]	
        /// </summary>
        private double m_PartialDerivative_dV_dT_p;

        /// <summary>
        /// Partial derivative(dV/dP)T dvdp in [m3/(kg·MPa)]
        /// </summary>
        private double m_PartialDerivative_dV_dP_T;

        /// <summary>
        /// Partial derivative(dP/dT)v dpdt in [MPa/K]  
        /// </summary>
        private double m_PartialDerivative_dP_dT_v;

        /// <summary>
        /// Isothermal Joule-Thomson coefficient  iJTC in [kJ/(kg·MPa)]
        /// </summary>
        private double m_IsothermalJouleThomsonCoefficient;

        /// <summary>
        /// Joule-Thomson coefficient JTC in [K/MPa]
        /// </summary>
        private double m_JouleThomsonCoefficient;

        /// <summary>
        /// Dynamic viscosity dv in [kg/(m·s)]
        /// </summary>
        private double m_DynamicViscosity;

        /// <summary>
        /// Kinematic viscosity kv in [m^2/s]
        /// </summary>
        private double m_KinematicViscosity;

        /// <summary>
        /// Thermal conductivity tc in [W/(m·K)]
        /// </summary>
        private double m_ThermalConductivity;

        /// <summary>
        /// Thermal diffusivity td in [um^2/s] 
        /// </summary>
        private double m_ThermalDiffusivity;

        /// <summary>
        /// Prandtl number pr
        /// </summary>
        private double m_PrandtlNumber;

        /// <summary>
        /// Surface tension st in [mN/m]
        /// </summary>
        private double m_SurfaceTension;

        #endregion Members

        #region Properties

        public PhysicalValue Pressure
        {
            get
            {
                if (m_CalculationRequired)
                {
                    m_Pressure = Calculate(Properties.Pressure);
                    CheckPressure(new(m_Pressure, PhysicalUnits.Megapascal));
                }
                return new(m_Pressure, PhysicalUnits.Megapascal);
            }
        }

        public PhysicalValue Temperature
        {
            get
            {
                if (m_CalculationRequired)
                {
                    m_Temperature = Calculate(Properties.Temperature);
                    CheckTemperature(new(m_Temperature, PhysicalUnits.Celsius));
                }
                return new(m_Temperature, PhysicalUnits.Celsius);
            }
        }

        public PhysicalValue Density
        {
            get
            {
                if (m_CalculationRequired)
                    m_Density = Calculate(Properties.Density);
                return new(m_Density, PhysicalUnits.KilogramPerCubicMetre);
            }
        }
        public PhysicalValue SurfaceTension
        {
            get
            {
                if (m_CalculationRequired)
                    m_SurfaceTension = Calculate(Properties.SurfaceTension);
                return new(m_SurfaceTension, PhysicalUnits.MillinewtonPerMetre);
            }
        }

        public PhysicalValue SpecificVolume
        {
            get
            {
                if (m_CalculationRequired)
                    m_Volume = Calculate(Properties.Volume);
                return new(m_Volume, PhysicalUnits.CubicMetrePerKilogram);
            }
        }

        public PhysicalValue DynamicViscosity
        {
            get
            {
                if (m_CalculationRequired)
                    m_DynamicViscosity = Calculate(Properties.DynamicViscosity);
                return new(m_DynamicViscosity, PhysicalUnits.KilogramPerMetreSecond);
            }
        }

        public PhysicalValue KineticViscosity
        {
            get
            {
                if (m_CalculationRequired)
                    m_KinematicViscosity = Calculate(Properties.KinematicViscosity);
                return new(m_KinematicViscosity, PhysicalUnits.SquareMetrePerSecond);
            }
        }

        public int Region
        {
            get
            {
                if (m_CalculationRequired)
                    m_Region = Calculate(Properties.Region);
                return (int)m_Region;
            }
        }

        public double SteamQuality
        {
            get
            {
                if (m_CalculationRequired)
                    m_SteamQuality = Calculate(Properties.SteamQuality);
                return m_SteamQuality;
            }
        }

        public double PrandtlNumber
        {
            get
            {
                if (m_CalculationRequired)
                    m_PrandtlNumber = Calculate(Properties.PrandtlNumber);
                return m_PrandtlNumber;
            }
        }

        public double CompressibilityFactor
        {
            get
            {
                if (m_CalculationRequired)
                    m_CompressibilityFactor = Calculate(Properties.CompressibilityFactor);
                return m_CompressibilityFactor;
            }
        }

        public double IsentropicExponent
        {
            get
            {
                if (m_CalculationRequired)
                    m_IsentropicExponent = Calculate(Properties.IsentropicExponent);
                return m_IsentropicExponent;
            }
        }

        public PhysicalValue SpeedOfSound
        {
            get
            {
                if (m_CalculationRequired)
                    m_SpeedOfSound = Calculate(Properties.SpeedOfSound);
                return new(m_SpeedOfSound, PhysicalUnits.MetrePerSecond);
            }
        }

        public PhysicalValue SpecificExergy
        {
            get
            {
                if (m_CalculationRequired)
                    m_Exergy = Calculate(Properties.Exergy);
                return new(m_Exergy, PhysicalUnits.KilojoulePerKilogram);
            }
        }

        public PhysicalValue SpecificEnthalpy
        {
            get
            {
                if (m_CalculationRequired)
                    m_Enthalpy = Calculate(Properties.Enthalpy);
                return new(m_Enthalpy, PhysicalUnits.KilojoulePerKilogram);
            }
        }

        public PhysicalValue SpecificInternalEnergy
        {
            get
            {
                if (m_CalculationRequired)
                    m_InternalEnergy = Calculate(Properties.InternalEnergy);
                return new(m_InternalEnergy, PhysicalUnits.KilojoulePerKilogram);
            }
        }

        public PhysicalValue SpecificHelmholtzFreeEnergy
        {
            get
            {
                if (m_CalculationRequired)
                    m_HelmholtzFreeEnergy = Calculate(Properties.HelmholtzFreeEnergy);
                return new(m_HelmholtzFreeEnergy, PhysicalUnits.KilojoulePerKilogram);
            }
        }

        public PhysicalValue SpecificGibbsFreeEnergy
        {
            get
            {
                if (m_CalculationRequired)
                    m_GibbsFreeEnergy = Calculate(Properties.GibbsFreeEnergy);
                return new(m_GibbsFreeEnergy, PhysicalUnits.KilojoulePerKilogram);
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">Can not set DLL directory</exception>
        /// <exception cref="AggregateException">Can not get assembly location</exception>
        public Water()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (path is not null)
            {
                path = Path.Combine(path, "Dependencies", Environment.Is64BitProcess ? "x64" : "x86");
                if (!SetDllDirectory(path))
                    throw new System.ComponentModel.Win32Exception("Can not set DLL directory");
            }
            else
                throw new AggregateException("Can not get assembly location");
        }
        #endregion Constructor

        #region Public services
        /// <summary>
        /// Update the water condition, with new values for pressure and temperature.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        /// <param name="temperature">New temperature value</param>
        public void UpdatePT(PhysicalValue pressure, PhysicalValue temperature)
        {
            CheckPressure(pressure);
            CheckTemperature(temperature);

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), ref m_Pressure);
            UpdateParameter(2, temperature.ValueAs(PhysicalUnits.Celsius), ref m_Temperature);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seupt;
        }

        /// <summary>
        /// Update the water condition, with new values for pressure and specific volume.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        /// <param name="specificVolume">New specific volume</param>
        public void UpdatePV(PhysicalValue pressure, PhysicalValue specificVolume)
        {
            CheckPressure(pressure);

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), ref m_Pressure);
            UpdateParameter(2, specificVolume.ValueAs(PhysicalUnits.CubicMetrePerKilogram), ref m_Volume);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seupv;
        }

        /// <summary>
        /// Update the water condition, with new values for temperature and specific volume.
        /// </summary>
        /// <param name="pressure">New temperature value</param>
        /// <param name="specificVolume">New specific volume</param>
        public void UpdateTV(PhysicalValue temperature, PhysicalValue specificVolume)
        {
            CheckTemperature(temperature);

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), ref m_Temperature);
            UpdateParameter(2, specificVolume.ValueAs(PhysicalUnits.CubicMetrePerKilogram), ref m_Volume);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seutv;
        }

        /// <summary>
        /// Update the water condition, with new values for temperature and steam quality.
        /// </summary>
        /// <param name="temperature">New temperature value</param>
        /// <param name="steamQuality">New steam quality value</param>
        public void UpdateTX(PhysicalValue temperature, double steamQuality)
        {
            CheckTemperature(temperature);

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), ref m_Temperature);
            UpdateParameter(2, steamQuality, ref m_SteamQuality);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seutx;
        }

        /// <summary>
        /// Update the water condition, with new values for pressure and steam quality.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        /// <param name="steamQuality">New steam quality value</param>
        public void UpdatePX(PhysicalValue pressure, double steamQuality)
        {
            CheckPressure(pressure);

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), ref m_Pressure);
            UpdateParameter(2, steamQuality, ref m_SteamQuality);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seupx;
        }

        /// <summary>
        /// Update the water condition, with new values for pressure and specific enthalpy.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        /// <param name="enthalpy">New specific enthalpy value</param>
        public void UpdatePH(PhysicalValue pressure, PhysicalValue enthalpy)
        {
            CheckPressure(pressure);

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), ref m_Pressure);
            UpdateParameter(2, enthalpy.ValueAs(PhysicalUnits.KilojoulePerKilogram), ref m_Enthalpy);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seuph;
        }

        /// <summary>
        /// Update the water condition, with new values for temperature and specific enthalpy.
        /// </summary>
        /// <param name="temperature">New temperature value</param>
        /// <param name="enthalpy">New specific enthalpy value</param>
        public void UpdateTH(PhysicalValue temperature, PhysicalValue enthalpy)
        {
            CheckTemperature(temperature);

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), ref m_Temperature);
            UpdateParameter(2, enthalpy.ValueAs(PhysicalUnits.KilojoulePerKilogram), ref m_Enthalpy);

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seuth;
        }

        #endregion Public services

        #region Private services

        private void UpdateParameter(int parameterNumber, double value, ref double parameterMember)
        {
            switch (parameterNumber)
            {
                case 1:
                    m_Parameter1 = value;
                    break;
                case 2:
                    m_Parameter2 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Only parameter 1 and 2 are available. Parameter {parameterNumber} does not exist.");
            }

            parameterMember = value;
        }

        private double Calculate(Properties property)
        {
            if (m_SeuIf97Function_Handler is not null)
            {
                m_CalculationRequired = true;
                return m_SeuIf97Function_Handler(m_Parameter1, m_Parameter2, property);
            }
            else
                throw new ArgumentNullException(nameof(m_SeuIf97Function_Handler), "Before the calculation, a parameter update is required.");
        }

        private void CheckPressure(PhysicalValue pressure)
        {
            if (!validPressureRange.InRange(pressure))
                throw new ArgumentOutOfRangeException($"Passed pressure value ({pressure}) is out of the valid range for IAPWS-IF97 calculation ({validPressureRange})");
        }

        private void CheckTemperature(PhysicalValue temperature)
        {
            if (!validTemperatureRange.InRange(temperature))
                throw new ArgumentOutOfRangeException($"Passed pressure value ({temperature}) is out of the valid range for IAPWS-IF97 calculation ({validTemperatureRange})");
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        #endregion Private services
    }
}
