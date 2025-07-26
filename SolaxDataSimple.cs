using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solax.InverterHttpApi
{
    public class SolaxDataSimple
    {
        public SolaxDataSimple(SolaxData result)
        {
            PVAllPower = result.GetValue(SolaxData.Parameter.AllPanelPower).Value;
            PV1Power = result.GetValue(SolaxData.Parameter.PV1Power).Value;
            PV2Power = result.GetValue(SolaxData.Parameter.PV2Power).Value;
            BatteryPower = result.GetValue(SolaxData.Parameter.BatteryPower).Value;
            BatteryRemainingCapacity = result.GetValue(SolaxData.Parameter.BatteryRemainingCapacity).Value;
            //PowerNow = result.PowerNow.Value;
            ExportedPower = result.GetValue(SolaxData.Parameter.ExportedPower).Value; ;
        }

        public decimal PVAllPower { get; set; }
        public decimal PV1Power { get; set; }
        public decimal PV2Power { get; set; }
        public decimal BatteryPower { get; set; }
        public decimal BatteryRemainingCapacity { get; set; }
        public decimal PowerNow { get; set; }
        public decimal ExportedPower { get; set; }
    }
}
