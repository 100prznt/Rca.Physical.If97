using Rca.Physical.Helpers;
using Rca.Physical.If97.SeuIf97;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        private delegate double SeuIf97FunctionDelegate(double parameter1, double parameter2, SeuIf97Properties property);

        #region Members

        private static bool m_SeuIf97DllIsInitialized = false;

        private Dictionary<string, CalculationProperty> m_PropertyInfos;

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

        #endregion Members

        #region Properties

        /// <summary>
        /// <inheritdoc cref="Properties.Pressure"/>
        /// </summary>
        public PhysicalValue Pressure
        {
            get
            {
                var temperature = GetPropertyValue();
                CheckPressure(temperature);
                return temperature;
            }
        }

        /// <summary>
        /// <inheritdoc cref="Properties.Temperature"/>
        /// </summary>
        public PhysicalValue Temperature
        {
            get
            {
                var temperature = GetPropertyValue();
                CheckTemperature(temperature);
                return temperature;
            }
        }

        /// <summary>
        /// Saturation pressure in [MPa]
        /// </summary>
        public PhysicalValue SaturationPressure => CalculateSaturationPressure();

        /// <summary>
        /// Saturation temperature in [°C]
        /// </summary>
        public PhysicalValue SaturationTemperature => CalculateSaturationTemperature();

        /// <summary>
        /// <inheritdoc cref="Properties.Density"/>
        /// </summary>
        public PhysicalValue Density => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.SurfaceTension"/>
        /// </summary>
        public PhysicalValue SurfaceTension => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.Volume"/>
        /// </summary>
        public PhysicalValue SpecificVolume => GetPropertyValue();


        /// <summary>
        /// <inheritdoc cref="Properties.DynamicViscosity"/>
        /// </summary>
        public PhysicalValue DynamicViscosity => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.KinematicViscosity"/>
        /// </summary>
        public PhysicalValue KineticViscosity => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.Region"/>
        /// </summary>
        public int Region => GetPropertyValue<int>();

        /// <summary>
        /// <inheritdoc cref="Properties.SteamQuality"/>
        /// </summary>
        public double SteamQuality => GetPropertyValue<double>();

        /// <summary>
        /// <inheritdoc cref="Properties.PrandtlNumber"/>
        /// </summary>
        public double PrandtlNumber => GetPropertyValue<double>();

        /// <summary>
        /// <inheritdoc cref="Properties.CompressibilityFactor"/>
        /// </summary>
        public double CompressibilityFactor => GetPropertyValue<double>();

        /// <summary>
        /// <inheritdoc cref="Properties.IsentropicExponent"/>
        /// </summary>
        public double IsentropicExponent => GetPropertyValue<double>();

        /// <summary>
        /// <inheritdoc cref="Properties.SpeedOfSound"/>
        /// </summary>
        public PhysicalValue SpeedOfSound => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.Exergy"/>
        /// </summary>
        public PhysicalValue SpecificExergy => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.Enthalpy"/>
        /// </summary>
        public PhysicalValue SpecificEnthalpy => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.InternalEnergy"/>
        /// </summary>
        public PhysicalValue SpecificInternalEnergy => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.HelmholtzFreeEnergy"/>
        /// </summary>
        public PhysicalValue SpecificHelmholtzFreeEnergy => GetPropertyValue();

        /// <summary>
        /// <inheritdoc cref="Properties.GibbsFreeEnergy"/>
        /// </summary>
        public PhysicalValue SpecificGibbsFreeEnergy => GetPropertyValue();

        #endregion Properties

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <exception cref="System.ComponentModel.Win32Exception">Can not set DLL directory</exception>
        /// <exception cref="AggregateException">Can not get assembly location</exception>
        public Water()
        {
            if (!m_SeuIf97DllIsInitialized)
            {
                DllHelper.ExtractEmbeddedDlls("libseuif97.dll", Properties.Resources.libseuif97);

                //var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //if (path is not null)
                //{
                //    path = Path.Combine(path, "Dependencies", Environment.Is64BitProcess ? "x64" : "x86");
                //    if (!SetDllDirectory(path))
                //        throw new System.ComponentModel.Win32Exception("Can not set DLL directory");
                //}
                //else
                //    throw new AggregateException("Can not get assembly location");



                ////O N L Y   F O R   D E B U G I N G
                //var logpath = @"D:/Temp/debuginfo_dllpath.txt";
                //if (Directory.Exists(Path.GetDirectoryName(logpath)) & !File.Exists(logpath))
                //{
                //    using var sw = new StreamWriter(logpath);
                //    sw.WriteLine($"SetDllDirectory({path})");
                //}

            }

            m_PropertyInfos = new Dictionary<string, CalculationProperty>()
            {
                { nameof(Pressure),                    new(PhysicalUnits.Megapascal,             SeuIf97Properties.Pressure              )},
                { nameof(Temperature),                 new(PhysicalUnits.Celsius,                SeuIf97Properties.Temperature           )},
                { nameof(SaturationPressure),          new(PhysicalUnits.Megapascal,             SeuIf97Properties.NotDefined            )},
                { nameof(SaturationTemperature),       new(PhysicalUnits.Celsius,                SeuIf97Properties.NotDefined            )},
                { nameof(Density),                     new(PhysicalUnits.KilogramPerCubicMetre,  SeuIf97Properties.Density               )},
                { nameof(SurfaceTension),              new(PhysicalUnits.MillinewtonPerMetre,    SeuIf97Properties.SurfaceTension        )},
                { nameof(SpecificVolume),              new(PhysicalUnits.CubicMetrePerKilogram,  SeuIf97Properties.Volume                )},
                { nameof(DynamicViscosity),            new(PhysicalUnits.KilogramPerMetreSecond, SeuIf97Properties.DynamicViscosity      )},
                { nameof(KineticViscosity),            new(PhysicalUnits.SquareMetrePerSecond,   SeuIf97Properties.KinematicViscosity    )},
                { nameof(Region),                      new(PhysicalUnits.None,                   SeuIf97Properties.Region                )},
                { nameof(SteamQuality),                new(PhysicalUnits.None,                   SeuIf97Properties.SteamQuality          )},
                { nameof(PrandtlNumber),               new(PhysicalUnits.None,                   SeuIf97Properties.PrandtlNumber         )},
                { nameof(CompressibilityFactor),       new(PhysicalUnits.None,                   SeuIf97Properties.CompressibilityFactor )},
                { nameof(IsentropicExponent),          new(PhysicalUnits.None,                   SeuIf97Properties.IsentropicExponent    )},
                { nameof(SpeedOfSound),                new(PhysicalUnits.MetrePerSecond,         SeuIf97Properties.SpeedOfSound          )},
                { nameof(SpecificExergy),              new(PhysicalUnits.KilojoulePerKilogram,   SeuIf97Properties.Exergy                )},
                { nameof(SpecificEnthalpy),            new(PhysicalUnits.KilojoulePerKilogram,   SeuIf97Properties.Enthalpy              )},
                { nameof(SpecificInternalEnergy),      new(PhysicalUnits.KilojoulePerKilogram,   SeuIf97Properties.InternalEnergy        )},
                { nameof(SpecificHelmholtzFreeEnergy), new(PhysicalUnits.KilojoulePerKilogram,   SeuIf97Properties.HelmholtzFreeEnergy   )},
                { nameof(SpecificGibbsFreeEnergy),     new(PhysicalUnits.KilojoulePerKilogram,   SeuIf97Properties.GibbsFreeEnergy       )}
            };
        }

        #endregion Constructor

        #region Public services

        /// <summary>
        /// Update the water condition, with new value for temperature.
        /// </summary>
        /// <param name="temperature">New temperature value</param>
        public void UpdateT(PhysicalValue temperature)
        {
            CheckTemperature(temperature);
            ResetCalculationStates();

            var parameterNumber = m_PropertyInfos[nameof(Temperature)].ParameterNumber;

            if (parameterNumber != 0)
                UpdateParameter(parameterNumber, temperature.ValueAs(PhysicalUnits.Celsius), nameof(Temperature));
            else
            {
                ResetParameterAssignment();
                m_Parameter1 = double.NaN;
                m_Parameter2 = double.NaN;
                m_PropertyInfos[nameof(Temperature)].Value = temperature.ValueAs(PhysicalUnits.Celsius);
                m_PropertyInfos[nameof(Temperature)].IsCalculated = true;
            }
        }

        /// <summary>
        /// Update the water condition, with new value for pressure.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        public void UpdateP(PhysicalValue pressure)
        {
            CheckPressure(pressure);
            ResetCalculationStates();

            var parameterNumber = m_PropertyInfos[nameof(Pressure)].ParameterNumber;

            if (parameterNumber != 0)
                UpdateParameter(parameterNumber, pressure.ValueAs(PhysicalUnits.Megapascal), nameof(Pressure));
            else
            {
                ResetParameterAssignment();
                m_Parameter1 = double.NaN;
                m_Parameter2 = double.NaN;
                m_PropertyInfos[nameof(Pressure)].Value = pressure.ValueAs(PhysicalUnits.Megapascal);
                m_PropertyInfos[nameof(Pressure)].IsCalculated = true;
            }
        }

        /// <summary>
        /// Update the water condition, with new values for pressure and temperature.
        /// </summary>
        /// <param name="pressure">New pressure value</param>
        /// <param name="temperature">New temperature value</param>
        public void UpdatePT(PhysicalValue pressure, PhysicalValue temperature)
        {
            CheckPressure(pressure);
            CheckTemperature(temperature);
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), nameof(Pressure));
            UpdateParameter(2, temperature.ValueAs(PhysicalUnits.Celsius), nameof(Temperature));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), nameof(Pressure));
            UpdateParameter(2, specificVolume.ValueAs(PhysicalUnits.CubicMetrePerKilogram), nameof(SpecificVolume));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), nameof(Temperature));
            UpdateParameter(2, specificVolume.ValueAs(PhysicalUnits.CubicMetrePerKilogram), nameof(SpecificVolume));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), nameof(Temperature));
            UpdateParameter(2, steamQuality, nameof(SteamQuality));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), nameof(Pressure));
            UpdateParameter(2, steamQuality, nameof(SteamQuality));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, pressure.ValueAs(PhysicalUnits.Megapascal), nameof(Pressure));
            UpdateParameter(2, enthalpy.ValueAs(PhysicalUnits.KilojoulePerKilogram), nameof(SpecificEnthalpy));

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
            ResetParameterAssignment();
            ResetCalculationStates();

            UpdateParameter(1, temperature.ValueAs(PhysicalUnits.Celsius), nameof(Temperature));
            UpdateParameter(2, enthalpy.ValueAs(PhysicalUnits.KilojoulePerKilogram), nameof(SpecificEnthalpy));

            m_SeuIf97Function_Handler = SeuIf97Wrapper.seuth;
        }

        #endregion Public services

        #region Private services

        private void UpdateParameter(int parameterNumber, double value, string propertyName)
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

            m_PropertyInfos[propertyName].Value = value;
            m_PropertyInfos[propertyName].ParameterNumber = parameterNumber;
            m_PropertyInfos[propertyName].IsCalculated = true;
        }

        private void ResetCalculationStates()
        {
            Parallel.ForEach(m_PropertyInfos, p => p.Value.IsCalculated = false);
        }

        private void ResetParameterAssignment()
        {
            Parallel.ForEach(m_PropertyInfos, p => p.Value.ParameterNumber = 0);
            m_SeuIf97Function_Handler = null;
        }

        private CalculationProperty GetPropertyInfos([CallerMemberName]string propertyName = "")
        {
            return m_PropertyInfos[propertyName];
        }

        private T GetPropertyValue<T>([CallerMemberName] string propertyName = "") where T : IConvertible
        {
            var value = GetPropertyValue(propertyName);

            if (value.Unit == PhysicalUnits.None)
                return (T)Convert.ChangeType(value.Value, typeof(T));
            else
                throw new ArgumentException($"Calculated value have a specific unit ({value.Unit}) and can not pass without unit.");
        }

        private  PhysicalValue GetPropertyValue([CallerMemberName] string propertyName = "")
        {
            var propertyInfo = GetPropertyInfos(propertyName);

            if (!propertyInfo.IsCalculated)
                CalculateProperty(propertyName);

            return new(GetPropertyInfos(propertyName).Value, propertyInfo.SeuIf97Unit);
        }

        private void CalculateProperty([CallerMemberName]string propertyName = "")
        {
            //O N L Y   F O R   D E B U G I N G
            var logpath = @"D:/Temp/debuginfo.txt";
            if (Directory.Exists(Path.GetDirectoryName(logpath)) &! File.Exists(logpath))
            {
                using var sw = new StreamWriter(logpath);
                sw.WriteLine(nameof(CalculateProperty));
                sw.WriteLine("System.AppContext.BaseDirectory = " + System.AppContext.BaseDirectory);
                sw.WriteLine("AppDomain.CurrentDomain.BaseDirectory = " + AppDomain.CurrentDomain.BaseDirectory);
                sw.WriteLine("Directory.GetCurrentDirectory() = " + Directory.GetCurrentDirectory());
                sw.WriteLine("Environment.CurrentDirectory = " + Environment.CurrentDirectory);
                sw.WriteLine("Assembly.GetExecutingAssembly().Location = " + Assembly.GetExecutingAssembly().Location);
                sw.WriteLine("Assembly.GetAssembly(typeof(Water)).Location = " + Assembly.GetAssembly(typeof(Water)).Location);

            }



            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName), "Property name must be set.");

            if (m_SeuIf97Function_Handler is not null)
            {
                var result = m_SeuIf97Function_Handler(m_Parameter1, m_Parameter2, m_PropertyInfos[propertyName].SeuIf97PropertyId);

                if (double.IsNormal(result))
                {
                    m_PropertyInfos[propertyName].IsCalculated = true;
                    m_PropertyInfos[propertyName].Value = result;
                }
                else
                    throw new ArgumentException("Calculation returns with error, result value is: " + result);
            }
            else
                throw new ArgumentNullException(nameof(m_SeuIf97Function_Handler), "Before the calculation, a parameter update is required.");
        }

        private PhysicalValue CalculateSaturationPressure([CallerMemberName] string propertyName = "")
        {
            var temperature = Temperature.ValueAs(PhysicalUnits.Celsius);
            var pressure = SeuIf97Wrapper.seutx(temperature, 1, SeuIf97Properties.Pressure);

            m_PropertyInfos[propertyName].Value = pressure;
            m_PropertyInfos[propertyName].IsCalculated = true;

            return new(pressure, PhysicalUnits.Megapascal);
        }

        private PhysicalValue CalculateSaturationTemperature([CallerMemberName] string propertyName = "")
        {
            var pressure = Pressure.ValueAs(PhysicalUnits.Megapascal);
            var temperature = SeuIf97Wrapper.seupx(pressure, 1, SeuIf97Properties.Temperature);

            m_PropertyInfos[propertyName].Value = temperature;
            m_PropertyInfos[propertyName].IsCalculated = true;

            return new(temperature, PhysicalUnits.Celsius);
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
