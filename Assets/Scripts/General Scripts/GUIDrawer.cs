using UnityEngine;

public class GUIDrawer : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Projectile projectile;
    Transform player;
    [SerializeField] float reticleAngle = 4f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraManager = GetComponent<CameraManager>();
        player = cameraManager.GetCurrentCamera().transform;
    }

    // Update is called once per frame
    void Update()
    {
        player = cameraManager.GetCurrentCamera().transform;
        foreach(AircraftBase aircraft in gameManager.aircraftEnemies.Keys)
            {
                Vector3 crosshairPos = aircraft.GetLeadPoint(
                    player.position,
                    aircraft.transform.position,
                    aircraft.moveSpeed * aircraft.transform.forward,
                    projectile.velocity
                );
                Vector3 dir = crosshairPos - aircraft.transform.position;
                float d = Vector3.Distance(aircraft.transform.position, player.position);
                if (RayHitsCylinder(
                    player.position,
                    player.forward,
                    2 * aircraft.transform.position - crosshairPos,
                    crosshairPos + dir * 0.25f,
                    d * Mathf.Tan(reticleAngle * Mathf.Deg2Rad)
                ))
                {
                    aircraft.aimReticle.Show(crosshairPos, player.position);
                }
                else
                {
                    aircraft.aimReticle.Hide();
                }
        }
        foreach(AttackEntryPoint p in gameManager.AttackEntryPoints)
        {
            Debug.DrawRay(
                p.transform.position,
                p.transform.forward * 50,
                Color.green
            );
        }
    }
    bool RayHitsCylinder(
    Vector3 rayOrigin,
    Vector3 rayDirection,
    Vector3 a,
    Vector3 b,
    float radius)
    {
        Vector3 axis = b - a;
        float length = axis.magnitude;
        Vector3 axisDir = axis / length;

        Vector3 w0 = rayOrigin - a;

        float a1 = Vector3.Dot(rayDirection, rayDirection);
        float b1 = Vector3.Dot(rayDirection, axisDir);
        float c1 = Vector3.Dot(axisDir, axisDir);
        float d1 = Vector3.Dot(rayDirection, w0);
        float e1 = Vector3.Dot(axisDir, w0);

        float denom = a1 * c1 - b1 * b1;

        if (Mathf.Abs(denom) < 0.0001f)
            return false; // параллельны

        float t = (b1 * e1 - c1 * d1) / denom;
        float s = (a1 * e1 - b1 * d1) / denom;

        // луч только вперед
        if (t < 0)
            return false;

        // ограничение конечного цилиндра
        if (s < 0 || s > length)
            return false;

        Vector3 pRay = rayOrigin + rayDirection * t;
        Vector3 pCylinder = a + axisDir * s;

        return Vector3.Distance(pRay, pCylinder) <= radius;
    }
}
