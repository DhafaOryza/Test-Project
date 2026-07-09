
using System.Collections.Generic;
using _01_Scripts.Runtime.Core.Character.Ally;
using DG.Tweening;
using LumineREx.Utils.Grid;
using LumineREx.Utils.RNG;
using UnityEngine;
using Random = System.Random;

namespace _01_Scripts.Runtime.Core.SlotSystem
{
    public class SlotSystem : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField]
        private float spinTime;
        
        [SerializeField] 
        private Reel[] _reels;
        
        private RNG _rng;
        
        private Grid2D<int> _grid;
        
        private List<Vector2Int[]> _paylines;
        private List<Vector2Int[]> _winningLines = new();

        [SerializeField]
        private int reelsCounter = 0;
        [SerializeField]
        private List<int> listOfWinnners = new List<int>();

        private void Awake()
        {
            _grid = new Grid2D<int>(3, 3, 1);
            _rng = new RNG();
            CreatePaylines();
        }

        private void OnEnable()
        {
            foreach (var reel in _reels) reel.Finished += FinishedRolling;
        }

        private void OnDisable()
        {
            foreach (var reel in _reels) reel.Finished -= FinishedRolling;
        }

        public void Roll()
        {
            
            
            FillGridTest();
            Evaluate();
            PrintBoard();
            // UpdateReels();
            SpinReels();
            
        }


        private void CreatePaylines()
        {
            _paylines = new List<Vector2Int[]>
            {
                //Atas
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

                //Bawah
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
                },
                
                //Vertical Kiri
                new[]
                {
                new Vector2Int(0,0),
                new Vector2Int(0,1),
                new Vector2Int(0,2)
                },
                
                //Vertical Tengah
                new[]
                {
                    new Vector2Int(1,0),
                    new Vector2Int(1,1),
                    new Vector2Int(1,2)
                },
                
                //Vertical Kanan
                new[]
                {
                    new Vector2Int(2,0),
                    new Vector2Int(2,1),
                    new Vector2Int(2,2)
                },
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
            listOfWinnners.Clear();
            
            foreach(var line in _paylines)
            {
                if(CheckLine(line))
                {
                    int symbol = _grid.GetValue(line[0].x, line[0].y);
                    Debug.Log($"WIN : {symbol}");
                    listOfWinnners.Add(symbol);
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

            for (int y = 0; y < _grid.Height; y++)
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
            if (_grid == null)
                return;

            float size = 1f;

            for (int y = 0; y < _grid.Height; y++)
            {
                for (int x = 0; x < _grid.Width; x++)
                {
                    Vector3 pos = new Vector3(x * size, (_grid.Height - 1 - y) * size, 0);

                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                }
            }

            foreach (var line in _winningLines)
            {
                Gizmos.color = Color.yellow;

                for (int i = 0; i < line.Length - 1; i++)
                {
                    Vector3 a = new Vector3(line[i].x, _grid.Height - 1 - line[i].y);
                    Vector3 b = new Vector3(line[i + 1].x, _grid.Height - 1 - line[i + 1].y);
                    Gizmos.DrawLine(a, b);
                }
            }
        }

        private void SpinReels()
        {
            foreach (var reel in _reels)
            {
                reel.Spin();
            }

            DOVirtual.DelayedCall(spinTime, () =>
            {
                for(int x = 0; x < 3; x++)
                {
                    int[] result = new int[3];

                    for(int y = 0; y < 3; y++)
                    {
                        result[y] = _grid.GetValue(x,y);
                    }

                    _reels[x].Stop(result);
                }
            });
        }

        private void FinishedRolling()
        {
            reelsCounter++;

            if (reelsCounter >= 3)
            {
                foreach (var symbol in listOfWinnners)    
                {
                    AlliesManager.Instance.AddCharacter(symbol);
                }
                
                reelsCounter = 0;
                
                Debug.Log("Kepanggil sekali");
            }
        }
    }
}