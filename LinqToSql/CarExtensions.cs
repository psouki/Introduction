using System.Collections.Generic;
using System.Linq;

namespace LinqToSql
{
    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCars(this IEnumerable<string> lines)
        {
            return lines
                .Select(line => line.Split(','))
                .Select(columns => new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                }).ToList();
        }
    }
}
