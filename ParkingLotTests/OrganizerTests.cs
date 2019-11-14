using Microsoft.VisualStudio.TestTools.UnitTesting;
using ParkingLot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using ParkingLot.Tests;
using System.Threading.Tasks;
using Moq;

namespace ParkingLot.Tests

{
    [TestClass()]
    public class OrganizerTests
    {

        [TestMethod()]
        public void NoSpacesLeftTest()
        {
            Mock<IUserInput> inputMock = new Mock<IUserInput>();

            Organizer organizer = new Organizer((IUserInput)inputMock.Object);
            UserInputStub userInput = new UserInputStub();
            inputMock.Setup(p => p.AskForType()).Returns(true);

            for (int i = 0; i < 100; i++)
            {
                inputMock.Setup(p => p.AskForReg()).Returns($"abcd{i.ToString()}");

                organizer.UserAddVehicle();
            }
            userInput.RegNum = $"abcdtest";
            inputMock.Setup(p => p.AskForReg()).Returns("abcdtest");

            Assert.AreEqual(false, organizer.UserAddVehicle());

        }


        [TestMethod()]
        public void UserAddCarTest()
        {
            Mock<IUserInput> inputMock = new Mock<IUserInput>();

            Organizer organizer = new Organizer((IUserInput)inputMock.Object);

            AddVehicle(organizer, true, "ABC123");

            Assert.AreEqual(true, organizer.vehicles[0].IsCar == true);
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "ABC123");
        }

        [TestMethod()]
        public void UserRemovesMcTest()
        {


            Organizer organizer = new Organizer((IUserInput)new Mock<IUserInput>().Object);
            // Adding two mc:s
            AddVehicle(organizer, false, "MC123");
            AddVehicle(organizer, false, "MC234");

            organizer.vehicles = new List<Vehicle>();

            organizer.RestoreFromFile();

            // Removing one mc
            int beforeRemove = organizer.vehicles.Count;
            RemovesVehicle(organizer, false, "MC234");
            int afterRemove = organizer.vehicles.Count;

            // checking for correct values
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
            Assert.AreEqual(false, organizer.vehicles[0].FillsWholeSpace == true);

            Assert.AreEqual(beforeRemove - 1, afterRemove);
        }
        [TestMethod()]
        public void UserAddTwoMcTest()
        {


            Organizer organizer = new Organizer(new UserInput());
            AddVehicle(organizer, false, "MC123");
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);

            AddVehicle(organizer, false, "MC234");

            Assert.AreEqual(true, organizer.vehicles[0].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[1].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[1].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
            Assert.AreEqual(true, organizer.vehicles[1].RegNum == "MC234");
        }
        [TestMethod()]
        public void UserAddTwoMcAndCarTest()
        {


            Organizer organizer = new Organizer(new UserInput());
            AddVehicle(organizer, false, "MC123");
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);

            AddVehicle(organizer, false, "MC234");



            organizer.vehicles = new List<Vehicle>();

            organizer.RestoreFromFile();

            // And a car
            AddVehicle(organizer, true, "ABC123");

            Assert.AreEqual(true, organizer.vehicles[2].IsCar == true);
            Assert.AreEqual(true, organizer.vehicles[2].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[2].RegNum == "ABC123");


            Assert.AreEqual(true, organizer.vehicles[0].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[1].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[1].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
            Assert.AreEqual(true, organizer.vehicles[1].RegNum == "MC234");
        }
        [TestMethod()]
        public void RestoreFromDbTest()
        {


            Organizer organizer = new Organizer(new UserInput());
            AddVehicle(organizer, false, "MC123");
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);

            AddVehicle(organizer, false, "MC234");




            // And a car
            AddVehicle(organizer, true, "ABC123");

            int expecHour = organizer.vehicles[1].StartTime.Hour;
            int expecMin = organizer.vehicles[1].StartTime.Minute;

            organizer.SaveToFile();
            organizer.vehicles = new List<Vehicle>();

