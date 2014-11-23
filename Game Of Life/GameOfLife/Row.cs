using System.Collections.Generic;
using System;

namespace GameOfLife
{
    public class Row
    {
        //list of cells
        public List<Cell> Cells { get; set; }

        /// <summary>
        /// indexer to get cell using index
        /// </summary>
        /// <param name="y"></param>
        /// <returns>returns cell</returns>
        public Cell this[int y]
        {
            get { if (Cells.Count >= y) throw new ArgumentOutOfRangeException("Argument out of bound"); return Cells[y]; }
            set { if (Cells.Count >= y) throw new ArgumentOutOfRangeException("Argument out of bound"); Cells[y] = value; }
        }
        /// <summary>
        /// initialize list of cells
        /// </summary>
        public Row()
        {
            Cells = new List<Cell>();
        }
        /// <summary    >
        /// Add a cell into the row
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(Cell cell)
        {
            Cells.Add(cell);
        }
        /// <summary>
        /// Insert a cell into specified index position
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cell"></param>
        /// <param name="ColumnCount"></param>
        public void InsertCell(int index, Cell cell, int ColumnCount)
        {
            if (index < 0 || index >= ColumnCount) throw new ArgumentOutOfRangeException("Invalid Index value: must be greater than zero and less than Column count");
            Cells.Insert(index, cell);
        }

    }
}
