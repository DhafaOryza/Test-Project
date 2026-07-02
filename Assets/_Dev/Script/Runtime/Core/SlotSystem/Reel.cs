using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace _Dev.Script.Runtime.Core.SlotSystem
{
    public class Reel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<SymbolView> symbols;
        [SerializeField] private List<Sprite> sprites;

        [Header("Settings")]
        [SerializeField] private float startSpeed = 10f;
        [SerializeField] private float stopSpeed = 2f;
        [SerializeField] private float deceleration = 25f;
        [SerializeField] private float spacing = 1.5f;
        [SerializeField] private float recycleY = -2f;

        private ReelState state = ReelState.Idle;

        private int[] targetResult;
        private int stopIndex;

        private float speed;

        public bool IsSpinning =>
            state != ReelState.Idle;

        private void Update()
        {
            if (state == ReelState.Idle)
                return;

            UpdateSpeed();
            MoveSymbols();
        }

        public void Spin()
        {
            speed = startSpeed;

            stopIndex = 0;
            targetResult = null;

            state = ReelState.Spinning;
        }

        public void Stop(int[] result)
        {
            if (result == null || result.Length == 0)
                return;

            targetResult = result;

            stopIndex = 0;

            state = ReelState.Stopping;
        }

        private void UpdateSpeed()
        {
            if (state != ReelState.Stopping)
                return;

            speed -= deceleration * Time.deltaTime;

            speed = Mathf.Max(speed, stopSpeed);
        }

        private void MoveSymbols()
        {
            foreach (var symbol in symbols)
            {
                symbol.transform.localPosition += Vector3.down * (speed * Time.deltaTime);

                if (symbol.transform.localPosition.y < recycleY)
                {
                    RecycleSymbol(symbol);
                }
            }
        }

        private void RecycleSymbol(SymbolView symbol)
        {
            float highestY = GetHighestSymbolY();

            symbol.transform.localPosition = new Vector3(0, highestY + spacing, 0);

            UpdateSymbol(symbol);
        }

        private void UpdateSymbol(SymbolView symbol)
        {
            if (state == ReelState.Stopping)
            {
                symbol.SetSymbol(sprites[targetResult[stopIndex]]);

                stopIndex++;

                if (stopIndex >= targetResult.Length)
                {
                    FinishStop();
                }

                return;
            }

            symbol.SetSymbol(sprites[Random.Range(0, sprites.Count)]);
        }

        private float GetHighestSymbolY()
        {
            float highest = float.MinValue;

            foreach (var symbol in symbols)
            {
                highest = Mathf.Max(highest, symbol.transform.localPosition.y);
            }

            return highest;
        }

        private void FinishStop()
        {
            state = ReelState.Idle;

            speed = stopSpeed;

            SnapSymbols();

            transform.DOPunchPosition(
                Vector3.down * .1f,
                .15f);

            Debug.Log("Stopped");
        }

        private void SnapSymbols()
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                symbols[i].transform.localPosition = new Vector3(0, (1 - i) * spacing, 0);
            }
        }
    }
}