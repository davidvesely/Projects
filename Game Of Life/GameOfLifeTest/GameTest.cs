using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLife;

namespace GameOfLifeTest
{
    [TestClass]
    public class GameTest
    {
        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        public void GameConstructorPositiveTest()
        {
            int rows = 2;
            int columns = 2;
            Game target = new Game(rows, columns);
            Assert.AreEqual(target.RowCount, 2);
            Assert.AreEqual(target.ColumnCount, 2);
        }

        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Row and Column size must be greater than or equal to zero")]
        public void GameConstructorExceptionTest1()
        {
            int rows = -1;
            int columns = 0;
            Game target = new Game(rows, columns);

        }

        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Row and Column size must be greater than zero")]
        public void GameConstructorExceptionTest2()
        {
            int rows = 0;
            int columns = -1;
            Game target = new Game(rows, columns);
        }

        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Row and Column size must be greater than zero")]
        public void GameConstructorExceptionTest3()
        {
            int rows = 0;
            int columns = 0;
            Game target = new Game(rows, columns);
        }

        /// <summary>
        ///A test for ToggleGridCell
        ///</summary>
        [TestMethod()]
        public void ToggleGridCellPositiveTest()
        {
            int rows = 2;
            int columns = 3;
            Game target = new Game(rows, columns);
            int x = 1;
            int y = 2;
            target.ToggleGridCell(x, y);
            Assert.AreEqual(target.InputGrid[1, 2].IsAlive, true);
        }

        /// <summary>
        ///A test for ToggleGridCell
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Argument out of bound")]
        public void ToggleGridCellExceptionTest1()
        {
            int rows = 1;
            int columns = 0;
            Game target = new Game(rows, columns);
            int x = 0;
            int y = 0;
            target.ToggleGridCell(x, y);
        }

        // <summary>
        ///A test for ToggleGridCell
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Argument out of bound")]
        public void ToggleGridCellExcecptinoTest2()
        {
            int rows = 0;
            int columns = 1;
            Game target = new Game(rows, columns);
            int x = 1;
            int y = 1;
            target.ToggleGridCell(x, y);
        }

        // <summary>
        ///A test for ToggleGridCell
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Argument out of bound")]
        public void ToggleGridCellExceptionTest2()
        {
            int rows = 2;
            int columns = 3;
            Game target = new Game(rows, columns);
            int x = 3;
            int y = 3;
            target.ToggleGridCell(x, y);
        }



        /// <summary>
        ///A test for validating MaxGeneration value
        ///</summary>
        [TestMethod()]
        public void MaxGenerationTest()
        {
            int rows = 2;
            int columns = 2;

            Game target = new Game(rows, columns);
            target.MaxGenerations = 2;
            Assert.AreEqual(target.MaxGenerations, 2);
        }

