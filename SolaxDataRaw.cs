using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Solax.InverterHttpApi
{
    // 
    public class SolaxDataRaw
    {

        [JsonPropertyName("sn")]
        public string SerialNumber { get; set; } = string.Empty;

        [JsonPropertyName("ver")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("Data")]
        public List<int> Data { get; set; } = new List<int>();

        [JsonPropertyName("Information")]
        public List<object> Information { get; set; } = new List<object>();

        
    }
}