using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solax.InverterHttpApi.SolaxDataList;

namespace Solax.InverterHttpApi
{
    public class SolaxValue
    {
        public static SolaxValue GetValue(SolaxDataRaw data, int index, string name, UnitsEnum? unit, OperationsEnum operation)
        {
            // Input validation
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Data == null)
                throw new ArgumentException("Data property cannot be null.", nameof(data));
            if (index < 0 || index >= data.Data.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));

            decimal value = data.Data[index];
            return operation switch
            {
                OperationsEnum.div10 => new SolaxValue(index, name, unit, value / 10m),
                OperationsEnum.twoway_div10 => new SolaxValue(index, name, unit, Math.Round(value / 10m, 2, MidpointRounding.ToEven) / 10m),
                OperationsEnum.twoway_div100 => new SolaxValue(index, name, unit, Math.Round(value / 100m, 2, MidpointRounding.ToEven) / 100m),
                OperationsEnum.div100 => new SolaxValue(index, name, unit, value / 100m),
                OperationsEnum.to_signed => new SolaxValue(index, name, unit, value),
                _ => new SolaxValue(index, name, unit, value)
            };
        }

        public SolaxValue(int index, string name, UnitsEnum? unit, decimal value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));
            this.Index = index;
            this.Name = name;
            this.Unit = unit;
            this.Value = value;
        }

        public int Index { get; }
        public string Name { get; }
        public UnitsEnum? Unit { get; }
        public decimal Value { get; }
        public Parameter Parameter => (Parameter)Index;

        public override string ToString()
        {
            return Unit switch
            {
                UnitsEnum.INVERTER_OPERATION_MODE => $"{Name} = {InverterModesDict.GetModeName(Convert.ToInt32(Value))}",
                UnitsEnum.BATTERY_OPERATION_MODE => $"{Name} = {BatteryModesDict.GetModeName(Convert.ToInt32(Value))}",
                UnitsEnum.PERCENT => $"{Name} = {Value} %",
                UnitsEnum.C => $"{Name} = {Value} °C",
                _ => $"{Name} = {Value} {Unit}"
            };
        }
    }
}
