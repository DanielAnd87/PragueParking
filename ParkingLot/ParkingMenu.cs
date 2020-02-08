using MenuLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot
{
    public class ParkingMenu
    {
        private const string OrderMessage = "Tryck på en knapp när ordern är uppskriven och flytta sedan fordonen.";

        public void Menu()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            bool running = true;
            while (running)
            {
                int choice = MenuUtils.AlternetivesMenu(0,
                                                        new string[] { "Checka in ett fordon", "Checka ut ett fordon", "Checka ut utan kostnad", "Flytta ett fordon", "Sök efter ett fordon", "Se överblicks-karta", "Organisera fordonsplats", "Optimesera alla MC platser", "Visa intäckts-rapport", "Sökbar intäckts-rapport", "Visa fordon som parkerat mer än två dygn", "Avsluta programmet", "Checka ut från lista.", "Lista över parkerade fordon." },
                                                        "Välkommen till PragueParking!");
                int checkIn = 0, checkOut = 1, checkOutFree = 2, moveVehicle = 3, search = 4, viewParkinglot = 5; 
                int organizeSpace = 6, optimizeMcSpace = 7, viewEarnings = 8, searchForEarnings = 9, viewLongParkedVehicles = 10, exit = 11, checkOutFromList = 12, listAllVehicles = 13;
                if (choice == checkIn)
                {
                    CheckIn();
                }
                else if (choice == checkOut)
                {
                    CheckOut();
                }
                else if (choice == checkOutFree)
                {
                    CheckOutFree();
                }
                else if (choice == moveVehicle)
                {
                    MoveVehicle();
                }
                else if (choice == search)
                {
                    SearchForVehicle();
                }
                else if (choice == viewParkinglot)
                {
                    ViewParkingLot();
                }
                else if (choice == organizeSpace)
                {
                    OrganizeSpace();
                }
                else if (choice == optimizeMcSpace)
                {
                    OptimizeMcSpace();
                }
                else if (choice == viewEarnings)
                {
                    ViewEarnings();
                }
                else if (choice == searchForEarnings)
                {
                    SearchForEarnings();
                }
                else if (choice == viewLongParkedVehicles)
                {
                    ViewLongParkedVehicles();
                }
                else if (choice == exit)
                {
                    running = false;
                }
                else if (choice == checkOutFromList)
                {
                    CheckOutFromList();
                }
                else if (choice == listAllVehicles)
                {
                    ListAllVehicles();
                }
            }
        }

        private void ListAllVehicles()
        {
            DbHandler handler = new DbHandler();
            List<Vehicle> vehicles = handler.GetAllVehicles();
            string[] regChoices = new string[vehicles.Count];
            for (int i = 0; i < vehicles.Count; i++)
            {
                regChoices[i] = String.Format($"Regnum: {vehicles[i].RegNum} In time: {vehicles[i].StartTime.ToString()} Parkeringsnummer: {vehicles[i].ParkingNum}");
            }
            foreach (string row in regChoices)
            {
                Console.WriteLine(row);

            }
            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");

        }

        private void CheckOutFromList()
        {

            DbHandler handler = new DbHandler();
            List<Vehicle> vehicles = handler.GetAllVehicles();
            string[] regChoices = new string[vehicles.Count];
            for (int i = 0; i < vehicles.Count; i++)
            {
                regChoices[i] = String.Format($"Regnum: {vehicles[i].RegNum} In time: {vehicles[i].StartTime.ToString()} Parkeringsnummer: {vehicles[i].ParkingNum}");
            }

            int choice = MenuUtils.AlternetivesMenu(0, regChoices, "Choose a vehicle to check out.");

            Console.Clear();
            CheckOut(handler, vehicles[choice].RegNum);


            //MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }

        private void ViewLongParkedVehicles()
        {
            DbHandler handler = new DbHandler();
            string[] parkedVehiclesReport = handler.FetchLongParkedVehicles();
            Console.Clear();
            foreach (string row in parkedVehiclesReport)
            {
                Console.WriteLine(row);
            }

            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }

        private void SearchForEarnings()
        {
            DbHandler handler = new DbHandler();
            DateTime startDate = MenuUtils.AskForDate("Startdatum:");
            DateTime endDate = MenuUtils.AskForDate("Slutdatum");
            //todo: make typesafe!
            string[] earninsReport = handler.FetchEarnings(startDate,endDate);
            Console.Clear();
            
            foreach (string row in earninsReport)
            {
                Console.WriteLine(row + " kronor.");

            }
            IncomeSum(startDate, endDate);

            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }


        private static void IncomeSum(DateTime startDate, DateTime endDate)
        {
            DbHandler database = new DbHandler();

            decimal[] earninsReport = database.FetchEarningsSum(startDate, endDate);
            foreach (decimal row in earninsReport)
            {
                string income = row.ToString();
                income = income.Substring(0, income.IndexOf(',') + 3);
                Console.WriteLine("\nSumman av perioden: "+income + " kronor.");

            }
        }

        private void ViewEarnings()
        {
            DbHandler handler = new DbHandler();
            string[] earninsReport = handler.FetchEarnings();
            Console.Clear();
            foreach (string row in earninsReport)
            {
                Console.WriteLine(row + " kronor.");

            }

            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }

        private void OptimizeMcSpace()
        {
            DbHandler handler = new DbHandler();
            string organizeOrder = handler.OrganizeMcSpace();
            Console.Clear();
            string[] orders = organizeOrder.Split('#');
            foreach (string row in orders)
            {
                Console.WriteLine(row);
                Console.WriteLine();
            }
            MenuUtils.PauseUntilFeedback(OrderMessage);
            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }

        private void OrganizeSpace()
        {
            DbHandler handler = new DbHandler();
            string organizeOrder = handler.OrganizeSpace();
            Console.Clear();

            Console.WriteLine(organizeOrder);
            MenuUtils.PauseUntilFeedback(OrderMessage);
        }

        private void CheckOutFree()
        {
            DbHandler handler = new DbHandler();
            string regNumber = MenuUtils.AskForStringWithoutSpecialChar("Vänligen skriv in reg-numret för fordonet som ska checkas ut.").ToUpper();
            string checkOutOrder = handler.CheckOutVehicleForFree(regNumber);
            Console.Clear();
            Console.WriteLine(checkOutOrder);

            MenuUtils.PauseUntilFeedback(OrderMessage);

        }
        private static void ViewParkingLot()
        {
            DbHandler handler = new DbHandler();
            Console.Clear();
            handler.ViewParkingLot();
            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
            
        }

        private static void SearchForVehicle()
        {

            DbHandler handler = new DbHandler();
            string regNum = MenuUtils.AskForStringWithoutSpecialChar("Skriv in fordonets reg-nummer").ToUpper();
            string searchMessage = handler.FetchVehicleInfo(regNum);
            Console.Clear();
            Console.WriteLine(searchMessage);

            MenuUtils.PauseUntilFeedback("Tryck på en knapp för att återvända till menyn");
        }

        private static void MoveVehicle()
        {
            DbHandler handler = new DbHandler();
            string regNumber = MenuUtils.AskForStringWithoutSpecialChar("Vänligen skriv in reg-numret för fordonet som ska flyttas.").ToUpper();
            string[] freeParkingSpaces = handler.FetchFreeParkingSpaces();
            int parkingspaceChoice = MenuUtils.AlternetivesMenu(0, freeParkingSpaces, "Välj en plats att flytta fordonet");
            string choosenSpot = freeParkingSpaces[parkingspaceChoice];
            int spaceToMoveFrom = handler.FetchVehicleSpot(regNumber);
            int spaceToMoveTo = Convert.ToInt32(choosenSpot);

            bool result = handler.MoveVehicle(regNumber, spaceToMoveTo);
            Console.Clear();
            if (result)
            {
                Console.WriteLine("Flytten lyckades!");
                Console.WriteLine($"Flytta fordonet med reg-nummer {regNumber} från plats {spaceToMoveFrom} till plats {spaceToMoveFrom}");
                MenuUtils.PauseUntilFeedback(OrderMessage);
            }
            else
            {
                Console.WriteLine("Något gick fel!");
                MenuUtils.PauseUntilFeedback("Tryck på en knapp för att fortsätta");
            }


        }

        private static void CheckOut()
        {

            DbHandler handler = new DbHandler();
            string regNumber = MenuUtils.AskForStringWithoutSpecialChar("Vänligen skriv in reg-numret för fordonet som ska checkas ut.").ToUpper();
            CheckOut(handler, regNumber);

        }

        private static void CheckOut(DbHandler handler, string regNumber)
        {
            string checkOutOrder = handler.CheckOutVehicle(regNumber);
            Console.Clear();
            //Console.WriteLine(checkOutOrder);

            MenuUtils.PauseUntilFeedback(checkOutOrder);
        }

        private static void CheckIn()
        {
            DbHandler handler = new DbHandler();
            int carType = MenuUtils.AlternetivesMenu(0, new string[] { "Bil", "Mc"}, "Vilken typ av fordon är det?") +1;
            string regNumber = MenuUtils.AskForStringWithoutSpecialChar("Vänligen skriv in reg-numret för fordonet som ska checkas in.").ToUpper();
            string checkOutOrder = handler.CheckInVehicle(regNumber, carType);
            Console.Clear();
            Console.WriteLine(checkOutOrder);

            MenuUtils.PauseUntilFeedback(OrderMessage);
        }


    }
}
