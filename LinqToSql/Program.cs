using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToSql.DynamicLambda;
using static System.Console;
namespace LinqToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
            QueryDataWithDynamicLambda();
        }

        private static IEnumerable<Car> GetCars(string path)
        {
            IEnumerable<Car> result = File.ReadAllLines(path)
                                            .Skip(1)
                                            .Where(l => l.Length > 1)
                                            .ToCars();

            return result;
        }

        public static void InsertData()
        {
            var db = new CarDb();
            var cars = GetCars("fuel.csv");

            // The db.Cars is under the control the context control
            // everything will be committed after the SaveChanges 
            foreach (Car car in cars)
            {
                db.Cars.Add(car);
            }
            db.SaveChanges();
        }

        private static void QueryData()
        {
            var db = new CarDb();

            // it can be used instead of sql profiler
            // it is the easier way to inspect the linq to sql translation
            db.Database.Log = WriteLine;

            // For read only data use AsNoTracking, that way the object will not be 
            // under the context control. It will increase the performance
            var query =
                from car in db.Cars.AsNoTracking()
                group car by car.Manufacturer into manufacturer
                select new
                {
                    Name = manufacturer.Key,
                    Cars = (from car in manufacturer
                            orderby car.Combined descending
                            select car).Take(2)
                };

            foreach (var group in query)
            {
                WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }
            // This is just a IQueryable, it has not hit the database yet.
            // It is just the query to be interpreted into sql
            IQueryable<Car> query2 = db.Cars.AsNoTracking()
                                       .Where(c => c.Name == "Ferrari");

            // It has hit the database and fetched the data
            IEnumerable<Car> cars = query2.ToList();

            // The ability to know where transform the query into data
            // can improve drastically the performance
            // here I'm ordering just the filtered data
            IEnumerable<Car> cars2 = query2.ToList().OrderBy(c => c.Year);

        }
        private static void QueryDataWithDynamicLambda()
        {
            var db = new CarDb();

            // Car with the Manufacturer that start with F
            QueryFilter filter1 = new QueryFilter("Manufacturer", "F", Operator.StartsWith);

            // Car with the year equals 2016
            QueryFilter filter2 = new QueryFilter("Year", 2016);


            ICollection<QueryFilter> filters = new List<QueryFilter>();
            filters.Add(filter1);
            filters.Add(filter2);

            // p => (p.Manufacturer.StartsWith("F") AndAlso (p.Year == 2016))
            var query = ExpressionBuilder.GetExpression<Car>(filters);

            IEnumerable<Car> cars = db.Cars.AsNoTracking().Where(query).ToList();

            foreach (var car in cars.OrderBy(c=>c.Manufacturer))
            {
                WriteLine(car.Manufacturer);
                WriteLine($"\t{car.Name}: {car.Combined}");
            }

            ReadKey();
        }
    }
}
