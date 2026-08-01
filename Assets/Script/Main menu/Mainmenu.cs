using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour
{
    public void LoadScene(string scenceName)
    {
        SceneManager.LoadScene(scenceName);
    }
    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu");

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
