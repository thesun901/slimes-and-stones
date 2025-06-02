using UnityEngine;

public class BoomerangSpinning : MonoBehaviour
{
    public float rotationSpeed = 520f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
