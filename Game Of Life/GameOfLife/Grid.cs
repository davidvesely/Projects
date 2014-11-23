using System;
using System.Collections.Generic;

namespace GameOfLife
{
    public class Grid
    {

        // List of Rows in Grid        
        public List<Row> GridObj { set; get; }

        /// <summary>
        /// Create grid by using row and column count
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public Grid(int rows, int columns)
        {            
            Setup(rows, columns);
        }

        /// <summary>
        /// Indexer to get grid cell by using indexes for ease of use
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>returns cell</returns>
        public Cell this[int x, int y]
        {
            get { if (GridObj.Count <= x || ColumnCount <= y) throw new ArgumentOutOfRangeException("Argument out of bound"); return GridObj[x].Cells[y]; }
            set { if (GridObj.Count <= x || ColumnCount <= y) throw new ArgumentOutOfRangeException("Argument out of bound"); GridObj[x].Cells[y] = value; }
        }
        /// <summary>
        /// Indexer to get grid row by using index for ease of use
        /// </summary>
        /// <param name="x"></param>
        /// <returns>returns row</returns>
        public Row this[int x]
        {
            get { if (GridObj.Count <= x) throw new ArgumentOutOfRangeException("Argument out of bound"); return GridObj[x]; }
            set { if (GridObj.Count <= x) throw new ArgumentOutOfRangeException("Argument out of bound"); GridObj[x] = value; }
        }
        // Get number of rows in grid
        public int RowCount { get { return GridObj.Count; } }
        // Get or Set number of columns in grid
        public int ColumnCount { set; get; }

        /// <summary>
        /// Re-initialize a grid with all dead cells
        /// </summary>
        public void ReInitialize()
        {
            Setup(RowCount, ColumnCount);
        }
        /// <summary>
        /// Setup grid by using row and column count
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        private void Setup(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentOutOfRangeException("Row and Column size must be greater than zero");
            GridObj = new List<Row>();
            for (int i = 0; i < rows; i++)
            {
                Row row = new Row();
                for (int j = 0; j < columns; j++)
                {
                    Cell cell = new Cell(false);
                    row.AddCell(cell);
                }
                GridObj.Add(row);
            }
            ColumnCount = columns;
        }
        /// <summary>
        /// Toggle state of input grid cells from its current state; live to dead or vice-verca
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>        
        public void ToggleCell(int x, int y)
        {
            if (GridObj.Count <= x || ColumnCount <= y) throw new ArgumentNullException("Cell doesn't have data for required indexes");
            this[x, y].IsAlive = !this[x, y].IsAlive;
        }
        /// <summary>
        /// Inserts a new row in the grid at specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="row"></param>
        public void InsertRow(int index, Row row)
        {
            if (index < 0 || index >= RowCount) throw new ArgumentOutOfRangeException("Invalid Index value: must be greater than or equal to zero and less than Row count");
            GridObj.Insert(index, row);
        }

        /// <summary>
        /// Add a new row in grid at the end in row list
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(Row row)
        {
            GridObj.Add(row);
        }

    }

}
