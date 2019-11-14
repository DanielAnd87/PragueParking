using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingLot
{
    public class VizulizeParkinglot
    {
        private readonly int[,] parkingArray;
        private List<Vehicle> vehicles;
        public new enum Types
        {
            E, C, m
        }
        public VizulizeParkinglot(List<Vehicle> list)
        {
            vehicles = list;
            parkingArray = new int[100,2];
            ListToArray(list);
        }


        public void PresentRows()
        {
            Console.Clear();
            for (int i = 0; i < 10; i++)
            {
                int mcs = 0, cars = 0;
                for (int j = 0; j < 10; j++)
                {
                    if (parkingArray[j + i * 10, 0] == (int) Types.C) cars++;
                    if (parkingArray[j + i * 10, 1] == (int) Types.C) cars++;
                    if (parkingArray[j + i * 10, 0] == (int) Types.m) mcs++;
                    if (parkingArray[j + i * 10, 1] == (int) Types.m) mcs++;
                }

                Console.WriteLine($"[{i+1}] {mcs} motorcykles and {cars} cars on row {i+1}");
            }

            UserInput user = new UserInput();
            int answer = user.AskForNumber(1, 10);
            answer--;

            PresentRow(answer);
        }

        private void PresentRow(int answer)
        {
            var enumerable = Enumerable.Range(answer * 10, answer * 10 + 10);
            int num = 0;
            foreach (Vehicle vehicle in vehicles)
            {
                if (enumerable.Contains(vehicle.ParkingNum))
                {
                    num++;
                    PrintVehicleInfo(vehicle);
                }
            }

            Console.WriteLine("");
            Console.WriteLine($"{num} vehicles in that row. Press enter to continue.");
            Console.ReadLine();
        }

        public static void PrintVehicleInfo(Vehicle vehicle)
        {
            string type = vehicle.IsCar ? "car" : "bike";
            Console.WriteLine();
            Console.Write($"A {type} with regplate ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(vehicle.RegNum);
            Console.ForegroundColor = ConsoleColor.Gray;
            
            Console.Write($" at {vehicle.ParkingNum + 1} ");
            Console.WriteLine($"Checked in at: {vehicle.StartTime.ToString()}");
            Console.ForegroundColor = ConsoleColor.Green;
            UserInput.PrintPrice(vehicle);
            Console.ForegroundColor = ConsoleColor.Gray;
        }


        private void ListToArray(List<Vehicle> list)
        {
            foreach (Vehicle vehicle in list)
            {
                if (vehicle.FillsWholeSpace)
                {
                    //check if one row is occupied.
                    if (parkingArray[vehicle.ParkingNum, 0] != 0)
                    {
                        parkingArray[vehicle.ParkingNum, 1] = vehicle.IsCar ? (int) Types.C :(int) Types.m;

                    }
                    else
                    {
                        parkingArray[vehicle.ParkingNum, 0] = vehicle.IsCar ? (int)Types.C : (int)Types.m;
                    }
                }
                else
                {
                    parkingArray[vehicle.ParkingNum, 0] = vehicle.IsCar ? (int)Types.C : (int)Types.m;

                }
            }
        }



        public void PrintParkinglot()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var types = (Types)parkingArray[(j)+ (i*10), 0];
                    var types1 = (Types)parkingArray[(j) + (i*10), 1];
                    string parking = types.ToString();
                    string parking1 = types1.ToString();
                    if (types == Types.E) parking = " ";
                    if (types1 == Types.E) parking1 = " ";
                    if (types == Types.C) parking = "C";
                    if (types1 == Types.C) parking1 = "C";
                    if (types == Types.m) parking = "M";
                    if (types1 == Types.m) parking1 = "M";
                    bool left = true;
                    while (parking.Length < "  ".Length)
                    {
                        if (left )
                        {
                            parking = parking.Insert(0, " ");
                        }
                        else
                        {
                            parking = parking.Insert(parking.Length, " ");


                        }

                        left = !left;
                    }

                    PrintParkingSpace(types, parking, parking1);
                }
                Console.WriteLine("");

                for (int j = 0; j < 10; j++)
                {
                    PrintParkingSpace(Types.E, " ", "  ");
                }
                Console.WriteLine("");
                Console.WriteLine("");

                System.Threading.Thread.Sleep(25);
            }

            Console.ForegroundColor = ConsoleColor.Gray;


        }

        private static void PrintParkingSpace(Types types, string parking, string parking1)
        {
            Console.Write("|");
            
            if (types == Types.E) Console.ForegroundColor = ConsoleColor.Green;
            if (types == Types.m) Console.ForegroundColor = ConsoleColor.Blue;
            if (types == Types.C) Console.ForegroundColor = ConsoleColor.Yellow;


            Console.Write($"{parking}");
            Console.Write($"{parking1}");


            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("| ");
        }




    }

}

