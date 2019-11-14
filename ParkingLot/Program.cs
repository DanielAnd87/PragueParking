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
            Menu menu = new Menu();
        }
        

        static void TestPrice()
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-3);
            Vehicle vehicle = new Vehicle("", true,false, dateTime);

            Console.WriteLine("En {2} som ståt parkerad i {0} timmar kostar {1} kronor.",3,CalculatePrice.GetCost(vehicle), "bil");
            
            dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-3);
            dateTime = dateTime.AddMinutes(-10);
            vehicle = new Vehicle("", true,false, dateTime);

            Console.WriteLine("En {2} som stått parkerad i {0} kostar {1} kronor.","3 timmar och 10 minuter",CalculatePrice.GetCost(vehicle), "bil");
            
            dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-113);
            dateTime = dateTime.AddMinutes(-10);
            vehicle = new Vehicle("", true,false, dateTime);

            Console.WriteLine("En {2} som stått parkerad i {0} kostar {1} kronor.","113 timmar och 10 minuter",CalculatePrice.GetCost(vehicle), "bil");
            
            dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-2);
            vehicle = new Vehicle("", false,false, dateTime);

            Console.WriteLine("En {2} som stått parkerad i {0} kostar {1} kronor.","2 timmar och 0 minuter",CalculatePrice.GetCost(vehicle), "MC");
            dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-1);
            dateTime = dateTime.AddMinutes(-10);
            vehicle = new Vehicle("", false,false, dateTime);

            Console.WriteLine("En {2} som stått parkerad i {0} kostar {1} kronor.","1 timme och 10 minuter",CalculatePrice.GetCost(vehicle), "MC");
            
            dateTime = DateTime.Now;
            dateTime = dateTime.AddMinutes(-4);
            vehicle = new Vehicle("", false,false, dateTime);

            Console.WriteLine("En {2} som stått parkerad i {0} timmar kostar {1} kronor.","4 minuter",CalculatePrice.GetCost(vehicle), "MC");
            
        }
    }
}
