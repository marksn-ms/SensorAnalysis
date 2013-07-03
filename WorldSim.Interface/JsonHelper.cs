using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace WorldSim.Interface
{
    public class JsonHelper
    {
        public static string WorldTypeToJson(string strKind, object o, int count = 1)
        {
            string strType = "";
            strType = strKind + ", count:" + count + ", type:\"" + o.GetType().ToString() + "\", assembly:\"" + o.GetType().Module.Assembly.FullName + "\"";

            var query = from PropertyInfo pi in o.GetType().GetProperties()
                        where pi.GetCustomAttributes(typeof(CategoryAttribute), true).Any(n => (n as CategoryAttribute).Category == "Initialization")
                        select new { name = pi.Name, value = pi.GetValue(o, null) };
            foreach (var p in query)
            {
                string serializedValue = SerializeBase64(p.value);
                strType += ", " + p.name + ":\"" + serializedValue + "\"";
            }

            Debug.WriteLine(strType);
            return strType;
        }

        public static string SerializeBase64(object o)
        {
            // Serialize to a base 64 string
            byte[] bytes;
            long length = 0;
            MemoryStream ws = new MemoryStream();
            BinaryFormatter sf = new BinaryFormatter();
            sf.Serialize(ws, o);
            length = ws.Length;
            bytes = ws.GetBuffer();
            string encodedData = bytes.Length + ":" + Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
            return encodedData;
        }

        public static object DeserializeBase64(string s)
        {
            // We need to know the exact length of the string - Base64 can sometimes pad us by a byte or two
            int p = s.IndexOf(':');
            int length = Convert.ToInt32(s.Substring(0, p));

            // Extract data from the base 64 string!
            byte[] memorydata = Convert.FromBase64String(s.Substring(p + 1));
            MemoryStream rs = new MemoryStream(memorydata, 0, length);
            BinaryFormatter sf = new BinaryFormatter();
            object o = sf.Deserialize(rs);
            return o;
        }
    }
}
