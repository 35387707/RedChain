using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Json
{
    public static class JSONHelper
    {
        public static string ToJson(this object obj, string dateformart = null)
        {
            if (!string.IsNullOrEmpty(dateformart))
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = dateformart;
                return JsonConvert.SerializeObject(obj, Formatting.Indented, timeFormat);
            }

            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static T DeJson<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public static JObject GetJObject(string jsonstr)
        {
            return (JObject)JsonConvert.DeserializeObject(jsonstr);
        }
    }
}
