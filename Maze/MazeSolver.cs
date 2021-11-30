using System;
using System.Collections.Generic;

#pragma warning disable CA1814
#pragma warning disable S2368

namespace Maze
{
    /// <summary>
    /// Class for finding exit out of a maze. It is assumed that the maze always has one entrance and only one exit (if any) and they are different.
    /// </summary>
    public class MazeSolver
    {
        private readonly int startX;
        private readonly int startY;

        private readonly int length;

        private readonly int[,] helperMap;

        private int step;
        private bool finished;
        private int finishX;
        private int finishY;

        private List<(int, int)> path;

        /// <summary>
        /// Initializes a new instance of the <see cref="MazeSolver"/> class.
        /// </summary>
        /// <param name="maze">Presents a maze as two-dimensional zero-based matrix.</param>
        /// <param name="rowStart">The zero-based index of row of the start.</param>
        /// <param name="columnStart">The zero-based index of column of the start.</param>
        /// <exception cref="ArgumentNullException">Thrown if passed maze is null.</exception>
        /// <exception cref="ArgumentException">Thrown if passed maze is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if rowStart or columnStart are not in mazeModel:
        /// less than zero or more then number of elements in the dimension.</exception>
        public MazeSolver(bool[,] maze, int rowStart, int columnStart)
        {
            if (maze is null)
            {
                throw new ArgumentNullException(nameof(maze));
            }

            if (maze.Length == 0)
            {
                throw new ArgumentException("invalid maze");
            }

            this.length = (int)Math.Sqrt(maze.Length);

            if (rowStart >= this.length || rowStart < 0 || columnStart < 0 || columnStart >= this.length)
            {
                throw new ArgumentOutOfRangeException(nameof(rowStart), "invalid start position");
            }

            this.startX = rowStart;
            this.startY = columnStart;
            this.helperMap = this.SetHelperMap(maze);
        }

        /// <summary>
        /// Starts an algorithm for finding shortest path.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the path does not exist.</exception>
        public void PassMaze()
        {
            if (this.WavesAlgorithm())
            {
                if (this.RestorePath())
                {
                    return;
                }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the shortest path as a one-dimensional array of the pairs (row, column).
        /// </summary>
        /// <returns>
        /// The one-dimensional array of the pairs (row, column).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if path finding algorithm wasn't started.
        /// </exception>
        public (int row, int column)[] GetPath()
        {
            if (this.path is null)
            { 
                throw new InvalidOperationException(); 
            }

            var result = this.path.ToArray();
            Array.Reverse(result);
            return result;
        }
        
        /// <summary>
        /// Gets the pairs (row, column) - indexes of row and columns of exit from maze.
        /// </summary>
        /// <returns>
        /// The indexes of row and columns of exit from maze.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if path finding algorithm wasn't started.
        /// </exception>
        public (int row, int column) GetExit() => this.path is null ? throw new InvalidOperationException() : (this.finishX, this.finishY);

        private int[,] SetHelperMap(bool[,] initialMap)
        {
            int length = (int)Math.Sqrt(initialMap.Length);
            int[,] map = new int[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    map[i, j] = initialMap[i, j] ? (int)Way.Wall : (int)Way.Path;
                }
            }

            map[this.startX, this.startY] = (int)Way.Start;
            return map;
        }

        private bool WavesAlgorithm()
        {
            bool finished = false;
            do
            {
                for (int i = 0; i < this.length; i++)
                {
                    for (int j = 0; j < this.length; j++)
                    {
                        if (this.helperMap[i, j] == this.step)
                        {
                            if (i != this.length - 1)
                            {
                                if (this.helperMap[i + 1, j] == (int)Way.Path)
                                {
                                    this.helperMap[i + 1, j] = this.step + 1;
                                }
                            }

                            if (j != this.length - 1)
                            {
                                if (this.helperMap[i, j + 1] == (int)Way.Path)
                                {
                                    this.helperMap[i, j + 1] = this.step + 1;
                                }
                            }

                            if (i != 0)
                            {
                                if (this.helperMap[i - 1, j] == (int)Way.Path)
                                {
                                    this.helperMap[i - 1, j] = this.step + 1;
                                }
                            }

                            if (j != 0)
                            {
                                if (this.helperMap[i, j - 1] == (int)Way.Path)
                                {
                                    this.helperMap[i, j - 1] = this.step + 1;
                                }
                            }

                            if (i < this.length - 1)
                            {
                                if (((i + 1) == this.length - 1 || j == this.length - 1 || (i + 1) == 0 || j == 0)
                                    && this.helperMap[i + 1, j] == this.step + 1)
                                {
                                    this.finishX = i + 1;
                                    this.finishY = j;
                                    finished = true;
                                }
                            }

                            if (j < this.length - 1)
                            {
                                if ((i == this.length - 1 || (j + 1) == this.length - 1 || i == 0 || j + 1 == 0)
                                   && this.helperMap[i, j + 1] == this.step + 1)
                                {
                                    this.finishX = i;
                                    this.finishY = j + 1;
                                    finished = true;
                                }
                            }

                            if (i > 0)
                            {
                                if (((i - 1) == (this.length - 1) || j == (this.length - 1) || (i - 1) == 0 || j == 0)
                                    && this.helperMap[i - 1, j] == this.step + 1)
                                {
                                    this.finishX = i - 1;
                                    this.finishY = j;
                                    finished = true;
                                }
                            }

                            if (j > 0)
                            {
                                if ((i == this.length - 1 || (j - 1) == this.length - 1 || i == 0 || j - 1 == 0)
                                    && this.helperMap[i, j - 1] == this.step + 1)
                                {
                                    this.finishX = i;
                                    this.finishY = j - 1;
                                    finished = true;
                                }
                            }
                        }
                    }
                }

                this.step++;
            }
            while (!finished && this.step < this.helperMap.Length);
            this.finished = finished;
            return finished;
        }

        private bool IsItFinish(int x, int y) => x == this.length - 1 || y == this.length - 1 || x == 0 || y == 0;

        private bool RestorePath()
        {
            if (!this.finished)
            {
                return false;
            }

            int i = this.finishX;
            int j = this.finishY;
            this.path = new List<(int, int)>();
            this.path.Add((i, j));

            do
            {
                if (i < this.length - 1)
                {
                    if (this.helperMap[i + 1, j] == this.step - 1)
                    {
                        this.path.Add((++i, j));
                    }
                }

                if (j < this.length - 1)
                {
                    if (this.helperMap[i, j + 1] == this.step - 1)
                    {
                        this.path.Add((i, ++j));
                    }
                }

                if (i > 0)
                {
                    if (this.helperMap[i - 1, j] == this.step - 1)
                    {
                        this.path.Add((--i, j));
                    }
                }

                if (j > 0)
                {
                    if (this.helperMap[i, j - 1] == this.step - 1)
                    {
                        this.path.Add((i, --j));
                    }
                }

                this.step--;
            }
            while (this.step != (int)Way.Start);
            return true;
        }
    }
}
