namespace Solax.InverterHttpApi
{
    public class InverterModesDict : Dictionary<int, string>
    {
        public static InverterModesDict Modes = new InverterModesDict
        {
            {0, "Waiting"},
            {1, "Checking"},
            {2, "Normal"},
            {3, "Off"},
            {4, "Permanent Fault"},
            {5, "Updating"},
            {6, "EPS Check"},
            {7, "EPS Mode"},
            {8, "Self Test"},
            {9, "Idle"},
            {10, "Standby"},
        };

        public static string GetModeName(int mode)
        {
            if (mode < 0)
                throw new ArgumentOutOfRangeException(nameof(mode), "Mode must be non-negative.");
            return Modes.TryGetValue(mode, out var name) ? name : "?";
        }
    }
}
