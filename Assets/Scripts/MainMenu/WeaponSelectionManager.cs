using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponSelectionManager : MonoBehaviour
{
    [Header("Selection Data")]
    public int selectedWeaponIndex = 0;

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "Game";
    
    public void SelectWeapon(int weaponIndex)
    {
        selectedWeaponIndex = weaponIndex;
        
        if (GameSession.Instance != null)
        {
            GameSession.Instance.selectedWeaponIndex = weaponIndex;
        }

        Debug.Log("Senjata terkunci di memori GameSession: Index " + weaponIndex);
    }

    public void StartGame()
    {
        // PERBAIKAN: Gunakan LoadLevel milik GameSession agar transisinya berjalan!
        if (GameSession.Instance != null)
        {
            GameSession.Instance.LoadLevel(gameplaySceneName);
        }
        else
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}