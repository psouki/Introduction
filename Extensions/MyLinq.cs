using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Introduction.Extensions
{
    public static class MyLinq
    {
        public static int Count<T>(this IEnumerable<T> list)
        {
            int result = 0;
            foreach (var item in list)
            {
                result = result + 1;
            }

            return result;
        }
    }
}
