using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfLife;

namespace GameOfLifeTest
{
    [TestClass]
    public class GridTest
    {
        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        public void GridConstructorPositiveTest()
        {
            int rows = 2;
            int columns = 2;
            Grid target = new Grid(rows, columns);
            Assert.AreEqual(target.RowCount, 2);
            Assert.AreEqual(target.ColumnCount, 2);
        }


        /// <summary>
        ///A test for Game Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Row and Column size must be greater than or equal to zero")]
        public void GridConstructorExceptionTest1()
        {
            int rows = -1;
            int columns = 0;
            Grid target = new Grid(rows, columns);

        }

        

    }
}