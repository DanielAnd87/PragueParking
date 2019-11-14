using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLot
{
    public interface IUserInput
    {
        string AskForReg();
        int AskForNumber(int min, int max);
        int AskForOptimizationChoice(int[,] choices, List<Vehicle> vehicles);

        bool AskForType();
        bool OrderUserToMoveCars(List<int> optimizeChoises, string regPlate);
        bool DriveOrder(Vehicle vehicle);
        bool MoveOrder(Vehicle vehicle, int from);

        bool PrintNothingtoSeeHere();
        bool BackToMenu();
        bool CheckOutVehicle(Vehicle vehicle);
        void PrintCheckOut(Vehicle vehicle);
    }
}
