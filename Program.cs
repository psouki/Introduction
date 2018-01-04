using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Introduction.Extensions;

namespace Introduction
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Employee> firstList = new Employee[]
            {
                new Employee {Id = 1, Name = "Pedro"},
                new Employee {Id = 2, Name = "Elemar"}  
            };

            IEnumerable<Employee> secondList = new List<Employee>
            {
                 new Employee {Id = 1, Name = "Carlos"},
            };

            //IEnumerator<Employee> enumerator = firstList.GetEnumerator();
            IEnumerator<Employee> enumerator = secondList.GetEnumerator();

            //int count = firstList.Count();

            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Name);
            }

            // without LINQ 
            Console.ReadKey();
        }

       
    }
}
