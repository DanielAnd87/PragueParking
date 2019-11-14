using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingLot
{
    // Bara en stub till testning.
    public class UserInputStub : UserInput
    {
        private string regNum;

        public string RegNum
        {
            get { return regNum;}
            set
            {
                regNum = value; 

            }
        }




        private bool wrong = false;
        public bool IsCar { get; set; }

        private int parkingNum;
        public int ParkingNum { get; set; }

        private bool fillWholeSpace;
        public bool FillWholeSpace { get; set; }
        public int IndexAnswer { get; set; }


        public string AskForReg()
        {
            //todo: Not implemented yet.
            return regNum;
        }
        public int AskForNumber(int i, int j)
        {
            //todo: Not implemented yet.
            return IndexAnswer;
        }
        
        public int AskForOptimizationChoice(int[,] choices)
        {
            CheckIfRetry();
            //todo: Not implemented yet.
            int numChoices = choices.GetLength(0);
            for (int i = 0; i < numChoices; i++)
            {
                int firstChoice = choices[i, 0];
                int secondChoice = choices[i, 1];
                Console.WriteLine("[{2}] Do you want to move bike on {1} next to bike on {0}?", firstChoice, secondChoice, i+1);
            }

            //int chosen = AskForNumber();
            int chosen = IndexAnswer;
            // todo: correct user if he print wrong number.
            // todo: check for IndexOutOfRange.
            if (Enumerable.Range(0, numChoices).Contains(chosen)) return IndexAnswer;
            
            else
            {
                wrong = true;
                return AskForOptimizationChoice(choices);
            }
        }

        private void CheckIfRetry()
        {
            if (wrong) Console.WriteLine("Try again,");
            wrong = false;
        }

        public bool AskForType()
        {
            //todo: Not implemented yet.
            return IsCar;
        }

        public bool OrderUserToMoveCars(List<int> optimizeChoises, string regPlate)
        {
            CheckIfRetry();
            Console.WriteLine("Move vehicle {0} from {1} to {2}. To comply write Y, else n", regPlate, optimizeChoises[0], optimizeChoises[1]);
            //string response = Console.ReadLine();
            string response = "Y";
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
                wrong = false;
                return OrderUserToMoveCars(optimizeChoises, regPlate);
            }

        }

        public static void PrintPrice(Vehicle vehicle)
        {

        }

        public void BackToMenu()
        {

        }

        public void DriveOrder(Vehicle vehicle)
        {

        }

        public void CheckOutVehicle(Vehicle vehicle)
        {

        }

        public void MoveOrder(Vehicle searchForVehicle, int place)
        {

        }

        public void PrintNothingtoSeeHere()
        {
            Console.WriteLine("Everything is as good as it gets,");
            Console.WriteLine("press enter to continue to menu.");
            //Console.ReadLine();
        }
    }
}