using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public void OnClick_LoadArsenal()
    {
    
    SceneManager.LoadScene("Arsenal");
    }

    public void OnClick_Return()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnClick_Logout()
    {
        SceneManager.LoadScene("Login Example");
    }

      public void Exit()
    {
        // If running in the Unity Editor
        #if UNITY_EDITOR
        // Stop playing the scene in the Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // If running in a build, quit the application
        Application.Quit();
        #endif
    }
}
