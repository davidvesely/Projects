using System;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class Game
    {
        private Grid _inputGrid;
        private Grid _outputGrid;
        public Grid InputGrid { get { return _inputGrid; } }  // input grod
        public Grid OutputGrid { get { return _outputGrid; } } // output grid
        //There are two Task for the Game of Life 
        // 1. Task for changing all existing Cell Status        
        private Task EvaluateCellTask;
        // 2. Task for expanding output gird if respective rule satifies
        private Task EvaluateGridGrowthTask;
        // MaxGeneration is used to restrict generations of grid changes
        public int MaxGenerations = 1; //set deafult as 1

        // Get number of rows in grid
        public int RowCount { get { return InputGrid.RowCount; } }
        // Get or Set number of columns in grid
        public int ColumnCount { get { return InputGrid.ColumnCount; } }


        /// <summary>
        /// Create input and output grids by using rows and column count and initialize reachable cells.
        /// Reachable Cells are cells which can be traversed from inner grid cells or outer grid cells i.e. virtual cells used for expanding grid
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public Game(int rows, int columns)
        {
            if (rows <= 0 || columns <= 0) throw new ArgumentOutOfRangeException("Row and Column size must be greater than zero");
            _inputGrid = new Grid(rows, columns);
            _outputGrid = new Grid(rows, columns);
            ReachableCell.InitializeReachableCells();
        }

        /// <summary>
        /// Toggle state of input grid cells from live to dead or vice-verca
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ToggleGridCell(int x, int y)
        {            
            if (_inputGrid.RowCount <= x || _inputGrid.ColumnCount <= y) throw new ArgumentOutOfRangeException("Argument out of bound");
            _inputGrid.ToggleCell(x, y);

        }

        /// <summary>
        /// Initialize the Game of lide
        /// </summary>
        public void Init()
        {
            Start();
        }
        /// <summary>
        /// Start Game of Life
        /// </summary>
        private void Start()
        {
            int currentGeneration = 0;
            GridHelper.Display(_inputGrid);
            do
            {
                currentGeneration++;
                // Process current generation for next generation
                ProcessGeneration();

                //*** un comment the below lines to see generation results after every 500 millisseconds  Note that tests will fail in console.clear() after uncommenting***
                //System.Threading.Thread.Sleep(500);
                //Console.Clear();
                //*** un comment the above lines to see generation results after every 500 millisseconds Note that tests will fail in console.clear() after uncommenting***
                
                Console.WriteLine("Generation: "+currentGeneration);                
                // Display the input grid
                GridHelper.Display(_inputGrid);
                // increment generation count                
            } while (currentGeneration < MaxGenerations);
        }
        /// <summary>
        /// Process current generation for next generation
        /// </summary>
        private void ProcessGeneration()
        {            
            SetNextGeneration();
            Tick();
            FlipGridState();
        }

        /// <summary>
        /// Handles tasks for setting next generation
        /// </summary>
        private void SetNextGeneration()
        {
            // Generate next state of the Grid if last generate state process is completed
            if ((EvaluateCellTask == null) || (EvaluateCellTask != null && EvaluateCellTask.IsCompleted))
            {
                EvaluateCellTask = ChangeCellsState();
                // ensure that Output grid existing cells are updated. 
                //Otherwise it may result in unpredictable result in output grid if row or column is added in parallel
                EvaluateCellTask.Wait();  
            }
            if ((EvaluateGridGrowthTask == null) || (EvaluateGridGrowthTask != null && EvaluateGridGrowthTask.IsCompleted))
            {
                EvaluateGridGrowthTask = ChangeGridState();
            }
        }
        /// <summary>
        /// Tick ensures that previous generation taks are completed
        /// </summary>
        private void Tick()
        {            
            if (EvaluateGridGrowthTask != null)
            {
                EvaluateGridGrowthTask.Wait();
            }
        }

        /// <summary>
        /// Set output grid to input grid by Deep Copy output grid into input grid
        /// </summary>
        private void FlipGridState()
        {
            GridHelper.Copy(_outputGrid, _inputGrid);
            _outputGrid.ReInitialize();
        }

        /// <summary>
        /// Change state of all input cells into output cells Simultaneously using Parallel For
        /// </summary>
        /// <returns>returns EvaluateCellTask</returns>
        private Task ChangeCellsState()
        {
            return Task.Factory.StartNew(() =>
            Parallel.For(0, _inputGrid.RowCount, x =>
            {
                Parallel.For(0, _inputGrid.ColumnCount, y =>
                {
                    Rule.ChangeCellsState(_inputGrid, _outputGrid, new CoOrdinates(x, y));
                });
            }));
        }
        /// <summary>
        /// Change state of grid if required
        /// </summary>
        /// <returns>returns EvaluateGridGrowthTask</returns>
        private Task ChangeGridState()
        {
            return Task.Factory.StartNew(delegate()
                {
                    Rule.ChangeGridState(_inputGrid, _outputGrid);
                }
            );
        }
    }
}
