using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadResetGame()
    {
        //fader.gameObject.SetActive(true);
        //LeanTween.scale(fader, Vector3.zero, 0f);
        //LeanTween.scale(fader, new Vector3(1, 1, 1), 0.5f).setOnComplete(() =>
        // {
        //FindObjectOfType<AudioManager>().Play("ButtonClick");
        SceneManager.LoadScene("GameScene"); //test scene
                                             //Invoke("LoadGame", 0.5f);
                                             // });
    }

    public void QuitGame()
    {
        //FindObjectOfType<AudioManager>().Play("ButtonClick");
        Application.Quit();
    }
}