        /// <summary>
        ///A default test for Init
        ///</summary>
        [TestMethod()]
        public void InitDefaultValueTest()
        {
            int rows = 2;
            int columns = 2;

            Game target = new Game(rows, columns);
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 0].IsAlive, false);
            Assert.AreEqual(target.InputGrid[0, 1].IsAlive, false);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, false);
            Assert.AreEqual(target.InputGrid[1, 1].IsAlive, false);
        }

        /// <summary>
        ///A Block pattern test for Init
        ///</summary>
        [TestMethod()]
        public void InitBlockPatternTest()
        {
            int rows = 2;
            int columns = 2;
            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 0);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(1, 0);
            target.ToggleGridCell(1, 1);
            target.MaxGenerations = 100;// This pattern remains unchanged for infinite generation, testing it for 100 generations
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[0, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 1].IsAlive, true);
        }

        /// <summary>
        ///A boat pattern test for Init
        ///</summary>
        [TestMethod()]
        public void InitBoatPatternTest()
        {
            int rows = 3;
            int columns = 3;
            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 0);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(1, 0);
            target.ToggleGridCell(1, 2);
            target.ToggleGridCell(2, 1);
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[0, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 1].IsAlive, true);
        }

        /// <summary>
        ///A Blinker test for Init
        ///</summary>
        [TestMethod()]
        public void InitBlinkerPatternTest()
        {
            int rows = 3;
            int columns = 3;
            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(1, 1);
            target.ToggleGridCell(2, 1);

            target.Init();
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 2].IsAlive, true);
        }


        /// <summary>
        ///A Toad pattern 1 test for Init
        ///</summary>
        [TestMethod()]
        public void InitToadPattern1Test()
        {
            int rows = 2;
            int columns = 4;
            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(0, 2);
            target.ToggleGridCell(0, 3);
            target.ToggleGridCell(1, 0);
            target.ToggleGridCell(1, 1);
            target.ToggleGridCell(1, 2);
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 1].IsAlive, true);
        }

        /// <summary>
        ///A Toad pattern 2 test for Init
        ///</summary>
        [TestMethod()]
        public void InitToadPattern2Test()
        {
            int rows = 4;
            int columns = 2;
            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 0);
            target.ToggleGridCell(1, 0);
            target.ToggleGridCell(1, 1);
            target.ToggleGridCell(2, 0);
            target.ToggleGridCell(2, 1);
            target.ToggleGridCell(3, 1);
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[0, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 2].IsAlive, true);
        }


        /// <summary>
        ///A Diamond Pattern test
        ///</summary>
        [TestMethod()]
        public void InitDiamondPatternTest()
        {
            int rows = 3;
            int columns = 4;

            Game target = new Game(rows, columns);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(0, 2);
            target.ToggleGridCell(1, 0);
            target.ToggleGridCell(1, 3);
            target.ToggleGridCell(2, 1);
            target.ToggleGridCell(2, 2);
            target.MaxGenerations = 50; // This pattern remains unchanged for infinite generation, testing it for 50 generations
            target.Init();
            Assert.AreEqual(target.InputGrid[0, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[0, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[1, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 2].IsAlive, true);
        }


        /// <summary>
        ///A test for Queen Bee Shuttle pattern
        ///</summary>
        [TestMethod()]
        public void InitQueenBeeShuttleTest()
        {
            // The Queen Bee Shuttle pattern
            int rows = 7; 
            int columns = 4; 
            Game target = new Game(rows, columns);             
            target.ToggleGridCell(0, 0);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(1, 2);
            target.ToggleGridCell(2, 3);
            target.ToggleGridCell(3, 3);
            target.ToggleGridCell(4, 3);
            target.ToggleGridCell(5, 2);
            target.ToggleGridCell(6, 0);
            target.ToggleGridCell(6, 1);
            target.MaxGenerations = 100;
            target.Init();
            Assert.AreEqual(target.InputGrid[4, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 0].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 1].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 8].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 7].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 9].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 6].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 10].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 6].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 10].IsAlive, true);
            Assert.AreEqual(target.InputGrid[7, 7].IsAlive, true);
            Assert.AreEqual(target.InputGrid[7, 8].IsAlive, true);
            Assert.AreEqual(target.InputGrid[7, 9].IsAlive, true);
            Assert.AreEqual(target.InputGrid[9, 7].IsAlive, true);
            Assert.AreEqual(target.InputGrid[9, 8].IsAlive, true);
            Assert.AreEqual(target.InputGrid[9, 9].IsAlive, true);
            Assert.AreEqual(target.InputGrid[10, 6].IsAlive, true);
            Assert.AreEqual(target.InputGrid[10, 10].IsAlive, true);
            Assert.AreEqual(target.InputGrid[11, 6].IsAlive, true);
            Assert.AreEqual(target.InputGrid[11, 10].IsAlive, true);
            Assert.AreEqual(target.InputGrid[12, 7].IsAlive, true);
            Assert.AreEqual(target.InputGrid[12, 9].IsAlive, true);
            Assert.AreEqual(target.InputGrid[13, 8].IsAlive, true);
        }


        /// <summary>
        ///A test for Period 15 Oscillator pattern
        ///</summary>
        [TestMethod()]
        public void InitPeriod15OscillatorTest()
        {
            // The Queen Bee Shuttle pattern
            int rows = 1;
            int columns = 10;
            Game target = new Game(rows, columns);

            target.ToggleGridCell(0, 0);
            target.ToggleGridCell(0, 1);
            target.ToggleGridCell(0, 2);
            target.ToggleGridCell(0, 3);
            target.ToggleGridCell(0, 4);
            target.ToggleGridCell(0, 5);
            target.ToggleGridCell(0, 6);
            target.ToggleGridCell(0, 7);
            target.ToggleGridCell(0, 8);
            target.ToggleGridCell(0, 9);
            target.MaxGenerations = 50;
            target.Init();
            Assert.AreEqual(target.InputGrid[4, 2].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 3].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 3].IsAlive, true);            
            Assert.AreEqual(target.InputGrid[2, 4].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 4].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 4].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 4].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 4].IsAlive, true);

            Assert.AreEqual(target.InputGrid[4, 13].IsAlive, true);            
            Assert.AreEqual(target.InputGrid[3, 12].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 12].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 12].IsAlive, true);
            Assert.AreEqual(target.InputGrid[2, 11].IsAlive, true);
            Assert.AreEqual(target.InputGrid[3, 11].IsAlive, true);
            Assert.AreEqual(target.InputGrid[4, 11].IsAlive, true);
            Assert.AreEqual(target.InputGrid[5, 11].IsAlive, true);
            Assert.AreEqual(target.InputGrid[6, 11].IsAlive, true);

        }
    }
}
