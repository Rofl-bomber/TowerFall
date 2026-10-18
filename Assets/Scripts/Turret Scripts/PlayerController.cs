using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float sensitivityX = 0.2f;
    public float sensitivityY = 0.05f;
    public Turret turretGun;
    public Transform turrelBase;
    public Transform playerCamera;
    private float minX = -45;
    private float maxX = 0;
    private float minY = -135f;
    private float maxY = 135f;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Quaternion initialRotation;
    float fireDelay;
    float nextFireTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = turrelBase.localRotation;
        fireDelay = 60f / turretGun.fireRate;
        nextFireTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Mouse.current.delta.ReadValue();

        float mouseX = mouse.x * sensitivityX;
        float mouseY = mouse.y * sensitivityY;
        rotationY = Mathf.Clamp(rotationY + mouseX, minY, maxY);
        turrelBase.localRotation = initialRotation * Quaternion.Euler(0, rotationY, 0);
        rotationX = Mathf.Clamp(rotationX - mouseY, minX, maxX);
        turretGun.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        Turret turretComponent = turretGun.GetComponent<Turret>();
        playerCamera.LookAt(
            turretGun.transform.position + turretGun.transform.forward * turretComponent.convergenceDistance
        );
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireDelay;
            Debug.Log("Shoot");
            turretGun.Shoot();
        }
    }
}
