using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public AudioSource audi;
    public AudioClip click;
    public AudioClip EnterSelection;
public void GotoFirstLevel()
    {
        audi.clip = click;
        audi.Play();
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        audi.clip = click;
        audi.Play();
        Application.Quit();   
    }
    public void SFXMENU()
    {
        audi.clip = EnterSelection;
        audi.Play();
    }
}
