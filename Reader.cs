using Solax.InverterHttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Solax.InverterHttpApi
{
    public static class Reader
    {
        public static async Task<SolaxDataRaw?> GetDataAsync(            
            string dongleUrl, string password,
            SolaxDataList.Lang lang = SolaxDataList.Lang.English
            )
        {
            return await GetDataAsync(new Uri(dongleUrl), password);
        }
        public static async Task<SolaxDataRaw?> GetDataAsync(Uri dongleUri, string password)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("X-Forwarded-For", "5.8.8.8");

                    var postData = new StringContent("optType=ReadRealTimeData&pwd=" + password);
                    var response = await httpClient.PostAsync(dongleUri, postData);
                    var responseData = await response.Content.ReadAsStringAsync();

                    try
                    {
                        return JsonSerializer.Deserialize<SolaxDataRaw>(responseData);
                    }
                    catch (Exception ex)
                    {

                        return null;
                    }
                }
                catch (Exception ex)
                {

                    return null;
                }
            }
        }
    }
}
