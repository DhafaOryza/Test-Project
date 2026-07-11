using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject GameMenuPanelUI;
    private bool AlreadyOpen;

    void Start()
    {
        if(GameMenuPanelUI != null)
        {
            GameMenuPanelUI.gameObject.SetActive(false);
        }
        else
        {
            return;
        }
        
        AlreadyOpen = false;
        Time.timeScale = 1f;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (AlreadyOpen)
            {
                return;
            }
            else
            {
               OpenMenuUI(); 
            }
            
        }
    }

    public void OpenMenuUI()
    {
        GameMenuPanelUI.gameObject.SetActive(true);
        AlreadyOpen = true;

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        GameMenuPanelUI.gameObject.SetActive(false);
        AlreadyOpen = false;

        Time.timeScale = 1f;
    }
    public void GoToScene(string nameScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nameScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}