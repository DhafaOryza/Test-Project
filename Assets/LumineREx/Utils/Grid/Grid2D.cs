using System.Collections;
using UnityEngine;

namespace LumineREx.Utils.Grid
{
    public class Grid2D<T> : IEnumerable
    {
        public int Height { get; }
        public int Width { get; }
        public float CellSize { get; }

        private T[,] _grid;

        public Grid2D(int height, int width, float cellSize)
        {
            Height = height;
            Width = width;
            CellSize = cellSize;

            _grid = new T[width, height];
        }

        public T GetValue(int x, int y)
        {
            return _grid[x, y];
        }

        public void SetValue(int x, int y, T value)
        {
            _grid[x, y] = value;
        }

        public T[,] GetGrid()
        {
            return _grid;
        }
        
        public IEnumerator GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return _grid[x, y];
                }
            }
        }
    }
}