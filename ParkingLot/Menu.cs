using System;
using System.Text;

namespace ParkingLot
{
    public class Menu
    {
        private readonly Organizer organizer;
        private readonly UserInput userInput;
        private VizulizeParkinglot vizulize;

        public Menu()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            // NSimSun
            userInput = new UserInput();
            organizer = new Organizer(userInput);
            organizer.RestoreFromFile();
            
            MainMenu();
        }




        public void MainMenu()
        {
            Console.Clear();

            vizulize = new VizulizeParkinglot(organizer.vehicles);
            vizulize.PrintParkinglot();

            //todo: Visulizer not implemented yet.
            // VISULIZER.PrintParkinglot(true, mParkingData);

            Console.WriteLine("\n\n\n");
            Console.WriteLine("[1] Check in a vehicle.\n[2] Checkout a vehacle.\n[3] Move a vehacle\n[4] Search for a vehacle\n[5] View parkinglot.");
            Console.WriteLine("[6] Move MCs to fill gaps.");
            Console.WriteLine("[7] Move cars to fill gaps.");
            string inOrOut = userInput.AskForNumber(1, 7).ToString();

            switch (inOrOut)
            {
                case "1":
                    organizer.UserAddVehicle();
                    MainMenu();
                    break;
                case "2":
                    organizer.UserRemovesVehicle();
                    MainMenu();
                    break;
                case "3":
                    organizer.UserMoveVehicle();
                    MainMenu();
                    break;
                case "4":
                    //todo: create user interface.
                    SearchForVehicle();
                    MainMenu();
                    break;
                case "5":
                    vizulize.PresentRows();
                    MainMenu();
                    break;
                case "6":
                    organizer.UserOptimizeMcSpace();
                    
                    MainMenu();
                    break;
                case "7":
                    organizer.UserOptimizeSpace();
                    MainMenu();
                    organizer.SaveToFile();
                    break;
            }
        }

        private void SearchForVehicle()
        {
            string regPlate = userInput.AskForReg();
            if (organizer.SearchForReg(regPlate) == -1)
            {
                Console.WriteLine("Couldn't find that one.");
                GoToMenu();
                return;
            }
            Vehicle vehicle = organizer.SearchForVehicle(regPlate);
            
            VizulizeParkinglot.PrintVehicleInfo(vehicle);
            GoToMenu();
        }


        public void GoToMenu()
            {
                Console.WriteLine("press enter to continue to menu.");
                Console.ReadLine();
            Console.Clear();
        }

        public void PrintNotFound()
        {
            Console.WriteLine("Sorry, can't be found.");
            GoToMenu();
        }
        public void PrintFoundAt(int regNumIndex)
        {
            string rowResponce = GetParkingSpaceString(regNumIndex);
            Console.WriteLine();

            Console.WriteLine(String.Format("Can be found at {0}.", rowResponce));
            GoToMenu();
        }



        public string GetParkingSpaceString(int index)
        {
            return String.Format("number {0}", index + 1);
        }

        public void AddSpace()
        {
            Console.WriteLine("\n\n");
        }
    }
}