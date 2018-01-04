using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Car> cars = GetCars("fuel.csv");
            IEnumerable<Manufacturer> manufacturers = ProcessManufacturers("manufacturers.csv");

            // Example of filter, ordering and projecting
            //FilterOrderProjectingExample(cars);

            // Example of join
            // JoinExample(cars, manufacturers);

            // Example of grouping and grouping with join
            //GroupAndGroupjoinExamples(cars, manufacturers);

            // Example of Aggregation
            AggregateExample(cars);
        }

        private static IEnumerable<Car> GetCars(string path)
        {
            //It is possible to do it either by using select and passing a function
            //IEnumerable<Car> result = File.ReadAllLines(path)
            //                                .Skip(1)
            //                                .Where( l => l.Length > 1)
            //                                .Select(LineToCar)
            //                                .ToList();

            // Or create a extending method 
            IEnumerable<Car> result = File.ReadAllLines(path)
                                            .Skip(1)
                                            .Where(l => l.Length > 1)
                                            .ToCars();

            return result;
        }

        //it is not need to use function or extension, it can be done all together
        //I did it for demonstration purpose. 
        public static IEnumerable<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    string[] columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });

            return query.ToList();
        }
        private static Car LineToCar(string line)
        {
            string[] columns = line.Split(',');
            return new Car
            {
                Year = int.Parse(columns[0]),
                Manufacturer = columns[1],
                Name = columns[2],
                Displacement = double.Parse(columns[3]),
                Cylinders = int.Parse(columns[4]),
                City = int.Parse(columns[5]),
                Highway = int.Parse(columns[6]),
                Combined = int.Parse(columns[7])
            };
        }

        private static void FilterOrderProjectingExample(IEnumerable<Car> cars)
        {
            // Returning the whole object
            //var query = cars
            //    .OrderByDescending(c => c.Combined)
            //    .ThenBy(c=>c.Name)
            //    .Take(10);

            // returning a projection with fewer columns using Lambda
            var query = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .Select(c => new
                {
                    c.Manufacturer,
                    c.Name,
                    c.Combined
                })
                .Take(10);

            foreach (var car in query)
            {
                WriteLine($"{car.Name} - {car.Manufacturer} - Gas : {car.Combined}");
            }

            // the same as above using query expressions syntax
            var query2 = (from car in cars
                          where car.Manufacturer == "Ferrari" && car.Year == 2016
                          orderby car.Combined descending, car.Name
                          select new
                          {
                              car.Manufacturer,
                              car.Name,
                              car.Combined
                          }).Take(10);

            foreach (var car in query2)
            {
                WriteLine($"{car.Name} - {car.Manufacturer} - Gas : {car.Combined}");
            }

        }

        private static void JoinExample(IEnumerable<Car> cars, IEnumerable<Manufacturer> manufacturers)
        {
            // join example in the query syntax 
            //var query = from car in cars
            //            join manufacturer in manufacturers
            //                on car.Manufacturer equals manufacturer.Name
            //            orderby car.Combined descending, car.Name
            //            select new // do the projection normally 
            //            {
            //                manufacturer.Headquarters,
            //                car.Name,
            //                car.Combined
            //            };

            // when it starts to be more complex like join with more than one key, 
            // the query syntax seems to be more appropriate
            var query = from car in cars
                        join manufacturer in manufacturers
                            on new { car.Manufacturer, car.Year }
                        equals new { Manufacturer = manufacturer.Name, manufacturer.Year }
                        orderby car.Combined descending, car.Name
                        select new // do the projection normally 
                        {
                            manufacturer.Headquarters,
                            car.Name,
                            car.Combined
                        };

            // join example in the lambda syntax
            var query2 = cars.Join(manufacturers,
                c => c.Manufacturer,
                m => m.Name, (c, m) => new // function that receives two parameters
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                }) // create one result in form of IEnumerable
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name);

            foreach (var car in query.Take(10))
            {
                WriteLine($"{car.Name} - {car.Headquarters} - Gas : {car.Combined}");
            }
        }

        private static void GroupAndGroupjoinExamples(IEnumerable<Car> cars, IEnumerable<Manufacturer> manufacturers)
        {
            // Group by query syntax version 
            //var query =
            //    from car in cars
            //    group car by car.Manufacturer.ToUpper() into manufacturer
            //    orderby manufacturer.Key
            //    select manufacturer;

            // the same as above in lambda syntax
            //var query2 = cars.GroupBy(c => c.Manufacturer)
            //    .OrderByDescending(c => c.Key);

            //foreach (var group in query)
            //{
            //    WriteLine(group.Key);
            //    foreach (Car car in group.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        WriteLine($"\t {car.Name} - {car.Combined}");
            //    }
            //}

            // Group by with join in query syntax
            //var query =
            //    from manufacturer in manufacturers
            //    join car in cars on manufacturer.Name equals car.Manufacturer
            //        into carGroup
            //    orderby manufacturer.Name
            //    select new
            //    {
            //        Manufacturer = manufacturer,
            //        Cars = carGroup
            //    };

            // the same as above in lambda syntax
            //var query2 = manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer,
            //    (m, c) => new
            //    {
            //        Manufacturer = m,
            //        Cars = c
            //    })
            //    .OrderBy(c => c.Manufacturer.Name);

            //foreach (var group in query2)
            //{
            //    WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
            //    foreach (Car car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
            //    {
            //        WriteLine($"\t {car.Name} - {car.Combined}");
            //    }
            //}

            // when the key for the grouping is not the joing key
            // just playing with the posibilities.
            var query =
                cars.Join(manufacturers, c => c.Manufacturer, m => m.Name,
                (c, m) => new
                {
                    Country = m.Headquarters,
                    Company = m.Name,
                    c.Name,
                    c.Combined
                })
                .GroupBy(c => c.Country)
                .OrderBy(c => c.Key);

            foreach (var group in query)
            {
                WriteLine(group.Key);
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(3))
                {
                    WriteLine($"\t {car.Company} -> {car.Name} : {car.Combined}");
                }
            }


        }

        private static void AggregateExample(IEnumerable<Car> cars)
        {
            // For aggregation in small group of data it works, 
            // but for large groups its inefficient because it makes 3 loops
            var query = from car in cars
                        group car by car.Manufacturer
                into carGroup
                        select new
                        {
                            Name = carGroup.Key,
                            Max = carGroup.Max(c => c.Combined), // itereate 1st time
                            Min = carGroup.Min(c => c.Combined), // itereate 2nd time
                            Avg = carGroup.Average(c => c.Combined) // itereate 3rd time
                        } into result
                        orderby result.Max descending
                        select result;

            // In this way all 3 calculations are done in just one loop.
            // with help of the Aggregate method
            var query2 = cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var result = g.Aggregate(new CarStatistics(),
                        (acc, car) => acc.Acumulate(car),
                        acc => acc.GetResult());
                    return new
                    {
                        Name = g.Key,
                        result.Max,
                        result.Min,
                        result.Avg
                    };
                })
                .OrderByDescending(c => c.Max);

            foreach (var car in query2)
            {
                WriteLine(car.Name);
                WriteLine($"\t {car.Max}");
                WriteLine($"\t {car.Min}");
                WriteLine($"\t {car.Avg}");
            }
        }
    }


}
