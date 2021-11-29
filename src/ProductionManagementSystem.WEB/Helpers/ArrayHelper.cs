using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductionManagementSystem.WEB.Helpers
{
    public static class ArrayHelper
    {
        public static string ToString(IEnumerable<int> array)
        {
            StringBuilder result = new StringBuilder("[");
            var list = (List<int>) array;
            for (int i = 0; i < list.Count() - 1 ; i++)
            {
                result.Append(list[i].ToString());
                result.Append(",");
            }
            
            result.Append(list[^1].ToString());
            result.Append("]");
            return result.ToString();
        }
        
        public static string ToString(IEnumerable<string> array)
        {
            StringBuilder result = new StringBuilder("[");
            var list = (List<string>) array;
            for (int i = 0; i < list.Count() - 1 ; i++)
            {
                result.Append("'" + list[i] + "'");
                result.Append(",");
            }
            
            result.Append("'" + list[^1] + "'");
            result.Append("]");
            return result.ToString();
        }
    }
}