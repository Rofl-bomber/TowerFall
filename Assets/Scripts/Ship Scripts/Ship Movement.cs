using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float pitch = Mathf.Sin(Time.time * 1.5f) * 2f;
        float roll = Mathf.Sin(Time.time * 0.8f) * 2f;

        transform.localRotation = Quaternion.Euler(pitch, 0, roll);
    }
}
