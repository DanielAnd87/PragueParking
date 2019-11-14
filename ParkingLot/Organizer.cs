using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace ParkingLot
{
    public class Organizer
    {
        private const string NOT_FOUND = "not found";
        public List<Vehicle> vehicles = new List<Vehicle>();
        public IUserInput userInput;

        public Organizer(IUserInput userInput)
        {
            this.userInput = userInput;
        }

        public int SearchForReg(string regNum)
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                Vehicle vehicle = vehicles[i];
                if (vehicle.RegNum == regNum)
                {
                    return i;
                }
            }

            return -1;
        }
        public Vehicle SearchForVehicle(string regNum)
        {
            for (int i = 0; i < vehicles.Count; i++)
            {
                Vehicle vehicle = vehicles[i];
                if (vehicle.RegNum == regNum)
                {
                    return vehicle;
                }
            }

            return new Vehicle(NOT_FOUND,false);
        }

        public bool UserAddVehicle()
        {
            string regNum = userInput.AskForReg();
            if (SearchForReg(regNum) != -1)
            {
                // Go to menu.
                Console.WriteLine("That vehicle has already checked in.");
                userInput.BackToMenu();
                return false;
            }
            bool isCar = userInput.AskForType();
            Vehicle vehicle = new Vehicle(regNum, isCar);
            vehicle = InsertVehicle(vehicle);
            // Om fordonet ej blivit tilldelad en ruta.
            userInput.DriveOrder(vehicle);
            DbHandler.Save(vehicles);
            return vehicle.ParkingNum != -1;
        }

        private Vehicle InsertVehicle(Vehicle vehicle)
        {
            // behöver lägga till fordonet på ledig plats
            // ge objektet rätt platsnummer och ifall den är uppfyld.
            for (int i = 0; i < 100; i++)
            {
                bool vacant = true;
                for (int j = 0; j < vehicles.Count; j++)
                {
                    if (vehicles[j].ParkingNum == i)
                    {
                        // todo: if isCar then check if fillWholeSpace is true.
                        vacant = false;
                        if (!vehicle.IsCar && !vehicles[j].IsCar && !vehicles[j].FillsWholeSpace)
                        {
                            vehicles[j].FillsWholeSpace = true;
                            vehicle.FillsWholeSpace = true;
                            vehicle.ParkingNum = i;
                            vehicles.Add(vehicle);
                            return vehicle;
                        }
                    }
                }

                if (vacant)
                {
                    vehicle.ParkingNum = i;
                    // Cars always fills whole parkingspaces.
                    vehicle.FillsWholeSpace = vehicle.IsCar;
                    vehicles.Add(vehicle);
                    break;
                }


            }

            return vehicle;

        }

        public bool UserRemovesVehicle()
        {
            string regNum = userInput.AskForReg();

            Vehicle vehicle = SearchForVehicle(regNum);
            if (vehicle.IsCar)
            {
                userInput.CheckOutVehicle(vehicle);
                vehicles.Remove(vehicle);
                DbHandler.Save(vehicles);

                return true;
            }
            else
            {
                foreach (Vehicle vehicle1 in vehicles)
                {
                    if (vehicle1.ParkingNum == vehicle.ParkingNum)
                    {
                        userInput.CheckOutVehicle(vehicle);
                        vehicle1.FillsWholeSpace = false;
                        vehicles.Remove(vehicle);
                        DbHandler.Save(vehicles);

                        return true;
                    }
                }
            }


            DbHandler.Save(vehicles);

            return false;
        }

        public bool UserMoveVehicle()
        {
            string regNum = userInput.AskForReg();
            // todo: Need to tell user why it failed.
            int place = SearchForVehicle(regNum).ParkingNum;

            if (SearchForReg(regNum) == -1) return false;
            int moveToIndex = userInput.AskForNumber(1,100);
            moveToIndex--;
            // todo: Need to tell user why it failed.
            bool success = MoveTo(regNum, moveToIndex);
            if (success)
            {
                userInput.MoveOrder(SearchForVehicle(regNum) ,place);
                DbHandler.Save(vehicles);

                return true;
            }
            else
            {
                Console.WriteLine("Couldn't move to there.");
                userInput.BackToMenu();
                DbHandler.Save(vehicles);

                return false;
            }
        }

        private bool MoveTo(string regNum, int moveToIndex)
        {
            Vehicle vehicle = SearchForVehicle(regNum);
            if (!IsVacant(moveToIndex, vehicle.IsCar)) return false;

            int lastParkingNum = vehicle.ParkingNum;

            if (!IsVacant(moveToIndex, true))
            {
                vehicle.FillsWholeSpace = true;
            }
            else
            {
                vehicle.FillsWholeSpace = false;
            }

            vehicle.ParkingNum = moveToIndex;

            if (!vehicle.IsCar)
            {
                foreach (Vehicle vehicle1 in vehicles)
                {
                    if (vehicle1.ParkingNum == lastParkingNum)
                    {
                        vehicle1.FillsWholeSpace = false;
                        break;
                    }
                }
            }

            return true;
        }

        private bool IsVacant(int moveToIndex, bool vehicleIsCar)
        {
            foreach (Vehicle vehicle1 in vehicles)
            {
                if (vehicle1.ParkingNum == moveToIndex)
                {
                    // om fordonet är en bil är plattsen alltid full.
                    if (vehicleIsCar) return false;
                    // om fordonet är en Mc kollar vi om all plats fylls.
                    if (vehicle1.FillsWholeSpace) return false;
                    return true;
                }
            }

            return true;
        }

        public bool UserOptimizeMcSpace()
        {
            // find all optimize opertunities and present in an array.
            List<int> optimizeChoises = FindOptimizeMcChoises();
            int numChoises = optimizeChoises.Count;
            if (optimizeChoises.Count % 2 == 1)
            {
                numChoises = (optimizeChoises.Count - 1) / 2;
            }
            else
            {
                numChoises = optimizeChoises.Count / 2;
            }

            int[,] choices = new int[numChoises,2];

            for (int i = 0; i < numChoises; i += 2)
            {
                choices[i, 0] = optimizeChoises[0];
                choices[i, 1] = optimizeChoises[i*2+1];
            }

            if (choices.GetLength(0) == 0)
            {
                userInput.PrintNothingtoSeeHere();
                return false;
            }
            //lets user make a choice of alternetives.
            int userChoice = userInput.AskForOptimizationChoice(choices, vehicles);

            int fromNum = vehicles[choices[userChoice, 1]].ParkingNum;
            int toNum = vehicles[choices[userChoice,0]].ParkingNum;
            Console.WriteLine("Move vehicle {0} from {1} to {2}", vehicles[choices[userChoice, 0]].RegNum, fromNum, toNum);
            //Console.WriteLine("Press enter to continue.");
            //Console.ReadLine();
            userInput.BackToMenu();

            vehicles[choices[userChoice, 1]].ParkingNum = toNum;
            vehicles[choices[userChoice, 0]].FillsWholeSpace = true;
            vehicles[choices[userChoice, 1]].FillsWholeSpace = true;

            // todo: Let user know where to drive cars.
            DbHandler.Save(vehicles);
            return true;
        }


        public bool UserOptimizeSpace()
        {
            List<int> optimizeChoises = FindOptimizeChoice();
            if (optimizeChoises.Count == 0)
            {
                userInput.PrintNothingtoSeeHere();
                return false;
            }


            int optimizeChoise = optimizeChoises[2];
            string regPlate = vehicles[optimizeChoise].RegNum;
            //Låter först användaren godkänna ändringen.
            if (userInput.OrderUserToMoveCars(optimizeChoises, regPlate))
            {
                DbHandler.Save(vehicles);
                return MoveTo(regPlate, optimizeChoises[0]);
            }
            return false;
        }

        private List<int> FindOptimizeMcChoises()
        {
            List<int> choiseList = new List<int>();
            vehicles = vehicles.OrderBy(o => o.ParkingNum).ToList();
            for (int i = 0; i < vehicles.Count; i++)
            {
                if (IsVacant(vehicles[i].ParkingNum, false))
                {
                    choiseList.Add(i);
                }
            }

            return choiseList;
        }
        private List<int> FindOptimizeChoice()
        {
            List<int> choiceList = new List<int>();
            vehicles = vehicles.OrderBy(o => o.ParkingNum).ToList();
            int last = 0;
            for (int i = 0; i < vehicles.Count; i++)
            {
                int num = vehicles[i].ParkingNum;

                if (i == 0)
                {
                    if (num != 0)
                    {

                        choiceList.Add(0);
                        choiceList.Insert(choiceList.Count, vehicles[vehicles.Count - 1].ParkingNum);
                        choiceList.Insert(choiceList.Count, vehicles.Count - 1);
                        return choiceList;
                    }
                    last = num;
                }
                else
                {
                    if (num - last > 1)
                    {
                        choiceList.Add(last+1);
                        choiceList.Insert(choiceList.Count, vehicles[vehicles.Count-1].ParkingNum);
                        choiceList.Insert(choiceList.Count, vehicles.Count-1);
                        return choiceList;
                    }
                }
                last = num;
            }

            return choiceList;
        }

        public bool SaveToFile()
        {
            return DbHandler.Save(vehicles);
        }

        public bool RestoreFromFile()
        {
            vehicles = DbHandler.Restore();
            return vehicles.Count > 0 ? true : false;

        }
    }
}