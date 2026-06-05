using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Main-Menu_SCENE")
            {
                Debug.Log("Geme closed!");
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene("Main-Menu_SCENE");
            }
        }
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main-Menu_SCENE");
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Geme close");
    }
}