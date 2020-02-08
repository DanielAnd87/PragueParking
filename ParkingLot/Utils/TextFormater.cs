using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot.Utils
{
    static internal class TextFormater
    {
        internal static string VehicleSearchResult(int id, string vehicleType, int location, string regNum)
        {

            string formatedType = vehicleType == "Car" ? "bil" : "Mc";
            string formatedMessage = String.Format($"En {formatedType} med reg-nummer {regNum} och id {id} ska stå på plats {location}.");
            return formatedMessage;
        }

        internal static string VehicleReciptString(int id, string vehicleType, int location, string regNum, DateTime startTime)
        {
            string formatedType = vehicleType == "Car" ? "Bil" : "Mc";
            string formatedMessage = String.Format($"Type: {formatedType} \nReg-nummer: {regNum} \nid: {id} \nplats {location} \nstart time: {startTime.ToString()} \nout time: {DateTime.Now.ToString()}");
            return formatedMessage;
        }
    }
}
