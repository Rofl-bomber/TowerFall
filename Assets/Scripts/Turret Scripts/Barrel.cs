using UnityEngine;
using UnityEngine.InputSystem;

public class Barrel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float fireRate = 200f;
    public GameObject projectilePrefab;
    public Transform turretPivot;
    public AudioSource audioSource;
    public AudioClip cannonShot;
    Transform firePoint;
    void Start()
    {
        firePoint = transform.Find("FirePoint");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void UpdateConvergence(float distance)
    {
        Vector3 targetPoint = turretPivot.position + turretPivot.forward * distance;
        transform.rotation = Quaternion.LookRotation(
                targetPoint - transform.position
        );
    }
    public void MakeShoot()
    {
        audioSource.PlayOneShot(cannonShot);
        GameObject shell = Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody rb = shell.GetComponent<Rigidbody>();
        Projectile pr = shell.GetComponent<Projectile>();
        rb.linearVelocity = firePoint.forward * pr.velocity;
    }

}
