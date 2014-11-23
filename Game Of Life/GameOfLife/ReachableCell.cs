using System.Collections.Generic;
using System;

namespace GameOfLife
{
    /// <summary>
    /// Cell types are unique types of cell in grid of any size
    /// Every cell type has distinct reachable djacent cells which can be traversed
    /// </summary>
    enum CellTypeEnum
    {
        TopLeftCorner,
        TopRightCorner,
        BottomLeftCorner,
        BottomRightCorner,
        TopSide,
        BottomSide,
        LeftSide,
        RightSide,
        Center,
        OuterTopSide,
        OuterRightSide,
        OuterBottomSide,
        OuterLeftSide,
        None
    }
    /// <summary>
    /// structure to hold x and y indices of grid cell
    /// </summary>
    struct CoOrdinates
    {
        public int X;
        public int Y;
        public CoOrdinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    static class ReachableCell
    {
        // Dictionary to hold list of reachable cells co-ordinates for specified cell type
        public static Dictionary<CellTypeEnum, List<CoOrdinates>> ReachableCells;
        /// <summary>
        /// initialize all reachable cells in Dictionary(Key=> CellTypeEnum, Value => List of Reachable cell co-ordinates
        /// </summary>
        public static void InitializeReachableCells()
        {
            CellTypeEnum innerCell;
            ReachableCells = new Dictionary<CellTypeEnum, List<CoOrdinates>>();

            // Add Reachable adjacent cell co-ordinates for Top Left corner cell into Dictionary with TopLeftCorner CellTypeEnum as key
            innerCell = CellTypeEnum.TopLeftCorner;
            List<CoOrdinates> TopLeftCoOrdinateList = new List<CoOrdinates>();
            TopLeftCoOrdinateList.Add(new CoOrdinates(0, 1));
            TopLeftCoOrdinateList.Add(new CoOrdinates(1, 1));
            TopLeftCoOrdinateList.Add(new CoOrdinates(1, 0));
            ReachableCells.Add(innerCell, TopLeftCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for Top right corner cell into Dictionary with TopRightCorner CellTypeEnum as key
            innerCell = CellTypeEnum.TopRightCorner;
            List<CoOrdinates> TopRightCoOrdinateList = new List<CoOrdinates>();
            TopRightCoOrdinateList.Add(new CoOrdinates(1, 0));
            TopRightCoOrdinateList.Add(new CoOrdinates(1, -1));
            TopRightCoOrdinateList.Add(new CoOrdinates(0, -1));
            ReachableCells.Add(innerCell, TopRightCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for bottom left corner cell into Dictionary with BottomLeftCorner CellTypeEnum as key
            innerCell = CellTypeEnum.BottomLeftCorner;
            List<CoOrdinates> BottomLeftCoOrdinateList = new List<CoOrdinates>();
            BottomLeftCoOrdinateList.Add(new CoOrdinates(-1, 0));
            BottomLeftCoOrdinateList.Add(new CoOrdinates(-1, 1));
            BottomLeftCoOrdinateList.Add(new CoOrdinates(0, 1));
            ReachableCells.Add(innerCell, BottomLeftCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for bottom right corner cell into Dictionary with BottomRightCorner CellTypeEnum as key
            innerCell = CellTypeEnum.BottomRightCorner;
            List<CoOrdinates> BottomRightCoOrdinateList = new List<CoOrdinates>();
            BottomRightCoOrdinateList.Add(new CoOrdinates(0, -1));
            BottomRightCoOrdinateList.Add(new CoOrdinates(-1, -1));
            BottomRightCoOrdinateList.Add(new CoOrdinates(-1, 0));
            ReachableCells.Add(innerCell, BottomRightCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for top side cell into Dictionary with BottomRightCorner TopSide as key
            innerCell = CellTypeEnum.TopSide;
            List<CoOrdinates> TopSideCoOrdinateList = new List<CoOrdinates>();
            TopSideCoOrdinateList.Add(new CoOrdinates(0, 1));
            TopSideCoOrdinateList.Add(new CoOrdinates(1, 1));
            TopSideCoOrdinateList.Add(new CoOrdinates(1, 0));
            TopSideCoOrdinateList.Add(new CoOrdinates(1, -1));
            TopSideCoOrdinateList.Add(new CoOrdinates(0, -1));
            ReachableCells.Add(innerCell, TopSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for bottom side cell into Dictionary with BottomRightCorner BottomSide as key
            innerCell = CellTypeEnum.BottomSide;
            List<CoOrdinates> BottomSideCoOrdinateList = new List<CoOrdinates>();
            BottomSideCoOrdinateList.Add(new CoOrdinates(0, -1));
            BottomSideCoOrdinateList.Add(new CoOrdinates(-1, -1));
            BottomSideCoOrdinateList.Add(new CoOrdinates(-1, 0));
            BottomSideCoOrdinateList.Add(new CoOrdinates(-1, 1));
            BottomSideCoOrdinateList.Add(new CoOrdinates(0, 1));
            ReachableCells.Add(innerCell, BottomSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for left side cell into Dictionary with BottomRightCorner LeftSide as key
            innerCell = CellTypeEnum.LeftSide;
            List<CoOrdinates> LeftSideCoOrdinateList = new List<CoOrdinates>();
            LeftSideCoOrdinateList.Add(new CoOrdinates(-1, 0));
            LeftSideCoOrdinateList.Add(new CoOrdinates(-1, 1));
            LeftSideCoOrdinateList.Add(new CoOrdinates(0, 1));
            LeftSideCoOrdinateList.Add(new CoOrdinates(1, 1));
            LeftSideCoOrdinateList.Add(new CoOrdinates(1, 0));
            ReachableCells.Add(innerCell, LeftSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for right side cell into Dictionary with BottomRightCorner RightSide as key
            innerCell = CellTypeEnum.RightSide;
            List<CoOrdinates> RightSideCoOrdinateList = new List<CoOrdinates>();
            RightSideCoOrdinateList.Add(new CoOrdinates(1, 0));
            RightSideCoOrdinateList.Add(new CoOrdinates(1, -1));
            RightSideCoOrdinateList.Add(new CoOrdinates(0, -1));
            RightSideCoOrdinateList.Add(new CoOrdinates(-1, -1));
            RightSideCoOrdinateList.Add(new CoOrdinates(-1, 0));
            ReachableCells.Add(innerCell, RightSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for Center cell into Dictionary with BottomRightCorner Center as key
            innerCell = CellTypeEnum.Center;
            List<CoOrdinates> CenterCoOrdinateList = new List<CoOrdinates>();
            CenterCoOrdinateList.Add(new CoOrdinates(-1, 0));
            CenterCoOrdinateList.Add(new CoOrdinates(-1, 1));
            CenterCoOrdinateList.Add(new CoOrdinates(0, 1));
            CenterCoOrdinateList.Add(new CoOrdinates(1, 1));
            CenterCoOrdinateList.Add(new CoOrdinates(1, 0));
            CenterCoOrdinateList.Add(new CoOrdinates(1, -1));
            CenterCoOrdinateList.Add(new CoOrdinates(0, -1));
            CenterCoOrdinateList.Add(new CoOrdinates(-1, -1));
            ReachableCells.Add(innerCell, CenterCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for outer top side cell into Dictionary with BottomRightCorner OuterTopSide as key
            innerCell = CellTypeEnum.OuterTopSide;
            List<CoOrdinates> OuterTopSideCoOrdinateList = new List<CoOrdinates>();
            OuterTopSideCoOrdinateList.Add(new CoOrdinates(1, -1));
            OuterTopSideCoOrdinateList.Add(new CoOrdinates(1, 0));
            OuterTopSideCoOrdinateList.Add(new CoOrdinates(1, 1));
            ReachableCells.Add(innerCell, OuterTopSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for outer right side cell into Dictionary with BottomRightCorner OuterRightSide as key
            innerCell = CellTypeEnum.OuterRightSide;
            List<CoOrdinates> OuterRightSideCoOrdinateList = new List<CoOrdinates>();
            OuterRightSideCoOrdinateList.Add(new CoOrdinates(-1, -1));
            OuterRightSideCoOrdinateList.Add(new CoOrdinates(0, -1));
            OuterRightSideCoOrdinateList.Add(new CoOrdinates(1, -1));
            ReachableCells.Add(innerCell, OuterRightSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for outer bottom side cell into Dictionary with BottomRightCorner OuterBottomSide as key
            innerCell = CellTypeEnum.OuterBottomSide;
            List<CoOrdinates> OuterBottomSideCoOrdinateList = new List<CoOrdinates>();
            OuterBottomSideCoOrdinateList.Add(new CoOrdinates(-1, -1));
            OuterBottomSideCoOrdinateList.Add(new CoOrdinates(-1, 0));
            OuterBottomSideCoOrdinateList.Add(new CoOrdinates(-1, 1));
            ReachableCells.Add(innerCell, OuterBottomSideCoOrdinateList);

            // Add Reachable adjacent cell co-ordinates for outer left side cell into Dictionary with BottomRightCorner OuterLeftSide as key
            innerCell = CellTypeEnum.OuterLeftSide;
            List<CoOrdinates> OuterLeftSideCoOrdinateList = new List<CoOrdinates>();
            OuterLeftSideCoOrdinateList.Add(new CoOrdinates(-1, 1));
            OuterLeftSideCoOrdinateList.Add(new CoOrdinates(0, 1));
            OuterLeftSideCoOrdinateList.Add(new CoOrdinates(1, 1));
            ReachableCells.Add(innerCell, OuterLeftSideCoOrdinateList);

        }

        /// <summary>
        /// Get the co-ordinates with respectt to grid and return the Cell type enum
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="coOrdinates"></param>
        /// <returns>returns CellTypeEnum</returns>
        public static CellTypeEnum GetCellType(Grid grid, CoOrdinates coOrdinates)
        {
            if ((coOrdinates.X < -1 || coOrdinates.X > grid.RowCount) || (coOrdinates.Y < -1 || coOrdinates.Y > grid.ColumnCount))
            {
                throw new ArgumentOutOfRangeException("Invalid Index value: must be greater than or equal to minus one and less than or equal to Row count");
            }
            CellTypeEnum enumCellType = CellTypeEnum.None;
            if (coOrdinates.X == 0 && coOrdinates.Y == 0)
                enumCellType = CellTypeEnum.TopLeftCorner;
            else if (coOrdinates.X == 0 && coOrdinates.Y == grid.ColumnCount - 1)
                enumCellType = CellTypeEnum.TopRightCorner;
            else if (coOrdinates.X == grid.RowCount - 1 && coOrdinates.Y == 0)
                enumCellType = CellTypeEnum.BottomLeftCorner;
            else if (coOrdinates.X == grid.RowCount - 1 && coOrdinates.Y == grid.ColumnCount - 1)
                enumCellType = CellTypeEnum.BottomRightCorner;
            else if (coOrdinates.X == 0 && (coOrdinates.Y > 0 && coOrdinates.Y < grid.ColumnCount - 1))
                enumCellType = CellTypeEnum.TopSide;
            else if (coOrdinates.X == grid.RowCount - 1 && (coOrdinates.Y > 0 && coOrdinates.Y < grid.ColumnCount - 1))
                enumCellType = CellTypeEnum.BottomSide;
            else if ((coOrdinates.X > 0 && coOrdinates.X < grid.RowCount - 1) && coOrdinates.Y == 0)
                enumCellType = CellTypeEnum.LeftSide;
            else if ((coOrdinates.X > 0 && coOrdinates.X < grid.RowCount - 1) && coOrdinates.Y == grid.ColumnCount - 1)
                enumCellType = CellTypeEnum.RightSide;
            else if ((coOrdinates.X > 0 && coOrdinates.X < grid.RowCount - 1) && (coOrdinates.Y > 0 && coOrdinates.Y < grid.ColumnCount - 1))
                enumCellType = CellTypeEnum.Center;
            else if (coOrdinates.X == -1 && (coOrdinates.Y > 0 && coOrdinates.Y < grid.ColumnCount - 1))
                enumCellType = CellTypeEnum.OuterTopSide;
            else if ((coOrdinates.X > 0 && coOrdinates.X < grid.RowCount - 1) && coOrdinates.Y == grid.ColumnCount)
                enumCellType = CellTypeEnum.OuterRightSide;
            else if (coOrdinates.X == grid.RowCount && (coOrdinates.Y > 0 && coOrdinates.Y < grid.ColumnCount - 1))
                enumCellType = CellTypeEnum.OuterBottomSide;
            else if ((coOrdinates.X > 0 && coOrdinates.X < grid.RowCount - 1) && coOrdinates.Y == -1)
                enumCellType = CellTypeEnum.OuterLeftSide;
            return enumCellType;
        }

    }
}
