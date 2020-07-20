using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {

	public void StartExperimental()
    {
        SceneManager.LoadScene("Game");
    }

    public void EditExperimental()
    {
        SceneManager.LoadScene("Editor");
    }

    public void ExitExperimental()
    {
        Application.Quit();
    }
}
