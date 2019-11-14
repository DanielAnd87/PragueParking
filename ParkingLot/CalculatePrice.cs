using System;

namespace ParkingLot
{
    public static class CalculatePrice
    {
        private const float priceCar = 20;
        private const float priceMc = 10;


        public static float GetCost(Vehicle vehicle)
        {
            DateTime dt = vehicle.StartTime;
            DateTime currentDt = DateTime.Now;
            int minutes = 0, hours = 1;


            while (dt.Hour < currentDt.Hour || dt.DayOfYear != currentDt.DayOfYear)
            {
                hours++;
                dt = dt.AddHours(1);
            }
            while (dt.Minute < currentDt.Minute)
            {
                minutes++;
                dt = dt.AddMinutes(1);
            }


            if (hours == 1 && minutes <= 5)
            {
                return 0;
            }

            if (hours > 2)
            {
                float price = vehicle.IsCar ? priceCar : priceMc;


                float cost = hours * price;
                
                return cost;
            }
            
            if (hours <= 2)
            {
                return CalcDefaultPrice(vehicle);
            }
            return -1;
        }

        private static float CalcDefaultPrice(Vehicle vehicle)
        {
            float price;
            if (vehicle.IsCar)
            {
                price = 2 * priceCar;
            }
            else
            {
                price = 2 * priceMc;
            }

            return price;
        }
    }
}