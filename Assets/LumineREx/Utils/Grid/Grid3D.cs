using System.Collections;
using UnityEngine;

namespace LumineREx.Utils.Grid
{
    public class Grid3D : IEnumerable
    {
        public int Width;
        public int Height;
        public int Depth;

        public Vector3 CellSpacing;
        public float CellSize;

        int[,,] _grid;

        public Grid3D(int width, int height, int depth, float cellSize, Vector3 cellSpacing)
        {
            Width = width;
            Height = height;
            Depth = depth;
            CellSize = cellSize;
            CellSpacing = cellSpacing;

            _grid = new int[height, width, depth];
        }

        public int GetValue(int x, int y, int z)
        {
            return _grid[y, x, z];
        }

        public void SetValue(int x, int y, int z, int value)
        {
            _grid[y, x, z] = value;
        }
        
        public Vector3 GetWorldPosition(int x, int y, int z)
        {
            return new Vector3(
                x * (CellSize + CellSpacing.x),
                y * (CellSize + CellSpacing.y),
                z * (CellSize + CellSpacing.z)
            );
        }

        public IEnumerator GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        yield return new Cell3D(x, y, z, _grid[y, x, z]);
                    }
                }
            }
        }
    }
}