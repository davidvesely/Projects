using System;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple Pattern
            //Game objLifeGame = new Game(4, 4);
            //objLifeGame.ToggleGridCell(1, 1);
            //objLifeGame.ToggleGridCell(1, 2);
            //objLifeGame.ToggleGridCell(2, 1);
            //objLifeGame.ToggleGridCell(2, 2);
            //objLifeGame.ToggleGridCell(2, 3);
            //objLifeGame.ToggleGridCell(3, 3);
            //objLifeGame.MaxGenerations = 4;
            //objLifeGame.Init();

            ////Toad Pattern 1
            //Game objLifeGame = new Game(2, 4);
            //objLifeGame.ToggleGridCell(0, 1);
            //objLifeGame.ToggleGridCell(0, 2);
            //objLifeGame.ToggleGridCell(0, 3);
            //objLifeGame.ToggleGridCell(1, 0);
            //objLifeGame.ToggleGridCell(1, 1);
            //objLifeGame.ToggleGridCell(1, 2);
            //objLifeGame.MaxGenerations = 8;
            //objLifeGame.Init();

            ////Toad Pattern 2
            //Game objLifeGame = new Game(4, 2);
            //objLifeGame.ToggleGridCell(0, 0);
            //objLifeGame.ToggleGridCell(1, 0);
            //objLifeGame.ToggleGridCell(1, 1);
            //objLifeGame.ToggleGridCell(2, 0);
            //objLifeGame.ToggleGridCell(2, 1);
            //objLifeGame.ToggleGridCell(3, 1);
            //objLifeGame.MaxGenerations = 100;
            //objLifeGame.Init();

            //Game objLifeGame = new Game(3, 4);
            //objLifeGame.ToggleGridCell(0, 1);
            //objLifeGame.ToggleGridCell(0, 2);
            //objLifeGame.ToggleGridCell(1, 0);
            //objLifeGame.ToggleGridCell(1, 3);
            //objLifeGame.ToggleGridCell(2, 1);
            //objLifeGame.ToggleGridCell(2, 2);
            //objLifeGame.MaxGenerations = 20;
            //objLifeGame.Init();


            // The Queen Bee Shuttle pattern
            //Game objLifeGame = new Game(7, 4);
            //objLifeGame.ToggleGridCell(0, 0);
            //objLifeGame.ToggleGridCell(0, 1);
            //objLifeGame.ToggleGridCell(1, 2);
            //objLifeGame.ToggleGridCell(2, 3);
            //objLifeGame.ToggleGridCell(3, 3);
            //objLifeGame.ToggleGridCell(4, 3);
            //objLifeGame.ToggleGridCell(5, 2);
            //objLifeGame.ToggleGridCell(6, 0);
            //objLifeGame.ToggleGridCell(6, 1);
            //objLifeGame.MaxGenerations = 100;
            //objLifeGame.Init();

            // The Period 3 Oscillator pattern
            //Game objLifeGame = new Game(5, 3);
            //objLifeGame.ToggleGridCell(0, 1);
            //objLifeGame.ToggleGridCell(1, 0);
            //objLifeGame.ToggleGridCell(1, 1);
            //objLifeGame.ToggleGridCell(1, 2);
            //objLifeGame.ToggleGridCell(2, 0);
            //objLifeGame.ToggleGridCell(2, 2);
            //objLifeGame.ToggleGridCell(3, 0);
            //objLifeGame.ToggleGridCell(3, 1);
            //objLifeGame.ToggleGridCell(3, 2);
            //objLifeGame.ToggleGridCell(4, 1);
            //objLifeGame.MaxGenerations = 50;
            //objLifeGame.Init();


            // The period-15 oscillator pattern
            Game objLifeGame = new Game(1, 10);
            objLifeGame.ToggleGridCell(0, 0);
            objLifeGame.ToggleGridCell(0, 1);
            objLifeGame.ToggleGridCell(0, 2);
            objLifeGame.ToggleGridCell(0, 3);
            objLifeGame.ToggleGridCell(0, 4);
            objLifeGame.ToggleGridCell(0, 5);
            objLifeGame.ToggleGridCell(0, 6);
            objLifeGame.ToggleGridCell(0, 7);
            objLifeGame.ToggleGridCell(0, 8);
            objLifeGame.ToggleGridCell(0, 9);
            objLifeGame.MaxGenerations = 50;
            objLifeGame.Init();

            Console.ReadKey();            
        }
    }
}
