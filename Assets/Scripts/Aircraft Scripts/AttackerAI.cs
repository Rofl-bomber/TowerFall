using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.InputSystem;

public class AttackerAI: AircraftBase
{
    [SerializeField] Bomb bombPrefab;
    public AudioSource audioSource;
    public AudioClip clip;
    protected override void Approach()
    {
        Debug.Log("Approach");
        moveSpeed = approachSpeed;
        if (currentT >= 0.9f) moveSpeed = approachSpeed * (1f - currentT) / 0.1f + attackSpeed *  (1 - (1 - currentT) / 0.1f);
        if (!Move(path, moveSpeed))
        {
            currentT = 0f;
            currentAircraftState = AirCraftState.Attack;
            Pose startPoint = new Pose(transform.position, transform.rotation);
            Pose endPoint = new();
            endPoint.position = startPoint.position + 2f * Vector3.Dot(target.position - startPoint.position, target.forward) * target.forward + Vector3.up * 20f;
            endPoint.rotation = Quaternion.LookRotation(endPoint.position - target.position);
            DropBomb();
            path = CreateAttackPath(startPoint, endPoint, attackSpeed);
        }
    }
    protected override void Attack()
    {
        audioSource.PlayOneShot(clip);
        Debug.Log("Attack");
        moveSpeed = attackSpeed;
        if (currentT >= 0.9f) moveSpeed = attackSpeed * (1f - currentT) / 0.1f + escapeSpeed *  (1 - (1 - currentT) / 0.1f);
        if (!Move(path, moveSpeed))
        {
            currentT = 0f;
            currentAircraftState = AirCraftState.Escape;
            Pose startPoint = new Pose(transform.position, transform.rotation);
            float minRadius = 200, maxRadius = 250f, minHeight = 40f, maxHeight = 50f;
            Vector2 onCircle = Random.onUnitCircle;
            float radius = Random.Range(minRadius, maxRadius);
            float height = Random.Range(minHeight, maxHeight);
            Vector3 endPointPos = new (onCircle.x * radius, height, onCircle.y * radius);
            Quaternion endPointRot = Quaternion.LookRotation(target.position - endPointPos);
            Pose endPoint = new Pose(endPointPos, endPointRot);
            path = CreateApproachPath(startPoint, endPoint, escapeSpeed);
        }
    }
    protected override void Escape()
    {
        moveSpeed = escapeSpeed;
        if (currentT >= 0.9f) moveSpeed = escapeSpeed * (1f - currentT) / 0.1f + approachSpeed *  (1 - (1 - currentT) / 0.1f);
        if (!Move(path, moveSpeed))
        {
            currentT = 0f;
            currentAircraftState = AirCraftState.Approach;
            Transform startPoint = transform;
            Transform endPoint = gameManager.AttackEntryPoints[Random.Range(0, gameManager.AttackEntryPoints.Length)].transform;
            path = CreateApproachPath(
                new Pose(startPoint.position, startPoint.rotation),
                new Pose(endPoint.position, endPoint.rotation),
                approachSpeed
            );
        }
    }
    void Start()
    {
        currentAircraftState = AirCraftState.Approach;

    }
    void Update()
    {
        
        switch(currentAircraftState)
        {
            case AirCraftState.Approach:
            Approach();
            break;
            case AirCraftState.Attack:
            Attack();
            break;
            case AirCraftState.Escape:
            Escape();
            break;
        }
    }
    void DropBomb()
    {
        Debug.Log("Drop bomb");
        Bomb bomb = Instantiate(
            bombPrefab,
            transform.position - Vector3.up,
            transform.rotation
        );
        bomb.GetComponent<Rigidbody>().linearVelocity = attackSpeed * 0.7f * transform.forward - transform.forward * 5f;
    }
    Spline CreateAttackPath(Pose startPoint, Pose endPoint, float moveSpeed)
    {
        Spline path = new Spline();
        Vector3 P0 = startPoint.position;
        
        Vector3 P3 = endPoint.position;
        Renderer r = target.GetComponentInChildren<Renderer>();
        Vector3 center = r.bounds.center;
        Vector3 size = r.bounds.size;
        Vector3 P1 = center + Vector3.up * size.y * 2f;
        Vector3 P2 = SmoothPath(P0, P1, P3, 0.05f);
        Vector3[] points = {P0, P1, P2, P3};
        foreach(Vector3 point in points)
        {
            path.Add(new BezierKnot(point));
        }
        path.SetTangentMode(TangentMode.AutoSmooth);
        BezierKnot startKnot = path[0];
        startKnot.TangentOut = startPoint.forward * moveSpeed;
        path[0] = startKnot;

        BezierKnot endKnot = path[^1];
        endKnot.TangentIn = endPoint.forward * moveSpeed;
        path[^1] = endKnot;
        return path;
    }
    public override Spline CreateApproachPath(Pose startPoint, Pose endPoint, float moveSpeed)
    {
        Spline path = new Spline();
        Vector3 P0 = startPoint.position;
        Vector3 P3 = endPoint.position;
        float d = Vector3.Distance(P0, P3);
        Vector3 P1;

        BezierKnot startKnot = new BezierKnot(P0, Vector3.zero, startPoint.forward * d * 0.45f);
        BezierKnot endKnot = new BezierKnot(P3, endPoint.forward * d * 0.45f, endPoint.forward * d * 0.45f);

        path.Add(startKnot);
        if (Vector3.Distance(P0, P3) > 70f)
        {
            Vector3 n = Vector3.Cross(startPoint.forward, endPoint.forward).normalized;
            Vector3 v = Vector3.Cross(P3 - P0, n) * 0.86f;
            P1 = (P0 + P3) * 0.5f + v;
            if (Vector3.Dot(startPoint.forward, (P3 - P1) * 0.5f + v) > Vector3.Dot(startPoint.forward, P1 - P0))
            {
                P1 += -2f * v;
            }
            BezierKnot P1Knot =  new BezierKnot(P1, (P3 - P0) * 0.45f, (P3 - P0) * 0.45f);
            path.Add(P1Knot);
        }
        path.Add(endKnot);
        path.SetTangentMode(TangentMode.Mirrored); 
        return path;
    }
    Vector3 SmoothPath(Vector3 P3, Vector3 P2, Vector3 P0, float height)
    {
        Vector3 line = (P2 - P0).normalized;

        // направление в плоскости, перпендикулярное P0P2
        Vector3 side = Vector3.Cross(
            Vector3.Cross(line, P3 - P0),
            line
        ).normalized;

        Vector3 mid = (P0 + P2) * 0.5f;

        if (Vector3.Dot(side, P3 - mid) > 0)
            side = -side;

        return mid + side * height * Vector3.Distance(P0, P2);
    }
    
}