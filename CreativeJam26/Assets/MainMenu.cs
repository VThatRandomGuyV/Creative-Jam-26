using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelBase");
    }

    public void QuitGame()
    {
        Debug.Log("Quit application triggered.");
        Application.Quit();
    }
}
