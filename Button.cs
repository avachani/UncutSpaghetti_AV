using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{

    public void MainButton()
    {
        SceneManager.LoadScene("Play");
    }

    public void CheckButton()
    {
        SceneManager.LoadScene("Check");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("Menue");
    }

    public void InstructionsButton()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void InstructionsButton2()
    {
        SceneManager.LoadScene("Instructions2");
    }

    public void InstructionsButton3()
    {
        SceneManager.LoadScene("Instructions3");
    }

    public void InstructionsButton4()
    {
        SceneManager.LoadScene("Instructions4");
    }
}
