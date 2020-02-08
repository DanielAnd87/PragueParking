using System;

namespace ParkingLot
{
    public static class CalculatePrice
    {
        private const float priceCar = 20;
        private const float priceMc = 10;


        public static float GetCost(DateTime dt, bool isCar)
        {
            DateTime currentDt = DateTime.Now;
            return GetCost(dt, currentDt, isCar);
        }

        public static float GetCost(DateTime inDateTime, DateTime outDate, bool isCar)
        {
            int minutes = 0, hours = 1;
            DateTime inDate = inDateTime;

            while (inDate.Hour < outDate.Hour || inDate.DayOfYear != outDate.DayOfYear)
            {
                hours++;
                inDate = inDate.AddHours(1);
            }
            while (inDate.Minute < outDate.Minute)
            {
                minutes++;
                inDate = inDate.AddMinutes(1);
            }


            if (hours == 1 && minutes <= 5)
            {
                return 0;
            }

            if (hours > 2)
            {
                float price = isCar ? priceCar : priceMc;


                float cost = hours * price;

                return cost;
            }

            if (hours <= 2)
            {
                return CalcDefaultPrice(isCar);
            }
            return -1;
        }

        private static float CalcDefaultPrice(bool isCar)
        {
            float price;
            if (isCar)
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