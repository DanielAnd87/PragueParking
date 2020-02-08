using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot
{
    class TestDataGenerator
    {
        private const string CONNECTION_STRING = "Server=S;Database=ParkingLot;persist security info=True; Integrated Security = SSPI; ";



        public void TestData()
        {
            DbHandler handler = new DbHandler();
            List<string> regnums = new List<string>();

            DateTime now = DateTime.Now;
            for (int i = 0; i < 50; i++)
            {


                string regNum = "QUE" + "-00" + i.ToString();
                int carType = (i % 2) + 1;
                handler.CheckInVehicle(regNum, carType);
                regnums.Add(regNum);
                DateTime inDate = now.AddHours(-i*10);
                Thread.Sleep(10);
                ChangeInDate(regNum, inDate);
                Thread.Sleep(10);
                
                    handler.CheckOutVehicle(regNum);
                Thread.Sleep(10);
                ChangeInDateHistory(regNum, inDate, inDate.AddHours(1), (decimal)CalculatePrice.GetCost(inDate, inDate.AddHours(1), carType == 1 ? true : false));
                Thread.Sleep(10);
            



        }

            //handler.CheckOutVehicle(regNum);
        }




        internal void CheckOutVehicle(string regNum)
        {
            DbHandler handler = new DbHandler();

            handler.CheckOutVehicleForFree(regNum);


        }




        private static bool ChangeInDate(string regNum, DateTime inDate)
        {
            int searchReport = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE Vehicles SET InTime = @inDate WHERE RegNum = @regNum; ", connection);

                SqlParameter regNumParam = new SqlParameter();
                regNumParam.ParameterName = "@regNum";
                regNumParam.Value = regNum;
                command.Parameters.Add(regNumParam);
                SqlParameter outDateParam = new SqlParameter();
                outDateParam.ParameterName = "@inDate";
                outDateParam.Value = inDate;
                command.Parameters.Add(outDateParam);
                
                searchReport = command.ExecuteNonQuery();

            }

            return searchReport > 0 ? true : false;
        }


    

        private static bool ChangeInDateHistory(string regNum, DateTime inDate, DateTime outDate, decimal price)
        {
            int searchReport = 0;
            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("UPDATE ParkingHistory SET PayedPrice = @price, OutTime = @outDate, InTime = @inDate WHERE RegNum = @regNum; ", connection);

                SqlParameter regNumParam = new SqlParameter();
                regNumParam.ParameterName = "@regNum";
                regNumParam.Value = regNum;
                command.Parameters.Add(regNumParam);
                SqlParameter outDateParam = new SqlParameter();
                outDateParam.ParameterName = "@outDate";
                outDateParam.Value = outDate;
                command.Parameters.Add(outDateParam);
                SqlParameter inDateParam = new SqlParameter();
                inDateParam.ParameterName = "@inDate";
                inDateParam.Value = inDate;
                command.Parameters.Add(inDateParam);
                SqlParameter priceParam = new SqlParameter();
                priceParam.ParameterName = "@price";
                priceParam.Value = price;
                command.Parameters.Add(priceParam);
                command.CommandTimeout = 5;
                searchReport = command.ExecuteNonQuery();

            }

            return searchReport > 0 ? true : false;
        }


    

        private static bool CheckOut(string regNum, float cost)
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


    }
}
