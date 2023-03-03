using Rca.Physical.If97.SeuIf97;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rca.Physical.If97
{
    internal class CalculationProperty
    {
        public double Value { get; set; }

        public bool IsCalculated { get; set; }

        public PhysicalUnits SeuIf97Unit { get; init; }

        public SeuIf97Properties SeuIf97PropertyId { get; init; }

        public int ParameterNumber { get; set; }

        public CalculationProperty(PhysicalUnits unit, SeuIf97Properties id)
        {
            SeuIf97Unit = unit;
            SeuIf97PropertyId = id;
            Value = double.NaN;
            IsCalculated = false;
            ParameterNumber = 0;
        }
    }
}
