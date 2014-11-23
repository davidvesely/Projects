using System;
using System.Collections.Generic;

namespace GameOfLife
{
    class Rule
    {
        /// <summary>
        /// Private Constructor
        /// </summary>
        private Rule()
        {

        }

        /// <summary>
        /// Change Cell state of specified co-ordinate using Ruls
        /// </summary>
        /// <param name="inputGrid"></param>
        /// <param name="outputGrid"></param>
        /// <param name="coOrdinates"></param>
        public static void ChangeCellsState(Grid inputGrid, Grid outputGrid, CoOrdinates coOrdinates)
        {
            int liveNeighbourCount = CountAliveNeighbours(inputGrid, coOrdinates);
            lock (outputGrid)
            {
                if (IsAliveInNextState(inputGrid[coOrdinates.X, coOrdinates.Y], liveNeighbourCount))
                {

                    //set output grid's cell to live only if it is in alive status in next generation
                    outputGrid[coOrdinates.X, coOrdinates.Y].IsAlive = true;
                }

            }

        }

        /// <summary>
        /// Count live adjacent cells for specified cell co-ordinates
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coOrdinates"></param>
        /// <returns>returns number of live neighbours</returns>
        private static int CountAliveNeighbours(Grid grid, CoOrdinates coOrdinates)
        {
            int liveNeighbours = 0;
            // Get the Cell type of current cell
            CellTypeEnum enumInnerCell = ReachableCell.GetCellType(grid, coOrdinates);
            List<CoOrdinates> reachableCells = new List<CoOrdinates>();
            // populate reachable cells from current cell for easier traversing
            ReachableCell.ReachableCells.TryGetValue(enumInnerCell, out reachableCells);
            if (reachableCells.Count == 0) throw new ArgumentNullException("Cannot find reachable co-ordinates");
            foreach (CoOrdinates coOrds in reachableCells)
            {
                liveNeighbours += IsAliveNeighbour(grid, coOrdinates, coOrds);
            }
            return liveNeighbours;
        }


        /// <summary>
        /// Check if the adjacent cell is alive or not
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="baseCoOrdinates"></param>
        /// <param name="offSetCoOrdinates"></param>
        /// <returns>returns 1 if live otherwise 0</returns>
        private static int IsAliveNeighbour(Grid grid, CoOrdinates baseCoOrdinates, CoOrdinates offSetCoOrdinates)
        {
            int live = 0; // set default as 0
            int x = baseCoOrdinates.X + offSetCoOrdinates.X; // get x axis of neighbour
            int y = baseCoOrdinates.Y + offSetCoOrdinates.Y; // get y axis of neighbour
            // check the computed bound is within range of grid, if it is not within bounds live is 0 as default
            if ((x >= 0 && x < grid.RowCount) && y >= 0 && y < grid.ColumnCount)
            {
                // if reachable neighbour cell is alive then set live to 1 otherwise 0
                live = grid[x, y].IsAlive ? 1 : 0;
            }

            return live;
        }

        /// <summary>
        /// Evaluate Cell state in next generation
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="liveNeighbourCount"></param>
        /// <returns>returns true if alive otherwise false</returns>
        private static Boolean IsAliveInNextState(Cell cell, int liveNeighbourCount)
        {
            Boolean alive = false;
            if (cell.IsAlive)
            {
                // if cell is alive and 2 or 3 ajacent cells are alive then set it to alive in next generation
                if (liveNeighbourCount == 2 || liveNeighbourCount == 3)
                {
                    alive = true;
                }
            }
            // if cell is dead and 3 adjacent cells are alive then set it to alive in next generation
            else if (liveNeighbourCount == 3)
            {
                alive = true;
            }
            return alive;
        }

