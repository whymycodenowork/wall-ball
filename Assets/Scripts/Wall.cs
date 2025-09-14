using UnityEngine;

/// <summary>
/// Generates a curved wall segment with adjustable radius, thickness, and angle.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class Wall : MonoBehaviour
{
    // ------------ unity stuff ------------
    [Header("Arc Settings")]
    [Min(0.01f)] public float radius = 2f;
    [Min(0.01f)] public float thickness = 0.5f;
    [Range(10f, 359f)] public float angle = 90f;
    [SerializeField, Range(3, 64)] private int smoothnessMultiplier = 16;
    private int Smoothness => Mathf.Clamp(Mathf.CeilToInt(angle / 360f * smoothnessMultiplier), 3, 64);

    private float _lastRadius;
    private float _lastThickness;
    private float _lastAngle;

    private MeshFilter meshFilter;
    private PolygonCollider2D polyCollider;
    private bool dirty = true;
    private Mesh mesh;

    [SerializeField] private Transform parent; // the parent transform to rotate around

    public float rotationSpeed = 180;
    private Quaternion rotation;

    void Awake()
    {
        mesh = new();
        meshFilter = GetComponent<MeshFilter>();
        polyCollider = GetComponent<PolygonCollider2D>();
        parent = transform.parent;
        // temp for testing
        ProjectilePool.SpawnProjectile<Projectiles.Balls.BasicBall>(Vector2.zero, Vector2.right * 5f);
    }

    void Update()
    {
        if (radius != _lastRadius || thickness != _lastThickness || angle != _lastAngle)
        {
            _lastRadius = radius;
            _lastThickness = thickness;
            _lastAngle = angle;
            dirty = true;
        }

        if (dirty)
        {
            Rebuild();
            dirty = false;
        }
        Quaternion targetRotation = parent.rotation;

        // Smoothly rotate this object towards parent's rotation
        rotation = Quaternion.RotateTowards(
            rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.rotation = rotation;

        WallUpdate();
    }

    void Rebuild()
    {
        int pointsCount = Smoothness + 1;
        Vector2[] outerArc = new Vector2[pointsCount];
        Vector2[] innerArc = new Vector2[pointsCount];

        float startAngle = -angle * 0.5f;
        float step = angle / Smoothness;

        for (int i = 0; i < pointsCount; i++)
        {
            float a = (startAngle + step * i) * Mathf.Deg2Rad;
            Vector2 dir = new(Mathf.Cos(a), Mathf.Sin(a));

            outerArc[i] = dir * radius;
            innerArc[pointsCount - 1 - i] = dir * (radius - thickness);
        }

        Vector2[] colliderPoints = new Vector2[outerArc.Length + innerArc.Length];
        outerArc.CopyTo(colliderPoints, 0);
        innerArc.CopyTo(colliderPoints, outerArc.Length);

        colliderPoints = RemoveDuplicatePoints(colliderPoints);

        // Ensure CCW winding
        if (!IsCCW(colliderPoints))
            System.Array.Reverse(colliderPoints);

        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, colliderPoints);

        Vector3[] vertices = new Vector3[colliderPoints.Length];
        for (int i = 0; i < colliderPoints.Length; i++)
            vertices[i] = colliderPoints[i];

        Triangulator triangulator = new(colliderPoints);
        int[] triangles = triangulator.Triangulate();

        bool validTriangles = triangles.Length >= 3;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (triangles[i] < 0 || triangles[i] >= vertices.Length)
            {
                validTriangles = false;
                Debug.LogError($"Invalid triangle index {triangles[i]} for vertices length {vertices.Length}");
                break;
            }
        }

        if (vertices.Length >= 3 && validTriangles)
        {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh; // Reassign updated mesh
        }
        else
        {
            meshFilter.sharedMesh = null;
            Debug.LogWarning("Mesh not assigned: invalid vertices or triangles.");
        }
    }

    // Utility to remove all duplicate points
    Vector2[] RemoveDuplicatePoints(Vector2[] points)
    {
        var unique = new System.Collections.Generic.List<Vector2>();
        foreach (var pt in points)
        {
            bool found = false;
            foreach (var u in unique)
            {
                if ((pt - u).sqrMagnitude < 1e-6f)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                unique.Add(pt);
        }
        return unique.ToArray();
    }

    // Utility to check CCW winding
    bool IsCCW(Vector2[] points)
    {
        float sum = 0f;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % points.Length];
            sum += (b.x - a.x) * (b.y + a.y);
        }
        return sum < 0f;
    }

    // ------------ wall logic & stuff ------------

    private WallBase _wallBase;

    public WallBase WallLogic
    {
        get => _wallBase;
        set
        {
            _wallBase = value;
            _wallBase.wallComp = this;
        }
    }

    private void WallUpdate()
    {
        WallLogic?.OnUpdate();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Player player))
        {
            _wallBase?.OnPlayerCollision(player);
        }
        else if (collision.collider.TryGetComponent(out Wall wall))
        {
            _wallBase?.OnWallCollision(wall);
        }
        else if (collision.collider.TryGetComponent(out Projectile projectile))
        {
            _wallBase?.OnProjectileCollision(projectile);
        }
    }
}

/// <summary>
/// Simple polygon triangulator (ear clipping).
/// </summary>
public class Triangulator
{
    private readonly Vector2[] m_points;

    public Triangulator(Vector2[] points)
    {
        m_points = points;
    }

    public int[] Triangulate()
    {
        var indices = new System.Collections.Generic.List<int>();

        int n = m_points.Length;
        if (n < 3) return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0) { for (int v = 0; v < n; v++) V[v] = v; }
        else { for (int v = 0; v < n; v++) V[v] = (n - 1) - v; }

        int nv = n;
        int count = 2 * nv;
        for (int v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0) break;

            int u = v; if (nv <= u) u = 0;
            v = u + 1; if (nv <= v) v = 0;
            int w = v + 1; if (nv <= w) w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a = V[u], b = V[v], c = V[w];
                indices.Add(a); indices.Add(b); indices.Add(c);
                for (int s = v, t = v + 1; t < nv; s++, t++) V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        return indices.ToArray();
    }

    private float Area()
    {
        int n = m_points.Length;
        float A = 0;
        for (int p = n - 1, q = 0; q < n; p = q++)
            A += (m_points[p].x * m_points[q].y) - (m_points[q].x * m_points[p].y);
        return A * 0.5f;
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        Vector2 A = m_points[V[u]];
        Vector2 B = m_points[V[v]];
        Vector2 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x)))) return false;
        for (int p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w)) continue;
            Vector2 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P)) return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax = C.x - B.x, ay = C.y - B.y;
        float bx = A.x - C.x, by = A.y - C.y;
        float cx = B.x - A.x, cy = B.y - A.y;
        float apx = P.x - A.x, apy = P.y - A.y;
        float bpx = P.x - B.x, bpy = P.y - B.y;
        float cpx = P.x - C.x, cpy = P.y - C.y;

        float aCROSSbp = ax * bpy - ay * bpx;
        float cCROSSap = cx * apy - cy * apx;
        float bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0) && (bCROSScp >= 0) && (cCROSSap >= 0));
    }
}