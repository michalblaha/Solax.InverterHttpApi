using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solax.InverterHttpApi
{
    public class SolaxDataSimple
    {
        public SolaxDataSimple(SolaxDataList result)
        {
            PVAllPower = result.GetValue(SolaxDataList.Parameter.AllPanelPower).Value;
            PV1Power = result.GetValue(SolaxDataList.Parameter.PV1Power).Value;
            PV2Power = result.GetValue(SolaxDataList.Parameter.PV2Power).Value;
            BatteryPower = result.GetValue(SolaxDataList.Parameter.BatteryPower).Value;
            BatteryRemainingCapacity = result.GetValue(SolaxDataList.Parameter.BatteryRemainingCapacity).Value;
            //PowerNow = result.PowerNow.Value;
            ExportedPower = result.GetValue(SolaxDataList.Parameter.ExportedPower).Value; ;
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
