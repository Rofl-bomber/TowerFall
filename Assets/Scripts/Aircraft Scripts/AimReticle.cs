using UnityEngine;

public class AimReticle : MonoBehaviour
{
    [SerializeField] float scaleCoeff = 0.02f;
    Vector3 baseScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(Vector3 cshPos, Vector3 playerPos)
    {
        transform.position = cshPos;
        transform.rotation = Quaternion.LookRotation(playerPos - cshPos);
        transform.localScale = Vector3.one * Vector3.Distance(cshPos, playerPos) * scaleCoeff;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
