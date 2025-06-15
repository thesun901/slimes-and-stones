using UnityEngine;

public class OpenUIWhenPlayerClose : MonoBehaviour
{
    public GameObject UIElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIElement.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            UIElement.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIElement.SetActive(false);
        }
    }
}
