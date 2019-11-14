using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ParkingLot
{
    public class DbHandler
    {
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
    }
}