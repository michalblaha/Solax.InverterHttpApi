namespace Solax.InverterHttpApi
{
    public class BatteryModesDict : Dictionary<int, string>
    {
        public static BatteryModesDict Modes = new BatteryModesDict
        {
            { 0, "Self Use Mode" },
            { 1, "Force Time Use" },
            { 2, "Back Up Mode" },
            { 3, "Feed-in Priority" },
        };

        public static string GetModeName(int mode)
        {
            if (mode < 0)
                throw new ArgumentOutOfRangeException(nameof(mode), "Mode must be non-negative.");
            return Modes.TryGetValue(mode, out var name) ? name : "?";
        }
    }
}