        /// <summary>
        /// Change state of grid if required to grow on any side
        /// </summary>
        /// <param name="inputGrid"></param>
        /// <param name="outputGrid"></param>
        public static void ChangeGridState(Grid inputGrid, Grid outputGrid)
        {
            CheckRowGrowth(inputGrid, outputGrid, -1);
            CheckRowGrowth(inputGrid, outputGrid, inputGrid.RowCount);
            CheckColumnGrowth(inputGrid, outputGrid, -1);
            CheckColumnGrowth(inputGrid, outputGrid, inputGrid.ColumnCount);
        }

        /// <summary>
        /// Check if rule satisfies to expand column 
        /// </summary>
        /// <param name="inputGrid"></param>
        /// <param name="outputGrid"></param>
        /// <param name="colId"></param>
        private static void CheckColumnGrowth(Grid inputGrid, Grid outputGrid, int colId)
        {
            //Create a whole new column in the beginning or end if rule is satified for any of the cell
            Boolean columnCreatedFlag = false;
            //Boolean IsPreviousCellsFilled = false;
            // start with the index 1  until 1 less than last index as index 0 and last index cannot have 3 live adjacent cell in any case
            // This index 0 and last index must be included if rule is changed in future; dead can alive with 2 live adjacent cells
            for (int i = 1; i < inputGrid.RowCount - 1; i++)
            {
                if (Rule.CountAliveNeighbours(inputGrid, new CoOrdinates(i, colId)) == 3)
                {
                    if (columnCreatedFlag == false)
                    {
                        //if (IsPreviousCellsFilled == false)
                        //{
                        for (int k = 0; k < outputGrid.RowCount; k++)
                        {
                            // Fill all cells with false
                            Cell newDeadCell = new Cell(false);
                            if (colId == -1)
                            {
                                outputGrid[k].InsertCell(0, newDeadCell, outputGrid.ColumnCount);
                            }
                            else
                            {
                                outputGrid[k].AddCell(newDeadCell);
                            }
                        }
                        //    IsPreviousCellsFilled = true;
                        //}
                        // increment column count to 1
                        outputGrid.ColumnCount += 1;
                        columnCreatedFlag = true;
                    }
                    int yAxis = (colId == -1) ? 0 : outputGrid.ColumnCount - 1;
                    outputGrid[i, yAxis].IsAlive = true;
                }
            }
        }
        /// <summary>
        /// Check if rule satisfies to expand row
        /// </summary>
        /// <param name="inputGrid"></param>
        /// <param name="outputGrid"></param>
        /// <param name="rowId"></param>
        private static void CheckRowGrowth(Grid inputGrid, Grid outputGrid, int rowId)
        {
            //Create a whole new row in the beginning or end if rule is satified for any of the cell
            Boolean rowCreatedFlag = false;
            //Boolean IsPreviousCellsFilled = false;
            // start with the index 1  until 1 less than last index as index 0 and last index cannot have 3 live adjacent cell in any case
            // This index 0 and last index must be included if rule is changed in future; dead can alive with 2 live adjacent cells
            for (int j = 1; j < inputGrid.ColumnCount - 1; j++)
            {
                if (Rule.CountAliveNeighbours(inputGrid, new CoOrdinates(rowId, j)) == 3)
                {
                    if (rowCreatedFlag == false)
                    {
                        Row newRow = new Row();
                        //if (IsPreviousCellsFilled == false)
                        //{
                        for (int k = 0; k < outputGrid.ColumnCount; k++)
                        {
                            // Fill all cells with false
                            Cell newDeadCell = new Cell(false);
                            newRow.AddCell(newDeadCell);
                        }
                        //IsPreviousCellsFilled = true;
                        //}
                        if (rowId == -1)
                        {
                            outputGrid.InsertRow(0, newRow);
                        }
                        else
                        {
                            outputGrid.AddRow(newRow);
                        }
                        rowCreatedFlag = true;
                    }
                    int XAxis = (rowId == -1) ? 0 : outputGrid.RowCount - 1;
                    outputGrid[XAxis, j].IsAlive = true;
                }
            }
        }

    }

}
