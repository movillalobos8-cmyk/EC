using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Lvl1()
    {
        SceneManager.LoadScene(0);
    }

    public void Introduccion()
    {
        SceneManager.LoadScene(2);
    }

     public void Menu()
    {
        SceneManager.LoadScene(1);
    }

     public void Lvl2()
    {
        SceneManager.LoadScene(5);
    }
}