            Assert.AreEqual(0, organizer.vehicles.Count());
            organizer.RestoreFromFile();

            Assert.AreEqual(true, organizer.vehicles[2].IsCar == true);
            Assert.AreEqual(true, organizer.vehicles[2].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[2].RegNum == "ABC123");


            Assert.AreEqual(true, organizer.vehicles[0].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[1].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[1].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
            Assert.AreEqual(true, organizer.vehicles[1].RegNum == "MC234");
            // todo: fixme: test don't work.'
            Assert.AreEqual(expecHour, organizer.vehicles[1].StartTime.Hour);
            Assert.AreEqual(expecMin, organizer.vehicles[1].StartTime.Minute);
        }
        
                [TestMethod()]
                public void UserMoveMcTest()
                {
                    Organizer organizer = new Organizer(new UserInput()); 
                    AddVehicle(organizer, false, "MC123");
                    Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);

                    AddVehicle(organizer, false, "MC234");



                    // And a car
                    AddVehicle(organizer, true, "ABC123");



                    organizer.vehicles = new List<Vehicle>();

                    organizer.RestoreFromFile();

                // Will move MC123
                    MoveVehicle(organizer, false, "MC123", 3);


                    Assert.AreEqual(true, organizer.vehicles[2].IsCar == true);
                    Assert.AreEqual(true, organizer.vehicles[2].FillsWholeSpace == true);
                    Assert.AreEqual(true, organizer.vehicles[2].RegNum == "ABC123");


