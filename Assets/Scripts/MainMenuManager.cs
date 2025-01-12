using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnLevel1_ButtonPressed()
    {
        SceneManager.LoadScene("level1_22");
        Time.timeScale = 1.0f;
    }

    public void OnLevel2_ButtonPressed()
    {
        SceneManager.LoadScene("level2_22");
        Time.timeScale = 1.0f;
    }

    public void OnExitToDesktop_ButtonPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
       
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
