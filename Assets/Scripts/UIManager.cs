using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider slider;
    public GameObject stopMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Play()
    {
        SceneManager.LoadScene("StoneCircleScene", LoadSceneMode.Single);


    }

    private void Awake()
    {
        GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("volume");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Adventure()
    {
        SceneManager.LoadScene("DungeonScene", LoadSceneMode.Single);
    }

    public void VolumeSet()
    {
        PlayerPrefs.SetFloat("volume", slider.value);
        GameObject.FindWithTag("MainCamera").GetComponent<AudioSource>().volume = slider.value;
        PlayerPrefs.Save();
    }

    public void ExitPauseMenu()
    {
        if(stopMenu != null)
        {
            stopMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape) && stopMenu != null)
        {
            stopMenu.SetActive(true);
        }
    }

    private void Start()
    {
        if (stopMenu != null)
        {
            stopMenu.SetActive(false);
        }
    }
}
