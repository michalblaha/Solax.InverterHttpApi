namespace Solax.InverterHttpApi
{
    public class SolaxInformation
    {
        public int InverterType { get; set; }
        public string InverterSerialNumber { get; set; }
        public string DspVersion { get; set; }
        public string ArmVersion { get; set; }
        public int TransportType { get; set; }
        public string DongleModType { get; set; }
        public string DongleWorkMode { get; set; }

        public string RunMode { get; set; }

        private readonly SolaxDataRaw solaxDataRaw;

        public SolaxInformation(SolaxDataRaw solaxDataRaw)
        {
            if (solaxDataRaw.Information == null)
            {
                throw new ArgumentNullException(nameof(solaxDataRaw.Information), "Information cannot be null.");
            }   
            var data = solaxDataRaw.Information;
            this.solaxDataRaw = solaxDataRaw;
            InverterType = GetVal<int>(1);

            if (InverterType >= 8)
            {
                InverterSerialNumber = GetVal<string>(2);
                DspVersion = GetVal<string>(4);
                ArmVersion = GetVal<string>(6);
                TransportType = 1;
            }
            else
            {
                InverterSerialNumber = GetVal<string>(3);
                DspVersion = GetVal<string>(5);
                ArmVersion = GetVal<string>(7);
                TransportType = GetVal<int>(9);
                
            }
            DongleModType = GetVal<string>(10);
            DongleWorkMode = GetVal<string>(11);
        }

        private T GetVal<T>(int index) { 
            if (index < 0 || index >= solaxDataRaw.Information.Count)
            {
                return default;
            }
            switch (Type.GetTypeCode( typeof(T)))
            {
                case TypeCode.Int32:
                    return int.TryParse(solaxDataRaw.Information[index].ToString(), out int intValue) ? (T)(object)intValue : default;
                case TypeCode.String:
                    return (T)(object)(solaxDataRaw.Information[index]?.ToString() ?? "");
                case TypeCode.Decimal:
                    return decimal.TryParse(solaxDataRaw.Information[index].ToString(), System.Globalization.CultureInfo.InvariantCulture, out decimal decValue) 
                            ? (T)(object)decValue : default;
                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }
        }

        public static string GetRunMode(int inverterType, int runModeValue)
        {
            Dictionary<int, string>? workingmode = inverterType switch
            {
                3  => _workingModes[3],
                13 => _workingModes[3],
                14 => _workingModes[3],
                15 => _workingModes[3],
                17 => _workingModes[3],

                4 => _workingModes[4],
                6 => _workingModes[4],
                7 => _workingModes[4],
                8 => _workingModes[4],
                16 => _workingModes[4],

                5 => _workingModes[5],
                9 => _workingModes[9],

                10 => _workingModes[10],
                11 => _workingModes[10],
                12 => _workingModes[10],

                18 => _workingModes[18],

                19 => _workingModes[19],
                20 => _workingModes[19],
                21 => _workingModes[19],

                23 => _workingModes[23],
                24 => _workingModes[23],

                100 => _workingModes[100],
                101 => _workingModes[100],
                _ => null
            };

            if ( workingmode?.TryGetValue(runModeValue, out string val) == true)
            {
                return val;
            }
            return string.Empty;
        }

        private static readonly Dictionary<int, Dictionary<int, string>> _workingModes = new()
        {
            // Inverter types: 3, 13, 14, 15, 17
            [3] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EPS Checking/Waiting",
                [7] = "EPS",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "Standby"
            },

            // Inverter types: 4, 6, 7, 8, 16
            [4] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading"
            },

            // Inverter type: 5
            [5] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EPS Checking/Waiting",
                [7] = "EPS",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "PvWakeUpBat",
                [11] = "Standby"
            },

            // Inverter type: 9
            [9] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "Standby"
            },

            // Inverter types: 10, 11, 12
            [10] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EPS Checking/Waiting",
                [7] = "EPS",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "Gen Check",
                [11] = "Gen Run",
                [12] = "RSD Standby"
            },

            // Inverter type: 18
            [18] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EpsCheckMode",
                [7] = "EpsMode"
            },

            // Inverter types: 19, 20, 21
            [19] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EPS Checking",
                [7] = "EPS",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "Standby",
                [11] = "Gen Checking",
                [12] = "Gen Run",
                [13] = "RSD Standby"
            },

            // Inverter types: 23, 24
            [23] = new Dictionary<int, string>
            {
                [0] = "Waiting",
                [1] = "Checking",
                [2] = "Normal",
                [3] = "Fault",
                [4] = "Permanent Fault",
                [5] = "Upgrading",
                [6] = "EPS Checking/Waiting",
                [7] = "EPS",
                [8] = "Self Testing",
                [9] = "Idle",
                [10] = "Standby"
            },

            // Inverter types: 100, 101
            [100] = new Dictionary<int, string>
            {
                [0] = "Init",
                [1] = "Idle",
                [2] = "Start",
                [3] = "Run",
                [4] = "Fault",
                [5] = "Upgrading"
            }
        };
    }
}