using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ParkingLot
{
    public class UserInput : IUserInput
    {

        private int wrong_msg = 0;
        private static readonly string[] ERROR = {
            "",
            "Too long, try again!",
            "No whitespaces aloud, try again!",
            "Not on the list, try again!",
            "Write 'Y' (yes) or 'n' (no), try again!"
        };

        public string AskForReg()
        {
            Console.Clear();
            CheckIfRetry();
            Console.WriteLine("write esc to exit.\n\n");
            Console.WriteLine("Please enter vehicle reg-number");



            string regNum = Console.ReadLine().ToUpper();
            bool noWhiteSpaces = !regNum.Contains(" ");
            bool isLongerThenTen = regNum.Length <= 10;
            if (!(isLongerThenTen && noWhiteSpaces))
            {
                if (!isLongerThenTen)
                {
                    wrong_msg = 1;
                    return AskForReg();
                }
                else
                {
                    wrong_msg = 1;
                    return AskForReg();
                }
            }
            return regNum;
        }


        public int AskForNumber(int min, int max)
        {
            Console.Write("Enter a number.");
            int response;
            while (!int.TryParse(Console.ReadLine(), out response)) {
                Console.Clear();
                Console.WriteLine("That is not a number, try again!");
            }

            if (!Enumerable.Range(min, max).Contains(response))
            {
                return AskForNumber(min, max);
            }
            return response;


        }
        
        public int AskForOptimizationChoice(int[,] choices, List<Vehicle> vehicles)
        {
            //todo: Not implemented yet.
            int numChoices = choices.GetLength(0);
            for (int i = 0; i < numChoices; i++)
            {
                

                int firstChoice = vehicles[choices[i, 1]].ParkingNum+1;
                int secondChoice = vehicles[choices[i, 0]].ParkingNum+1;
                Console.WriteLine("[{2}] Do you want to move bike on space {0} next to bike on space {1}?", firstChoice, secondChoice, i+1);
            }

            return AskForNumber(1, numChoices)-1;
        }

        private void CheckIfRetry()
        {
            Console.WriteLine(ERROR[wrong_msg]);
            wrong_msg = 0;
        }

        public bool AskForType()
        {
            Console.WriteLine("[1] CAR");
            Console.WriteLine("[2] MC");
            return AskForNumber(1,2)  == 1 ? true : false;
        }

        public bool OrderUserToMoveCars(List<int> optimizeChoises, string regPlate)
        {
            CheckIfRetry();
            Console.WriteLine("Move vehicle {0} from {1} to {2}. To comply write Y, else n", regPlate, optimizeChoises[1] + 1,optimizeChoises[0]+1);
            Console.WriteLine("Flytta MC {0} från {1} till {2}, om ja mata in Y, annars n.", regPlate, optimizeChoises[1] + 1,optimizeChoises[0]+1);
            //string response = Console.ReadLine();
            string response = Console.ReadLine();
            if (response == "Y")
            {
                return true;
            }

            if (response == "n")
            {
                return false;
            }
            else
            {
                wrong_msg = 4;
                return OrderUserToMoveCars(optimizeChoises, regPlate);
            }

        }

        public bool DriveOrder(Vehicle vehicle)
        {
            Console.WriteLine($"Drive vehicle {vehicle.RegNum} to parking-space {vehicle.ParkingNum + 1}.");
            Console.WriteLine("Flytta MC {0} till {1}, om ja mata in Y, annars n.", vehicle.RegNum, vehicle.ParkingNum + 1);

            BackToMenu();
            return true;
        }
        public bool MoveOrder(Vehicle vehicle, int from)
        {
            Console.WriteLine("Flytta MC {0} från plats {1} till {2}", vehicle.RegNum, from+1, vehicle.ParkingNum + 1);
            Console.WriteLine("Move MC {0} from {1} to {2}", vehicle.RegNum, from+1, vehicle.ParkingNum + 1);

            BackToMenu();
            return true;
        }


        public bool PrintNothingtoSeeHere()
        {
            Console.WriteLine("Everything is as good as it gets,");
            Console.WriteLine("press enter to continue to menu.");
            Console.ReadLine();
            return true;

        }

        public bool BackToMenu()
        {
            Console.WriteLine("Press enter to return to menu.");
            Console.ReadLine();
            return true;
        }

        public bool CheckOutVehicle(Vehicle vehicle)
        {
            PrintCheckOut(vehicle);
            BackToMenu();
            return true;
        }




        public void PrintCheckOut(Vehicle vehicle)
        {
            // need to tell user where to find the checked out vehicle.
            Console.WriteLine(String.Format("The vehicle with reg-number {1} has been checked out.\nIn time: {2}\nOut time: {3}.\nPlease remove {1} from parkingspace {0}.",
                vehicle.ParkingNum+1,
                vehicle.RegNum,
                vehicle.StartTime.ToString(),
                DateTime.Now.ToString()
            ));

            PrintPrice(vehicle);
        }

        public static void PrintPrice(Vehicle vehicle)
        {
            //todo: fix wrong price for vehicles just arived.
            // todo: fix visuliser!
            Console.WriteLine("Price: {0:F2} koruna", CalculatePrice.GetCost(vehicle));
        }
    }
}