using UnityEngine;

public class HeroAnimationScripts : MonoBehaviour
{
    public PlayerController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEventThrow()
    {
        controller.ThrowBoomerang();
    }
}
