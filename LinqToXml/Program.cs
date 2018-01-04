using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Console;

namespace LinqToXml
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Car> records = GetCars("fuel.csv");

            // Long form of creating xml
            //XDocument doc = CreateXml(records);

            // short form
            XDocument doc = CreateXmlShort(records);
            doc.Save("fuel.xml");

            QueryXml();

            ReadKey();
        }

        private static XDocument CreateXmlShort(IEnumerable<Car> records)
        {
            XDocument doc = new XDocument();
            XElement cars = new XElement("Cars", records.Select(c => new XElement("Car",
                new XAttribute("Name", c.Name),
                new XAttribute("Combined", c.Combined),
                new XAttribute("Manufacturer", c.Manufacturer)
                )));

            doc.Add(cars);

            return doc;
        }

        // Long form of creating xml
        private static XDocument CreateXml(IEnumerable<Car> records)
        {
            XDocument doc = new XDocument();
            // Always the xml document begins with a single root
            XElement cars = new XElement("Cars");

            foreach (Car item in records)
            {
                XElement car = new XElement("Car");
                XElement name = new XElement("Name", item.Name);
                XElement combined = new XElement("Combined", item.Combined);

                car.Add(name);
                car.Add(combined);

                cars.Add(car);
            }
            doc.Add(cars);

            return doc;
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

        private static void QueryXml()
        {
            var query = XDocument.Load("fuel.xml")
                .Element("Cars")
                .Elements("Car")
                .Where(c=>c.Attribute("Manufacturer").Value == "BMW");

            foreach (XElement element in query)
            {
                WriteLine($"{element.Attribute("Name")?.Value} - {element.Attribute("Manufacturer")?.Value}");
            }
        }
    }
}
