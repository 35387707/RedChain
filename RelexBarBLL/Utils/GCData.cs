using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Utils
{
    public class GCData
    {
        //采用排序的Dictionary的好处是方便对数据包进行签名，不用再签名之前再做一次排序
        private System.Collections.Generic.SortedDictionary<string, object> m_values =
            new System.Collections.Generic.SortedDictionary<string, object>();
        /**
        * 设置某个字段的值
        * @param key 字段名
         * @param value 字段值
        */
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }

        /**
        * 根据字段名获取某个字段的值
        * @param key 字段名
         * @return key对应的字段值
        */
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }
        //将字典转换为url参数形式
        public string ToUrl()
        {
            string buff = "";
            foreach (System.Collections.Generic.KeyValuePair<string, object> pair in m_values)
            {


                //if (pair.Key != "sign" && pair.Value.ToString() != "")
                //{
                //    buff += pair.Key + "=" + pair.Value + "&";
                //}
                string tempval = pair.Value == null ? "" : pair.Value.ToString();

                buff += pair.Key + "=" + pair.Value + "&";
                //if (pair.Key != "sign" && tempval != "")
                //{
                //    buff += pair.Key + "=" + pair.Value + "&";
                //}
            }
            buff = buff.Trim('&');
            return buff;
        }
        private string MakeSignToUrl()
        {
            string buff = "";
            foreach (System.Collections.Generic.KeyValuePair<string, object> pair in m_values)
            {
                string tempval = pair.Value == null ? "" : pair.Value.ToString();

                if (pair.Key != "sign" && tempval != "")
                {
                    buff += pair.Key + "=" + pair.Value + "&";
                }
            }
            buff = buff.Trim('&');
            return buff;
        }
        public string MakeSign(string AppSecret)
        {
            //转url格式
            string str = MakeSignToUrl();

            var sb = new System.Text.StringBuilder();
            //MD5加密
            //在string后加入API KEY
            str += "&key=" + AppSecret;
            var md5 = System.Security.Cryptography.MD5.Create();
            var bs = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }
    }
}
