using System;

namespace ParkingLot
{
    public class Vehicle
    {
        private readonly string regNum;

        public string RegNum
        {
            get { return regNum; }
        }
        
        private readonly DateTime startTime;

        public DateTime StartTime
        {

            get { return startTime; }
        }
        private readonly bool isCar;

        public bool IsCar => isCar;

        public int ParkingNum { get; set; }

        private bool fillWholeSpace;

        public bool FillsWholeSpace
        {
            get { return fillWholeSpace; }
            set { fillWholeSpace = value; }
        }


        
        public Vehicle(string regNum, bool isCar)
        {
            this.regNum = regNum;
            startTime = DateTime.Now;
            this.ParkingNum = -1;

            this.isCar = isCar;
            this.fillWholeSpace = isCar;
        }
        public Vehicle(string regNum, bool isCar, bool fillWholeSpace, int parkingNum)
        {
            this.regNum = regNum;
            startTime = DateTime.Now;
            this.isCar = isCar;
            this.ParkingNum = parkingNum;

            this.fillWholeSpace = fillWholeSpace;
            if (isCar) fillWholeSpace = true;

        }
        public Vehicle(string regNum, bool isCar, bool fillWholeSpace, DateTime dateTime)
        {
            this.regNum = regNum;
            startTime = dateTime;
            this.isCar = isCar;
            this.fillWholeSpace = fillWholeSpace;
            if (isCar) fillWholeSpace = true;
            this.ParkingNum = -1;

        }
        public Vehicle(string regNum, bool isCar, bool fillWholeSpace, DateTime dateTime, int parkingNum)
        {
            this.regNum = regNum;
            startTime = dateTime;
            this.isCar = isCar;
            this.ParkingNum = parkingNum;
            this.fillWholeSpace = fillWholeSpace;
            if (isCar) fillWholeSpace = true;
        }



    }
}