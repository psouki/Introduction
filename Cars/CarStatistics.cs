using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    public class CarStatistics
    {
        public CarStatistics()
        {
            Max = int.MinValue;
            Min = int.MaxValue;
        }

        public string Manufacturer { get; set; }
        public double Total { get; set; }
        public double Count { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public double Avg { get; set; }

        public CarStatistics Acumulate(Car car)
        {
            Count++;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);
            return this;
        }

        public CarStatistics GetResult()
        {
            Avg = Total/Count;
            return this;
        }



    }
}
