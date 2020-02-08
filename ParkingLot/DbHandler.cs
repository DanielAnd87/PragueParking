using ParkingLot.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ParkingLot
{
    public class DbHandler
    {
        private const string CONNECTION_STRING = "Server=S;Database=ParkingLot;persist security info=True; Integrated Security = SSPI; ";

        private const string FILE_DIRECTORY = @"/PARKING_DATA";
        private enum POS
        {
            PARKING_NUM, FILL_SPACE, IS_CAR, REG, DATE
        }
        public static bool Save(List<Vehicle> vehicles)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Directory.GetCurrentDirectory());
            builder.Append(FILE_DIRECTORY);
            // Rensar filen innan jag sparar datan.
            // Manualen säger att nedanstående stänger sig själv.
            File.WriteAllText(builder.ToString(), string.Empty);

            using (StreamWriter writer = new StreamWriter(builder.ToString()))
            {
                foreach (Vehicle vehicle in vehicles)
                {
                    //todo: Make it possible to charges for several weeks.
                    
                    string data =
                        $"{vehicle.ParkingNum}#{vehicle.FillsWholeSpace}#{vehicle.IsCar}#{vehicle.RegNum}#{vehicle.StartTime.ToString()}";
                    writer.WriteLine(data);
                }
            }
            // Raden under är praktisk när jag debuggar, därför behåller jag den som kommentar.
            //Process.Start("notepad.exe", builder.ToString());
            return true;
        }

        public static List<Vehicle> Restore()
        {
            List<Vehicle> vehicles = new List<Vehicle>();
            // todo: cars and bikes can share the same space. fixme!
            // todo: verkar vara optimize metoderna som strular.
            StringBuilder builder = new StringBuilder();
            builder.Append(Directory.GetCurrentDirectory());
            builder.Append(FILE_DIRECTORY);
            try
            {

                using (StreamReader reader = new StreamReader(builder.ToString()))
                {
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();
                        string[] objectData = line.Split('#');
                        bool fillWhole = objectData[(int)POS.FILL_SPACE] == "True" ? true : false;
                        bool isCar = objectData[(int)POS.IS_CAR] == "True" ? true : false;
                        string regNum = objectData[(int)POS.REG];
                        int parkingNum = (int)Convert.ToDouble(objectData[(int)POS.PARKING_NUM]);

                        string[] timeArray = objectData[(int)POS.DATE].Split(new char[] { ' ', '-', ':' });
                        DateTime temp = new DateTime(
                            Convert.ToInt32(timeArray[0]),
                            Convert.ToInt32(timeArray[1]),
                            Convert.ToInt32(timeArray[2]),
                            Convert.ToInt32(timeArray[3]),
                            Convert.ToInt32(timeArray[4]),
                            Convert.ToInt32(timeArray[5]));

                        vehicles.Add(new Vehicle(regNum, isCar, fillWhole, temp, parkingNum));

                    }

                }

            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("No previus data found.");
            }

            return vehicles;
        }

        public List<Vehicle> GetAllVehicles() {
            List<Vehicle> searchReport = new List<Vehicle>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("SELECT v.RegNum, v.VehicleTypeId, ph.Available, v.InTime, ph.ParkLocation FROM Vehicles v INNER JOIN VehicleTypes vh ON vh.Id = V.VehicleTypeId INNER JOIN ParkingSpace ph ON v.ParkingSpaceId = ph.Id; ", connection);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    //DateTime date = (DateTime)searchResult[0];
                    //string timeIntervalString = (string)searchResult[0].ToString();
                    //string regnum = searchResult[1].ToString();


                    int parkingNum = (int)searchResult[4];
                    DateTime dateTime = (DateTime)searchResult[3];
                    int isFull = (int)searchResult[2];
                    bool fillWholeSpace =(int) isFull == 0 ? true : false;
                    bool isCar = (int)searchResult[1] != 2 ? true : false;
                    string regNum = searchResult[0].ToString();
                    searchReport.Add(new Vehicle(
                        regNum,
                        isCar,
                        fillWholeSpace,
                        dateTime,
                        parkingNum-1
                        ));


                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }

            return searchReport;

        }

        public void ViewParkingLot() {
            VizulizeParkinglot vizulize = new VizulizeParkinglot(GetAllVehicles());
            vizulize.PrintParkinglot();
        }

        internal string[] FetchLongParkedVehicles()
        {

            List<string> searchReport = new List<string>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC [TwoDaysPlusReport];", connection);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    //DateTime date = (DateTime)searchResult[0];
                    string timeIntervalString = (string)searchResult[0].ToString();
                    string regnum = searchResult[1].ToString();


                    searchReport.Add(String.Format("{0},    {1}",
                        timeIntervalString,
                        regnum));

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }


            return searchReport.ToArray();
        }

        internal string[] FetchEarnings(DateTime startDate, DateTime endDate)
        {
            List<string> searchReport = new List<string>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC IncomeIntervalReport @StartDate = @startdate, @endDate = @enddate;", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@startdate";
                param.Value = startDate;
                command.Parameters.Add(param);
                SqlParameter endDateParam = new SqlParameter();
                endDateParam.ParameterName = "@enddate";
                endDateParam.Value = endDate;
                command.Parameters.Add(endDateParam);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    //DateTime date = (DateTime)searchResult[0];
                    string dateString = (string)searchResult[0];
                    string income = searchResult[1].ToString();


                    searchReport.Add(String.Format("{0},    {1}",
                        dateString,
                        income));

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }

            return searchReport.ToArray();
        }
        internal decimal[] FetchEarningsSum(DateTime startDate, DateTime endDate)
        {
            List<decimal> searchReport = new List<decimal>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC [IncomeScalarIntervalReport] @StartDate = @startdate, @endDate = @enddate;", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@startdate";
                param.Value = startDate;
                command.Parameters.Add(param);
                SqlParameter endDateParam = new SqlParameter();
                endDateParam.ParameterName = "@enddate";
                endDateParam.Value = endDate;
                command.Parameters.Add(endDateParam);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    //DateTime date = (DateTime)searchResult[0];
                    decimal income = (decimal)searchResult[0];



                    searchReport.Add(income);

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }

            return searchReport.ToArray();
        }
        internal string[] FetchEarnings()
        {

            List<string> searchReport = new List<string>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC [DailyIncomeReport];", connection);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    //DateTime date = (DateTime)searchResult[0];
                    string dateString = (string) searchResult[0];
                    string income = searchResult[1].ToString();
                    income = income.Substring(0, income.IndexOf(',')+3);


                    searchReport.Add(String.Format("{0},    {1}",
                        dateString,
                        income));

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }

            return searchReport.ToArray();
        }

        internal string OrganizeMcSpace()
        {
            string report = "";

            List<string> searchReport = new List<string>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC OptimizeAllMotorBikes", connection);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    searchReport.Add((string)searchResult[0].ToString()); 
                    report = report + (string)searchResult[0].ToString();

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }

            }

            return report;
        }


            internal string OrganizeSpace()
        {
            throw new NotImplementedException();
        }

        private float CalcPrice(string regNum)
        {
            float cost = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                Console.WriteLine("Search result: ");
                SqlCommand dateCommand = new SqlCommand("SELECT v.InTime, v.VehicleTypeId  FROM Vehicles V INNER JOIN VehicleTypes vh ON vh.Id = V.VehicleTypeId WHERE V.RegNum = @regNum; ", connection);
                SqlParameter priceAndTypeParam = new SqlParameter();
                priceAndTypeParam.ParameterName = "@regNum";
                priceAndTypeParam.Value = regNum;
                dateCommand.Parameters.Add(priceAndTypeParam);
                SqlDataReader searchResult = dateCommand.ExecuteReader();
                DateTime date = new DateTime();
                bool isCar = false;
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    date = (DateTime)searchResult[0];
                    isCar =(int) searchResult[1] == 1 ? true : false;
                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
                cost = CalculatePrice.GetCost(date, isCar);
                
            }

            return cost;
        }

        internal string CheckOutVehicle(string regNum)
        {
            float cost = CalcPrice(regNum);
            string reciept = FetchVehicleReciptInfo(regNum);
            reciept = reciept + $" \n \nPris: {cost} kronor \n\nFordonet är utckeckat från systemet.";

            bool result = CheckOutVehicle(regNum, cost);

            if (result)
            {
                return reciept;
            }
            else
            {
                return "Något gick fel";

            }
        }
        
        private static bool CheckOutVehicle(string regNum, float cost)
        {
            int searchReport = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("EXEC [CheckoutAndChargeVehicleRegnum] @RegNum = @regNum, @PriceToPay = @price;", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                command.Parameters.Add(param);
                SqlParameter priceParam = new SqlParameter();
                priceParam.ParameterName = "@price";
                priceParam.Value = cost;
                command.Parameters.Add(priceParam);
                command.CommandTimeout = 5;
                searchReport = command.ExecuteNonQuery();
               
            }

            return searchReport > 0 ? true : false;
        }

        internal string CheckOutVehicleForFree(string regNum)
        {
            string reciept = FetchVehicleReciptInfo(regNum);
            
            bool result = CheckOutVehicle(regNum, 0);

            if (result)
            {
                return reciept + " \n \nPris: 0";
            }
            else
            {
                return "Något gick fel";

            }
        }

        internal string CheckInVehicle(string regNum, int carType)
        {
            string searchReport = "Något gick fel!";
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("EXEC Insert_Vehicle @regNum = @regNum, @carTypeId = @TYPE;", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                SqlParameter toSpaceParam = new SqlParameter();
                toSpaceParam.ParameterName = "@TYPE";
                toSpaceParam.Value = carType;


                command.Parameters.Add(param);
                command.Parameters.Add(toSpaceParam);

                try
                {
                    rowsAffected = command.ExecuteNonQuery();

                }
                catch (SqlException e)
                {
                    switch (e.Number)
                    {
                        case 2627:
                            searchReport = "Det regnummret finns redan i systemet";

                            break;
                        default:
                            throw;
                    }
                }

            }
            if (rowsAffected > 0)
            {
                return FetchVehicleInfo(regNum);
            }
            else
            {
                return searchReport;
            }
        }

        internal string[] FetchFreeParkingSpaces()
        {
            List<string> searchReport = new List<string>();
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {


                connection.Open();

                SqlCommand command = new SqlCommand("SELECT ph.ParkLocation FROM ParkingSpace PH WHERE PH.Available = PH.Size;", connection);


                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                    //                                                (string)searchResult[1],
                    //                                                (int)searchResult[2],
                    //                                                regNum);
                    searchReport.Add((string) searchResult[0].ToString());

                    //Console.WriteLine("{0}",
                    //    searchResult[0]);
                    //                        searchResult[1]);
                }
            }

            return searchReport.ToArray();
        }

        internal bool MoveVehicle(string regNum, int spaceToMove)
        {
            int searchReport = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                

                connection.Open();

                SqlCommand command = new SqlCommand("EXEC MoveVehicleRegnum @RegNum = @regNum, @toSpace = @soSpace ", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                SqlParameter toSpaceParam = new SqlParameter();
                toSpaceParam.ParameterName = "@soSpace";
                toSpaceParam.Value = spaceToMove;
                
                
                command.Parameters.Add(param);
                command.Parameters.Add(toSpaceParam);

                
                searchReport = command.ExecuteNonQuery();
                //while (searchResult.Read())
                //{
                //    //searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                //    //                                                (string)searchResult[1],
                //    //                                                (int)searchResult[2],
                //    //                                                regNum);
                //    searchReport = searchResult[0].ToString();
                    
                //}
            }

            return searchReport > 0 ? true : false;
        }

        internal int FetchVehicleSpot(string regNum)
        {
            int searchReport = -1;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                
                Console.WriteLine("Search result: ");
                SqlCommand command = new SqlCommand("SELECT ph.ParkLocation FROM Vehicles V INNER JOIN VehicleTypes vh ON vh.Id = V.VehicleTypeId INNER JOIN ParkingSpace ph ON v.ParkingSpaceId = ph.Id WHERE RegNum = @regNum; ", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                command.Parameters.Add(param);
                command.CommandTimeout = 5;
                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    searchReport = (int)searchResult[0];
                                                                    

                    //Console.WriteLine("id: {0}, typ: {1} plats: {2}",
                    //    searchResult[0],
                    //    searchResult[1]);
                }
                //int location = (int)searchResult[2];
                //string type = (string)searchResult[1];
                //int id =(int) searchResult[0];
                //searchReport = string.Format($"id: {id} typ: {type} {location} regnummer: {regNum}");

            }
            return searchReport;
    }
        internal string FetchVehicleInfo(string regNum)
        {
            string searchReport = "Hittade ingen fordon med det reg-numret";
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                
                Console.WriteLine("Search result: ");
                SqlCommand command = new SqlCommand("SELECT v.Id, vh.Description, ph.ParkLocation FROM Vehicles V INNER JOIN VehicleTypes vh ON vh.Id = V.VehicleTypeId INNER JOIN ParkingSpace ph ON v.ParkingSpaceId = ph.Id WHERE RegNum = @regNum; ", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                command.Parameters.Add(param);
                command.CommandTimeout = 5;
                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    searchReport = TextFormater.VehicleSearchResult((int)searchResult[0],
                                                                    (string)searchResult[1],
                                                                    (int)searchResult[2],
                                                                    regNum);

                    //Console.WriteLine("id: {0}, typ: {1} plats: {2}",
                    //    searchResult[0],
                    //    searchResult[1]);
                }
                //int location = (int)searchResult[2];
                //string type = (string)searchResult[1];
                //int id =(int) searchResult[0];
                //searchReport = string.Format($"id: {id} typ: {type} {location} regnummer: {regNum}");

            }
            return searchReport;
    }

        private static string FetchVehicleReciptInfo(string regNum)
        {
            string searchReport = "Hittade ingen fordon med det reg-numret";
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                Console.WriteLine("Search result: ");
                SqlCommand command = new SqlCommand("SELECT v.Id, vh.Description, ph.ParkLocation, v.InTime FROM Vehicles V INNER JOIN VehicleTypes vh ON vh.Id = V.VehicleTypeId INNER JOIN ParkingSpace ph ON v.ParkingSpaceId = ph.Id WHERE RegNum = @regNum; ", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@regNum";
                param.Value = regNum;
                command.Parameters.Add(param);
                command.CommandTimeout = 5;
                SqlDataReader searchResult = command.ExecuteReader();
                while (searchResult.Read())
                {
                    searchReport = TextFormater.VehicleReciptString((int)searchResult[0],
                                                                    (string)searchResult[1],
                                                                    (int)searchResult[2],
                                                                    regNum,
                                                                    (DateTime)searchResult[3]);

                    //Console.WriteLine("id: {0}, typ: {1} plats: {2}",
                    //    searchResult[0],
                    //    searchResult[1]);
                }
                //int location = (int)searchResult[2];
                //string type = (string)searchResult[1];
                //int id =(int) searchResult[0];
                //searchReport = string.Format($"id: {id} typ: {type} {location} regnummer: {regNum}");

            }

            return searchReport;
        }
        }
        }
    