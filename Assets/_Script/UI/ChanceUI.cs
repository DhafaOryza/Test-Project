using UnityEngine;
public class ChanceUI : MonoBehaviour
{
    [SerializeField] private GameObject[] chanceBlock;

    public void SetChanceUI(int remaining)
    {
        for (int i = 0 ; i < chanceBlock.Length ; i++)
        {
            chanceBlock[i].SetActive (i < remaining);
        }
    }
}