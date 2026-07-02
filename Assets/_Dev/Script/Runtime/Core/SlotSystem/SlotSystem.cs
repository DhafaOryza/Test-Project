using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using LumineREx.Utils.Grid;
using LumineREx.Utils.RNG;
using UnityEngine;
using Random = System.Random;

namespace _Dev.Script.Runtime.Core.SlotSystem
{
    public class SlotSystem : MonoBehaviour
    {
        private RNG _rng;
        
        private Grid2D<int> _grid;
        
        private List<Vector2Int[]> _paylines;
        private List<Vector2Int[]> _winningLines = new();

        private void Awake()
        {
            _grid = new Grid2D<int>(3, 3, 1);
            _rng = new RNG();
            CreatePaylines();
        }

        private void Start()
        {
            Roll();
        }

        public void Roll()
        {
            FillGridTest();
            Evaluate();
            PrintBoard();
        }


        private void CreatePaylines()
        {
            _paylines = new List<Vector2Int[]>
            {
                // bawah
                new[]
                {
                    new Vector2Int(0,0),
                    new Vector2Int(1,0),
                    new Vector2Int(2,0)
                },

                // tengah
                new[]
                {
                    new Vector2Int(0,1),
                    new Vector2Int(1,1),
                    new Vector2Int(2,1)
                },

                // atas
                new[]
                {
                    new Vector2Int(0,2),
                    new Vector2Int(1,2),
                    new Vector2Int(2,2)
                },

                // diagonal kiri
                new[]
                {
                    new Vector2Int(0,0),
                    new Vector2Int(1,1),
                    new Vector2Int(2,2)
                },

                // diagonal kanan
                new[]
                {
                    new Vector2Int(2,0),
                    new Vector2Int(1,1),
                    new Vector2Int(0,2)
                }
            };
        }
        
        private bool CheckLine(Vector2Int[] line)
        {
            int first = _grid.GetValue(line[0].x, line[0].y);

            foreach (var pos in line)
            {
                int current = _grid.GetValue(pos.x,pos.y);

                if (current != first)
                    return false;
            }

            return true;
        }
        
        private void Evaluate()
        {
            _winningLines.Clear();
            
            foreach(var line in _paylines)
            {
                if(CheckLine(line))
                {
                    Debug.Log("WIN");
                    _winningLines.Add(line);

                }
            }
        }

        private void FillGridTest()
        {
            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    _grid.SetValue(x, y, _rng.Roll(1, 5));
                }
            }
        }
        
        private void PrintBoard()
        {
            string result = "";

            for (int y = _grid.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    result += _grid.GetValue(x, y) + " ";
                }

                result += "\n";
            }

            Debug.Log(result);
        }
        
        private void OnDrawGizmos()
        {
            //Grid
            if (_grid == null)
                return;

            float size = 1f;

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    Vector3 pos = new Vector3(x * size, y * size, 0);

                    Gizmos.color = Color.white;

                    Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                }
            }
            
            
            // // Paylines
            // if (_paylines == null)
            //     return;
            //
            // Gizmos.color = Color.green;
            //
            // foreach(var line in _paylines)
            // {
            //     for(int i = 0; i < line.Length - 1; i++)
            //     {
            //         Vector3 a = new Vector3(line[i].x, line[i].y, 0);
            //
            //         Vector3 b = new Vector3(line[i+1].x, line[i+1].y, 0);
            //
            //         Gizmos.DrawLine(a,b);
            //     }
            // }
            
            foreach(var line in _winningLines)
            {
                Gizmos.color = Color.yellow;

                for(int i = 0; i < line.Length - 1; i++)
                {
                    Gizmos.DrawLine(
                        new Vector3(line[i].x,line[i].y),
                        new Vector3(line[i+1].x,line[i+1].y)
                    );
                }
            }
            
        }
        
    }
}