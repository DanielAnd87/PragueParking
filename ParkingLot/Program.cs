using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkingMenu menu = new ParkingMenu();
            menu.Menu();
            //   TestIncomeReport();
            //TestDataGenerator testDataGenerator = new TestDataGenerator();
            //testDataGenerator.TestData();

            //database.MoveVehicle("en bil", 13);
            //DateTime startDate = DateTime.Now;
            //startDate = startDate.AddDays(-4);
            //DateTime endDate = DateTime.Now;
            //database.Commit();
            //database.FetchVehicleInfo("en bil");
        }

        private static void TestIncomeReport()
        {
            DbHandler database = new DbHandler();
            DateTime startDate = AskForDate("2020/02/06");
            DateTime endDate = AskForDate("20sdfds20/02/09");

            decimal[] earninsReport = database.FetchEarningsSum(startDate, endDate);
            Console.Clear();
            foreach (decimal row in earninsReport)
            {
                string income = row.ToString();
                income = income.Substring(0, income.IndexOf(',') + 3);
                Console.WriteLine(income + " kronor.");

            }
        }

        public static DateTime AskForDate(string input)
        {
            string date = input;
            try
            {
                return DateTime.Parse(date);
            }
            catch
            {
                return AskForDate(input);
            }
        }

    }
}
