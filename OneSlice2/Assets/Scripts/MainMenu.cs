using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool gameOver = false;
    //[SerializeField] public GameObject gameOverMenu;


    void Awake()
    {

    }

    void Update()
    {

    }

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSlice");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSlice");
        Application.Quit();
    }


    public void ResetGame()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSlice");
        SceneManager.LoadScene("GameScene"); //test scene
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<AudioManager>().Play("ButtonSlice");
        SceneManager.LoadScene("_MainMenu"); //test scene
    }

}
