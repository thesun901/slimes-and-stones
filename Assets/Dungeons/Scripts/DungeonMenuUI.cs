using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonMenuUI : MonoBehaviour
{
    public static DungeonMenuUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        gameObject.SetActive(false);
    }

    public void ExitDungeon()
    {
        SceneManager.LoadScene("StoneCircleScene", LoadSceneMode.Single);
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }
}
