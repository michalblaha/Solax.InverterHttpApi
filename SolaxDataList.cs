using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Solax.InverterHttpApi
{
    public class SolaxDataList
    {
        public SolaxDataRaw? RawData { get; }
        public Lang Language { get; }
        Dictionary<int, SolaxValue> decoded = null;

        public SolaxDataList(SolaxDataRaw? rawData, SolaxDataList.Lang language)
        {
            this.RawData = rawData;
            this.Language = language;
            decoded = decode();
        }

        public SolaxValue GetValue(Parameter parameter)
        { 
            if (decoded == null)
                throw new InvalidOperationException("Data has not been decoded yet.");

            if (decoded.TryGetValue((int)parameter, out var value))                
                return value;

            return null;
        }
        public Dictionary<int, SolaxValue> AllValues => decode();

        private Dictionary<int, SolaxValue> decode()
        {
            if (RawData == null)
                throw new InvalidOperationException("Data property cannot be null.");
            var result = new Dictionary<int, SolaxValue>(200);

            Add(result, 0, UnitsEnum.V, OperationsEnum.div10);
            Add(result, 1, UnitsEnum.V, OperationsEnum.div10);
            Add(result, 2, UnitsEnum.V, OperationsEnum.div10);

            Add(result, 3,UnitsEnum.A, OperationsEnum.twoway_div10);
            Add(result, 4,  UnitsEnum.A, OperationsEnum.twoway_div10);
            Add(result, 5,  UnitsEnum.A, OperationsEnum.twoway_div10);

            Add(result, 6, UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 7, UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 8, UnitsEnum.W, OperationsEnum.to_signed);

            Add(result, 9, UnitsEnum.W, OperationsEnum.to_signed);

            Add(result, 10, UnitsEnum.V, OperationsEnum.div10);
            Add(result, 11, UnitsEnum.V, OperationsEnum.div10);

            Add(result, 12, UnitsEnum.A, OperationsEnum.div10);
            Add(result, 13, UnitsEnum.A, OperationsEnum.div10);

            Add(result, 14, UnitsEnum.W, OperationsEnum.NONE);
            Add(result, 15, UnitsEnum.W, OperationsEnum.NONE);

            Add(result, 16,  UnitsEnum.Hz, OperationsEnum.div100);
            Add(result, 17,  UnitsEnum.Hz, OperationsEnum.div100);
            Add(result, 18,  UnitsEnum.Hz, OperationsEnum.div100);

            Add(result, 19,  UnitsEnum.INVERTER_OPERATION_MODE, OperationsEnum.NONE);

            //# 20 - 32: always 0
            //# 33     : always 1

            Add(result, 34,  UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 35,  UnitsEnum.W, OperationsEnum.to_signed);
            //# instead of to_signed this is actually 34 - 35,
            //# because 35 =  if 34>32767: 0 else: 65535
            //# 35: if 34>32767: 0 else: 65535

            //# 36 - 38: always  0
            Add(result, 39,  UnitsEnum.V, OperationsEnum.div100);
            Add(result, 40,  UnitsEnum.A, OperationsEnum.twoway_div100);
            Add(result, 41,  UnitsEnum.W, OperationsEnum.to_signed);
            Add(result, 44,  UnitsEnum.W, OperationsEnum.to_signed);

            //# 42: div10, almost identical to [39]
            //# 43: twoway_div10,  almost the same as "40" (battery current)
            //# 44: twoway_div100, almost the same as "41" (battery power),
            //# 45: always 1
            //# 46: follows PV Output, idles around 44, peaks at 52,
            Add(result, 47,  UnitsEnum.W, OperationsEnum.to_signed);
            //# 48: always 256
            //# 49,50: [49] + [50] * 15160 some increasing counter
            //# 51: always 5634
            //# 52: always 100
            //# 53: always 0
            //# 54: follows PV Output, idles around 35, peaks at 54,
            //# 55-67: always 0
            //"Total Energy": (pack_u16(68, 69), Total(Units.KWH), div10),
            //# 70: div10, today's energy including battery usage
            //# 71-73: 0
            Add(result, 92,  UnitsEnum.KWh, OperationsEnum.div100);
            //# 93-101: always 0
            //# 102: always 1
            Add(result, 103,  UnitsEnum.PERCENT, OperationsEnum.NONE);
            //# 104: always 1
            Add(result, 105,  UnitsEnum.C, OperationsEnum.NONE);
            Add(result, 106,  UnitsEnum.KWh, OperationsEnum.div10);
            //# 107: always 256 or 0
            //# 108: always 3504
            //# 109: always 2400

            Add(result, 168,  UnitsEnum.BATTERY_OPERATION_MODE, OperationsEnum.NONE);


            if (!result.TryGetValue(41, out var v41))
                throw new InvalidOperationException("ValueDict missing key 41");
            var value = v41.Value;
            var sign = (value < 32767) ? 0 : 65535;

            result.Add(-1, new SolaxValue(-1, GetValueName(Parameter.AllPanelPower, this.Language), UnitsEnum.W, result[14].Value + result[15].Value));
            result.Add(-2, new SolaxValue(-2, GetValueName(Parameter.ExportedPower, this.Language), UnitsEnum.W, result[34].Value - result[35].Value));

            result.Add(-3, new SolaxValue(-3, GetValueName( Parameter.BatteryPower, this.Language), UnitsEnum.W, value - sign));

            //public SolaxValue PowerNow => new SolaxValue(-1, "BatteryPower", UnitsEnum.W, -ValueDict[47].Value);
            return result;
        }

        private void Add(Dictionary<int, SolaxValue> dict, int index,  UnitsEnum? unit, OperationsEnum operation)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));
            if (unit == null)
                throw new ArgumentNullException(nameof(unit));

            var value = SolaxValue.GetValue(this.RawData, index, GetValueName((Parameter)index, this.Language), unit, operation);

            dict.Add(index, value);
        }


        public enum Lang
        {
            English,
            German,
            French,
            Spanish,
            Czech,
        }

        public enum Parameter
        {
            BatteryPower = -3,
            ExportedPower = -2,
            AllPanelPower = -1,
            NetworkVoltagePhase1 = 0,
            NetworkVoltagePhase2 = 1,
            NetworkVoltagePhase3 = 2,
            OutputCurrentPhase1 = 3,
            OutputCurrentPhase2 = 4,
            OutputCurrentPhase3 = 5,
            PowerNowPhase1 = 6,
            PowerNowPhase2 = 7,
            PowerNowPhase3 = 8,
            ACPower = 9,
            PV1Voltage = 10,
            PV2Voltage = 11,
            PV1Current = 12,
            PV2Current = 13,
            PV1Power = 14,
            PV2Power = 15,
            GridFrequencyPhase1 = 16,
            GridFrequencyPhase2 = 17,
            GridFrequencyPhase3 = 18,
            InverterOperationMode = 19,
            ExportedPowerValue = 34,
            ExportedPowerSign = 35,
            BatteryVoltage = 39,
            BatteryCurrent = 40,
            BatteryPower41 = 41,
            BatteryPower44 = 44,
            PowerNow = 47,
            TodaysConsumption = 92,
            BatteryRemainingCapacity = 103,
            BatteryTemperature = 105,
            BatteryRemainingEnergy = 106,
            BatteryOperationMode = 168,
        }

        public static Dictionary<int, string> GetValueNames(Lang lang = Lang.English)
        {
            return lang switch
            {
                Lang.English => new Dictionary<int, string>
                {
                    { -3,  "Battery Power"},
                    { -2,  "Exported Power"},
                    { -1,  "All Panel Power"},
                    {  0,  "Network Voltage Phase 1"},
                    {  1,  "Network Voltage Phase 2"},
                    {  2,  "Network Voltage Phase 3"},
                    {  3,  "Output Current Phase 1"},
                    {  4,  "Output Current Phase 2"},
                    {  5,  "Output Current Phase 3"},
                    {  6,  "Power Now Phase 1"},
                    {  7,  "Power Now Phase 2"},
                    {  8,  "Power Now Phase 3"},
                    {  9,  "AC Power"},
                    { 10,  "PV1 Voltage"},
                    { 11,  "PV2 Voltage"},
                    { 12,  "PV1 Current"},
                    { 13,  "PV2 Current"},
                    { 14,  "PV1 Power"},
                    { 15,  "PV2 Power"},
                    { 16,  "Grid Frequency Phase 1"},
                    { 17,  "Grid Frequency Phase 2"},
                    { 18,  "Grid Frequency Phase 3"},
                    { 19,  "Inverter Operation mode"},
                    { 34,  "Exported Power (value)"},
                    { 35,  "Exported Power (sing)"},
                    { 39,  "Battery Voltage"},
                    { 40,  "Battery Current"},
                    { 41,  "Battery Power (41)"},
                    { 44,  "Battery Power (44)"},
                    { 47,  "Power Now"},
                    { 92,  "Today's Consumption"},
                    {103,  "Battery Remaining Capacity"},
                    {105,  "Battery Temperature"},
                    {106,  "Battery Remaining Energy"},
                    {168,  "Battery Operation mode"},
                },
                Lang.German => new Dictionary<int, string>
                {
                    { -3,  "Batterie Leistung"},
                    { -2, "Exportierte Leistung"},
                    { -1, "Gesamte Panel-Leistung"},
                    {  0,  "Netzspannung Phase 1"},
                    {  1,  "Netzspannung Phase 2"},
                    {  2,  "Netzspannung Phase 3"},
                    {  3,  "Ausgangsstrom Phase 1"},
                    {  4,  "Ausgangsstrom Phase 2"},
                    {  5,  "Ausgangsstrom Phase 3"},
                    {  6,  "Aktuelle Leistung Phase 1"},
                    {  7,  "Aktuelle Leistung Phase 2"},
                    {  8,  "Aktuelle Leistung Phase 3"},
                    {  9,  "AC-Leistung"},
                    { 10,  "PV1 Spannung"},
                    { 11,  "PV2 Spannung"},
                    { 12,  "PV1 Strom"},
                    { 13,  "PV2 Strom"},
                    { 14,  "PV1 Leistung"},
                    { 15,  "PV2 Leistung"},
                    { 16,  "Netzfrequenz Phase 1"},
                    { 17,  "Netzfrequenz Phase 2"},
                    { 18,  "Netzfrequenz Phase 3"},
                    { 19,  "Wechselrichter Betriebsmodus"},
                    { 34,  "Exportierte Leistung (Wert)"},
                    { 35,  "Exportierte Leistung (Vorzeichen)"},
                    { 39,  "Batterie Spannung"},
                    { 40,  "Batterie Strom"},
                    { 41,  "Batterie Leistung (41)"},
                    { 44,  "Batterie Leistung (44)"},
                    { 47,  "Aktuelle Leistung"},
                    { 92,  "Heutiger Verbrauch"},
                    {103,  "Batterie Verbleibende Kapazität"},
                    {105,  "Batterie Temperatur"},
                    {106,  "Batterie Verbleibende Energie"},
                    {168,  "Batterie Betriebsmodus"},
                },
                Lang.French => new Dictionary<int, string>
                {
                    { -3,  "Puissance Batterie"},
                    { -2, "Puissance exportée"},
                    { -1, "Puissance totale des panneaux"},
                    {  0,  "Tension du réseau Phase 1"},
                    {  1,  "Tension du réseau Phase 2"},
                    {  2,  "Tension du réseau Phase 3"},
                    {  3,  "Courant de sortie Phase 1"},
                    {  4,  "Courant de sortie Phase 2"},
                    {  5,  "Courant de sortie Phase 3"},
                    {  6,  "Puissance actuelle Phase 1"},
                    {  7,  "Puissance actuelle Phase 2"},
                    {  8,  "Puissance actuelle Phase 3"},
                    {  9,  "Puissance CA"},
                    { 10,  "Tension PV1"},
                    { 11,  "Tension PV2"},
                    { 12,  "Courant PV1"},
                    { 13,  "Courant PV2"},
                    { 14,  "Puissance PV1"},
                    { 15,  "Puissance PV2"},
                    { 16,  "Fréquence du réseau Phase 1"},
                    { 17,  "Fréquence du réseau Phase 2"},
                    { 18,  "Fréquence du réseau Phase 3"},
                    { 19,  "Mode de fonctionnement onduleur"},
                    { 34,  "Puissance exportée (valeur)"},
                    { 35,  "Puissance exportée (signe)"},
                    { 39,  "Tension de la batterie"},
                    { 40,  "Courant de la batterie"},
                    { 41,  "Puissance de la batterie (41)"},
                    { 44,  "Puissance de la batterie (44)"},
                    { 47,  "Puissance actuelle"},
                    { 92,  "Consommation d'aujourd'hui"},
                    {103,  "Capacité restante de la batterie"},
                    {105,  "Température de la batterie"},
                    {106,  "Énergie restante de la batterie"},
                    {168,  "Mode de fonctionnement batterie"},
                },
                Lang.Spanish => new Dictionary<int, string>
                {
                    { -3,  "Potencia de la Batería"},
                    { -2, "Potencia exportada"},
                    { -1, "Potencia total de paneles"},
                    {  0,  "Voltaje de red Fase 1"},
                    {  1,  "Voltaje de red Fase 2"},
                    {  2,  "Voltaje de red Fase 3"},
                    {  3,  "Corriente de salida Fase 1"},
                    {  4,  "Corriente de salida Fase 2"},
                    {  5,  "Corriente de salida Fase 3"},
                    {  6,  "Potencia actual Fase 1"},
                    {  7,  "Potencia actual Fase 2"},
                    {  8,  "Potencia actual Fase 3"},
                    {  9,  "Potencia CA"},
                    { 10,  "Voltaje FV1"},
                    { 11,  "Voltaje FV2"},
                    { 12,  "Corriente FV1"},
                    { 13,  "Corriente FV2"},
                    { 14,  "Potencia FV1"},
                    { 15,  "Potencia FV2"},
                    { 16,  "Frecuencia de red Fase 1"},
                    { 17,  "Frecuencia de red Fase 2"},
                    { 18,  "Frecuencia de red Fase 3"},
                    { 19,  "Modo de operación del inversor"},
                    { 34,  "Potencia exportada (valor)"},
                    { 35,  "Potencia exportada (signo)"},
                    { 39,  "Voltaje de la batería"},
                    { 40,  "Corriente de la batería"},
                    { 41,  "Potencia de la batería (41)"},
                    { 44,  "Potencia de la batería (44)"},
                    { 47,  "Potencia actual"},
                    { 92,  "Consumo de hoy"},
                    {103,  "Capacidad restante de la batería"},
                    {105,  "Temperatura de la batería"},
                    {106,  "Energía restante de la batería"},
                    {168,  "Modo de operación de la batería"},
                },
                Lang.Czech => new Dictionary<int, string>
                {
                    { -3,  "Výkon baterie"},
                    { -2, "Exportovaný výkon"},
                    { -1, "Výkon všech panelů"},
                    {  0,  "Síťové napětí Fáze 1"},
                    {  1,  "Síťové napětí Fáze 2"},
                    {  2,  "Síťové napětí Fáze 3"},
                    {  3,  "Výstupní proud Fáze 1"},
                    {  4,  "Výstupní proud Fáze 2"},
                    {  5,  "Výstupní proud Fáze 3"},
                    {  6,  "Aktuální výkon Fáze 1"},
                    {  7,  "Aktuální výkon Fáze 2"},
                    {  8,  "Aktuální výkon Fáze 3"},
                    {  9,  "AC výkon"},
                    { 10,  "FV1 napětí"},
                    { 11,  "FV2 napětí"},
                    { 12,  "FV1 proud"},
                    { 13,  "FV2 proud"},
                    { 14,  "FV1 výkon"},
                    { 15,  "FV2 výkon"},
                    { 16,  "Síťová frekvence Fáze 1"},
                    { 17,  "Síťová frekvence Fáze 2"},
                    { 18,  "Síťová frekvence Fáze 3"},
                    { 19,  "Provozní režim měniče"},
                    { 34,  "Exportovaný výkon (hodnota)"},
                    { 35,  "Exportovaný výkon (znaménko)"},
                    { 39,  "Napětí baterie"},
                    { 40,  "Proud baterie"},
                    { 41,  "Výkon baterie (41)"},
                    { 44,  "Výkon baterie (44)"},
                    { 47,  "Aktuální výkon"},
                    { 92,  "Dnešní spotřeba"},
                    {103,  "Zbývající kapacita baterie"},
                    {105,  "Teplota baterie"},
                    {106,  "Zbývající energie baterie"},
                    {168,  "Provozní režim baterie"},
                },
                _ => throw new ArgumentOutOfRangeException(nameof(lang), lang, null)
            };
        }

        public static string GetValueName(Parameter dataValue, Lang lang = Lang.English)
        {
            var valueNames = GetValueNames(lang);
            return valueNames.TryGetValue((int)dataValue, out var name) ? name : dataValue.ToString();
        }
    }
}