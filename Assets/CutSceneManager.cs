using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    int step = 0;
    public List<string> textes = new List<string>();

    public void nextStep()
    {
        step = step + 1;
        Debug.Log(step);
        if (step < textes.Count)
        {
            tmp.text = textes[step];
        }
        else
        {
            Skip();
        }
    }

    public void Skip()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