                    Assert.AreEqual(true, organizer.vehicles[0].IsCar == false);
                    Assert.AreEqual(true, organizer.vehicles[1].IsCar == false);
                    Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);
                    Assert.AreEqual(true, organizer.vehicles[1].FillsWholeSpace == false);
                    Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
                    Assert.AreEqual(true, organizer.vehicles[1].RegNum == "MC234");
                    // Kollar ifall fordonen flyttades till rätt plattser.
                    Assert.AreEqual(3, organizer.vehicles[0].ParkingNum);
                    Assert.AreEqual(0, organizer.vehicles[1].ParkingNum);
                }

         
        [TestMethod()]
        public void UserOptimizeTest()
        {
            Organizer organizer = new Organizer(new UserInput());
            AddVehicle(organizer, false, "MC123");
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);

            AddVehicle(organizer, false, "MC234");



            // And a car
            AddVehicle(organizer, true, "ABC123");

            // Will move MC123
            MoveVehicle(organizer, false, "MC123", 3);


            Assert.AreEqual(true, organizer.vehicles[2].IsCar == true);
            Assert.AreEqual(true, organizer.vehicles[2].FillsWholeSpace == true);
            Assert.AreEqual(true, organizer.vehicles[2].RegNum == "ABC123");



            Assert.AreEqual(true, organizer.vehicles[0].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[1].IsCar == false);
            Assert.AreEqual(true, organizer.vehicles[0].FillsWholeSpace == false);
            Assert.AreEqual(true, organizer.vehicles[1].FillsWholeSpace == false);
            Assert.AreEqual(true, organizer.vehicles[0].RegNum == "MC123");
            Assert.AreEqual(true, organizer.vehicles[1].RegNum == "MC234");
            // Kollar ifall fordonen flyttades till rätt plattser.
            Assert.AreEqual(3, organizer.vehicles[0].ParkingNum);
            Assert.AreEqual(0, organizer.vehicles[1].ParkingNum);
            // Nu har allt kollats så att upplägget för testet är korrekt.
            // ===> här börjar testet ===>
            OptimizeParkingSpace(organizer, true, "dd", 0);
            OptimizeParkingSpace(organizer, true, "dd", 0);
            Vehicle vehicle0 = organizer.SearchForVehicle("MC123");
            Vehicle vehicle1 = organizer.SearchForVehicle("MC234");
            Assert.AreEqual(0, vehicle0.ParkingNum);
            Assert.AreEqual(0, vehicle1.ParkingNum);

            Assert.AreEqual(true, vehicle0.FillsWholeSpace);
            Assert.AreEqual(true, vehicle1.FillsWholeSpace);
        }


        [TestMethod()]
        public void SearchForRegTest()
        {

            Organizer organizer = new Organizer(new UserInput());
            AddVehicle(organizer, true, "ABC123");

            Assert.AreEqual(0, organizer.SearchForReg("ABC123"));
        }

        [TestMethod()]
        public void TestAlphabethTest()
        {

            Organizer organizer = new Organizer(new UserInput());
            string regNum = "ABC123";
            AddVehicle(organizer, true, regNum);

            Assert.AreEqual(0, organizer.SearchForReg(regNum));
            regNum = "АБЦABC123";
            AddVehicle(organizer, true, regNum);

            Assert.AreEqual(1, organizer.SearchForReg(regNum));
            regNum = "ŪŌĪ123";
            AddVehicle(organizer, true, regNum);

            Assert.AreEqual(2, organizer.SearchForReg(regNum));
            regNum = "としは123".ToUpper();
            AddVehicle(organizer, true, regNum);

            Assert.AreEqual(3, organizer.SearchForReg(regNum));
            regNum = "עִבְרִ123".ToUpper();
            AddVehicle(organizer, true, regNum);

            Assert.AreEqual(4, organizer.SearchForReg(regNum));
        }

        private static Organizer RemovesVehicle(Organizer organizer, bool isCar, string regNum)
        {
            var mock = new Mock<IUserInput>();
            mock.Setup(p => p.AskForReg()).Returns(regNum);
            mock.Setup(p => p.AskForType()).Returns(isCar);
            mock.Setup(p => p.BackToMenu()).Returns(true);
            mock.Setup(p => p.CheckOutVehicle(new Vehicle("", true))).Returns(true);

            organizer.userInput = (IUserInput)mock.Object;

            //            organizer.userInput = userInput;

            organizer.UserRemovesVehicle();
            return organizer;
        }

        private static Organizer AddVehicle(Organizer organizer, bool isCar, string regNum)
        {
            var mock = new Mock<IUserInput>();
            mock.Setup(p => p.AskForReg()).Returns(regNum);
            mock.Setup(p => p.AskForType()).Returns(isCar);
            mock.Setup(p => p.BackToMenu()).Returns(true);

            organizer.userInput = (IUserInput)mock.Object;

            organizer.UserAddVehicle();
            return organizer;
        }

        private static Organizer MoveVehicle(Organizer organizer, bool isCar, string regNum, int moveToIndex)
        {
            var mock = new Mock<IUserInput>();
            mock.Setup(p => p.AskForReg()).Returns(regNum);
            mock.Setup(p => p.AskForReg()).Returns(regNum);
            mock.Setup(p => p.AskForType()).Returns(isCar);
            mock.Setup(p => p.BackToMenu()).Returns(true);
            mock.Setup(p => p.AskForNumber(1, 3))
                .Returns(moveToIndex + 1);mock.Setup(p => p.AskForNumber(1, 100))
                .Returns(moveToIndex + 1);


            //UserInputStub userInput = new UserInputStub();
            //userInput.RegNum = regNum;
            //userInput.IsCar = isCar;
            //userInput.IndexAnswer = moveToIndex + 1;
            organizer.userInput = (IUserInput)mock.Object;

            organizer.UserMoveVehicle();

            return organizer;
        }
        private static Organizer OptimizeParkingSpace(Organizer organizer, bool isCar, string regNum, int userOptimizeChoise)
        {
            var mock = new Mock<IUserInput>();
            mock.Setup(p => p.AskForReg()).Returns(regNum);
            mock.Setup(p => p.AskForType()).Returns(isCar);
            mock.Setup(p => p.BackToMenu()).Returns(true);
            mock.Setup(p => p.AskForNumber(0, 100))
                .Returns(userOptimizeChoise);
            

            organizer.userInput = (IUserInput) mock.Object;

            organizer.UserOptimizeMcSpace();

            return organizer;
        }
    }
}
