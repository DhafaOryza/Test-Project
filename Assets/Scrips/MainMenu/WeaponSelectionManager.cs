using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectionManager : MonoBehaviour
{
    public static WeaponSelectionManager Instance { get; private set; }

    [Header("Selection Data")]
    public int selectedWeaponIndex = 0;

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "Game";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SelectWeapon(int weaponIndex)
    {
        selectedWeaponIndex = weaponIndex;
        Debug.Log("Senjata terkunci di memori: Index " + selectedWeaponIndex);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}