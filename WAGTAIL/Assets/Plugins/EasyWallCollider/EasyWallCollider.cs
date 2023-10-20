using System.Collections.Generic;
using System.Linq;
using PepijnWillekens.EasyWallColliderUnity;
using PepijnWillekens.Extensions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.AI;
#endif

#if UNITY_2018_3_OR_NEWER
[ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
public class EasyWallCollider : MonoBehaviour
{
    [HideInInspector] public Transform colliderContainer;

    [HideInInspector] public List<Transform> Corners = new List<Transform>();

    [Header("Settings")] public float Radius = 0.5f;

    public float Heigth = 2;
    public float Depth;
    public float ExtraWidth;
    public bool Invert;
    public bool Loop;

    public bool MakeRenderers;
    public bool IsStatic;

    public bool UseNavObstacle;


    public PhysicMaterial PhysicsMaterial;

    [Header("Gizmos")] public bool OnlyWhenSelected;

    public float GizmoLineInterval = 0.5f;

    public Color GizmoColor = Color.green;

    [Tooltip("콜라이더 객체 숨기기를 활성화하고 비활성화합니다.")]
    public bool NoHide;

    private List<Vector2> _cachedCornerList = new List<Vector2>();
    private List<Vector2> _cachedCornerListToOutset = new List<Vector2>();

    [HideInInspector] public int lastHash;

#if UNITY_EDITOR
    private Mesh _cubeMesh;
    private Mesh _cylinderMesh;
    private Material _defaultMaterial;


    private Mesh CubeMesh
    {
        get
        {
            if (_cubeMesh == null)
                FixPrimitiveRefs();

            return _cubeMesh;
        }
    }

    private Mesh CylinderMesh
    {
        get
        {
            if (_cylinderMesh == null)
                FixPrimitiveRefs();

            return _cylinderMesh;
        }
    }

    private Material DefaultMaterial
    {
        get
        {
            if (_defaultMaterial == null)
                FixPrimitiveRefs();

            return _defaultMaterial;
        }
    }

    private void FixPrimitiveRefs()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        _cubeMesh = cube.GetComponent<MeshFilter>().sharedMesh;
        _cylinderMesh = cylinder.GetComponent<MeshFilter>().sharedMesh;
        _defaultMaterial = cube.GetComponent<MeshRenderer>().sharedMaterial;

        if (Application.isPlaying)
        {
            Destroy(cube);
            Destroy(cylinder);
        }
        else
        {
            DestroyImmediate(cube);
            DestroyImmediate(cylinder);
        }
    }

    private int CalculateSettingsHash()
    {
        unchecked
        {
            int hashCode = 0;
            for (int i = 0; i < Corners.Count; i++)
            {
                if (Corners[i] != null)
                {
                    hashCode = (hashCode * 397) ^ Corners[i].localPosition.GetHashCode();
                }
            }

            hashCode = (hashCode * 397) ^ Radius.GetHashCode();
            hashCode = (hashCode * 397) ^ MakeRenderers.GetHashCode();
            hashCode = (hashCode * 397) ^ transform.childCount.GetHashCode();
            hashCode = (hashCode * 397) ^ Heigth.GetHashCode();
            hashCode = (hashCode * 397) ^ Depth.GetHashCode();
            hashCode = (hashCode * 397) ^ ExtraWidth.GetHashCode();
            hashCode = (hashCode * 397) ^ Invert.GetHashCode();
            hashCode = (hashCode * 397) ^ Loop.GetHashCode();
            hashCode = (hashCode * 397) ^ gameObject.layer.GetHashCode();
            hashCode = (hashCode * 397) ^ gameObject.isStatic.GetHashCode();
            hashCode = (hashCode * 397) ^ ((PhysicsMaterial) ? PhysicsMaterial.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ NoHide.GetHashCode();
            return hashCode;
        }
    }

    public int LoopInt()
    {
        return Loop ? 0 : -1;
    }

    private void Update()
    {
        if (Application.isPlaying) return;

        Vector3 lossyScale = transform.lossyScale;
        transform.localScale = Vector3.Scale(transform.localScale,
            new Vector3(1 / lossyScale.x, 1 / lossyScale.y, 1 / lossyScale.z));


        int newHash = CalculateSettingsHash();
        if (newHash != lastHash)
        {
            lastHash = newHash;

            Corners.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child != colliderContainer)
                {
                    Corners.Add(child);
                    if (CanBeEdited())
                    {
                        child.gameObject.name = Corners.Count.ToString();
                    }
                }
            }

            if (CanBeEdited())
                MakeColliders();
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child != colliderContainer)
                child.gameObject.hideFlags = NoHide || CanBeEdited() ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        ColliderContainer.hideFlags = NoHide ? HideFlags.None : HideFlags.HideInHierarchy;
    }

    private Transform ColliderContainer
    {
        get
        {
            if (colliderContainer == null)
            {
                colliderContainer = new GameObject("Colliders").transform;
                colliderContainer.SetParent(transform);
                colliderContainer.Reset();
            }

            return colliderContainer;
        }
    }

    private void MakeColliders()
    {
        ColliderContainer.DestroyAllChildren();

        for (int i = 0; i < Corners.Count; i++)
            Corners[i].localPosition = Corners[i].localPosition.ChangeY(0);

        if (Corners.Count >= 3)
            MakeAllColliders();
        else
        {
            for (int i = Corners.Count; i < 3; i++)
            {
                Transform newObj = new GameObject().transform;
                newObj.SetParent(transform);
                newObj.localPosition = Vector3.forward * (GizmoLineInterval * 5 * i);
                newObj.gameObject.name = i.ToString();
                PolygonColliderEditorExtention.DrawIcon(newObj.gameObject, 0);
            }
        }
    }

    private void MakeAllColliders()
    {
        MakeAllEdgeColliders(InSetCorners(FillCachedCornerList(Corners), Radius));

        if (Radius > 0)
            MakeAllCornerColliders(InSetCorners(FillCachedCornerList(Corners), Radius));
    }

    public bool CanBeEdited()
    {
#if UNITY_2018_3_OR_NEWER
        return PrefabUtility.GetPrefabInstanceStatus(gameObject) == PrefabInstanceStatus.NotAPrefab;
#else
            return true;
#endif
    }

    private void MakeAllEdgeColliders(IReadOnlyList<Vector2> corners)
    {
        for (int i = 0; i < corners.Count + LoopInt(); i++)
            MakeEdgeCollider($"{i + 1} - {(i + 1) % corners.Count + 1}", corners[i],
                corners[(i + 1) % corners.Count], Radius, Heigth + Depth, Depth, ExtraWidth);
    }

    private void MakeAllCornerColliders(IReadOnlyList<Vector2> corners)
    {
        for (int i = 0; i < corners.Count; i++)
            MakeCornerCollider($"{i + 1}", corners[i], Radius, Heigth + Depth, Depth);
    }

    private void MakeEdgeCollider(string name, Vector2 from, Vector2 to, float radius, float heigth, float yOffset,
        float extraWidth)
    {
        GameObject go = new GameObject("EdgeCollider " + name)
        {
            tag = "Wall",
            layer = gameObject.layer
        };

        if (IsStatic)
            go.isStatic = true;
        else
            go.isStatic = gameObject.isStatic;

        go.transform.SetParent(ColliderContainer);
        go.AddComponent<BoxCollider>().sharedMaterial = PhysicsMaterial;
        if (MakeRenderers)
        {
            go.AddComponent<MeshFilter>().sharedMesh = CubeMesh;
            go.AddComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;
        }

        if (UseNavObstacle)
            go.AddComponent<NavMeshObstacle>().shape = NavMeshObstacleShape.Box;

        go.transform.localPosition = To3D((from + to) / 2 + Perp(from, to, true) * extraWidth / 2)
            .ChangeY(heigth / 2 - yOffset);
        go.transform.rotation = Quaternion.LookRotation(transform.TransformDirection(To3D((to - from).normalized)),
            transform.TransformDirection(Vector3.up));
        go.transform.localScale = new Vector3(radius * 2 + extraWidth, heigth, Vector2.Distance(from, to));
    }

    private void MakeCornerCollider(string name, Vector2 current, float radius, float heigth, float yOffset)
    {
        GameObject go = new GameObject("CornerCollider " + name);
        go.layer = gameObject.layer;
        go.tag = "Wall";

        if (IsStatic)
            go.isStatic = true;
        else
            go.isStatic = gameObject.isStatic;

        go.transform.SetParent(ColliderContainer);
        CapsuleCollider collider = go.AddComponent<CapsuleCollider>();
        go.transform.localPosition = To3D(current).ChangeY(heigth / 2 - yOffset);
        collider.radius = radius;
        collider.height = heigth + radius * 2;
        collider.sharedMaterial = PhysicsMaterial;
        go.transform.localRotation = Quaternion.identity;

        if (UseNavObstacle)
        {
            NavMeshObstacle obstacle = go.AddComponent<NavMeshObstacle>();
            obstacle.shape = NavMeshObstacleShape.Capsule;
            obstacle.radius = radius;
            obstacle.height = heigth + radius * 2;
        }

        if (MakeRenderers)
        {
            GameObject r = new GameObject("CornerCollider");
            r.layer = gameObject.layer;
            r.tag = gameObject.tag;
            r.isStatic = gameObject.isStatic;
            r.transform.SetParent(ColliderContainer);
            r.AddComponent<MeshFilter>().sharedMesh = CylinderMesh;
            r.AddComponent<MeshRenderer>().sharedMaterial = DefaultMaterial;
            r.transform.localPosition = To3D(current).ChangeY(heigth / 2 - yOffset);
            r.transform.localScale = new Vector3(radius * 2, heigth / 2, radius * 2);
            r.transform.localRotation = Quaternion.identity;
        }
    }

    private List<Vector2> FillCachedCornerList(List<Transform> corners)
    {
        _cachedCornerList.Clear();
        _cachedCornerList.AddRange(corners.Select((e) => To2D(e.localPosition)));

        if (Invert)
            _cachedCornerList.Reverse();

        return _cachedCornerList;
    }

    private List<Vector2> InSetCorners(List<Vector2> corners, float radius)
    {
        _cachedCornerListToOutset.Clear();
        for (int i = 0; i < corners.Count; i++)
        {
            Vector2 prev = corners[(i - 1 + corners.Count) % corners.Count];
            Vector2 cur = corners[i];
            Vector2 next = corners[(i + 1) % corners.Count];

            if (!Loop && i == 0)
                prev = cur + (cur - next);

            if (!Loop && i == corners.Count - 1)
                next = cur + (cur - prev);

            _cachedCornerListToOutset.Add(InsetCorner(prev, cur, next, radius));
        }

        corners.Clear();
        corners.AddRange(_cachedCornerListToOutset);

        return corners;
    }

    private Vector2 InsetCorner(Vector2 prev, Vector2 current, Vector2 next, float radius)
    {
        Vector2 nextDirection = (next - current).normalized;
        Vector2 previousDirection = (prev - current).normalized;
        Vector2 perpDirection = Perp(current, prev, false);

        float cos = Mathf.Cos(Vector2.Angle(perpDirection, nextDirection) * Mathf.Deg2Rad);
        float d = radius / cos;

        if (Mathf.Abs(cos) < 0.00001f)
            return current + perpDirection * radius;

        return current
               + d * nextDirection
               + d * previousDirection;
    }

    private void OnDrawGizmos()
    {
        if (!OnlyWhenSelected && isActiveAndEnabled) DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (OnlyWhenSelected && isActiveAndEnabled) DrawGizmos();
    }

    private void DrawGizmos()
    {
        Color prevHandlesColor = Handles.color;
        Color prevGizmosColor = Gizmos.color;
        Gizmos.color = Handles.color = GizmoColor;

        if (Corners.Count >= 3)
            DrawPolygon(InSetCorners(FillCachedCornerList(Corners), Radius));

        Handles.color = prevHandlesColor;
        Gizmos.color = prevGizmosColor;
    }

    private void DrawPolygon(IReadOnlyList<Vector2> corners)
    {
        for (int i = 0; i < corners.Count + LoopInt(); i++)
        {
            DrawEdge(corners[i], corners[(i + 1) % corners.Count], Radius);

            if (i < corners.Count + LoopInt() * 2)
                DrawCorner(corners[i], corners[(i + 1) % corners.Count], corners[(i + 2) % corners.Count], Radius);
        }
    }

    private void DrawEdge(Vector2 from, Vector2 to, float radius)
    {
        Vector2 offset = Perp(from, to, false) * radius;
        Vector3 start = To3D(from + offset);
        Vector3 end = To3D(to + offset);
        DrawLine(start, end);
        DrawLine(start.ChangeY(-Depth), end.ChangeY(-Depth));
        DrawLine(start.ChangeY(Heigth), end.ChangeY(Heigth));


        float distance = Vector3.Distance(start, end);
        int n = Mathf.CeilToInt(distance / GizmoLineInterval);
        n = Mathf.Min(n, 250);
        for (int i = 0; i <= n; i++)
        {
            if (n != 0)
                DrawBar(Vector3.Lerp(start, end, (float)i / n));
        }
    }

    private void DrawCorner(Vector2 prev, Vector2 current, Vector2 next, float radius)
    {
        DrawCornerLine(prev, current, next, radius, -Depth);
        DrawCornerLine(prev, current, next, radius, 0);
        DrawCornerLine(prev, current, next, radius, Heigth);

        float angle = FromToAngle(prev, current, next);
        float distance = angle / 360 * radius * 2 * Mathf.PI;

        int n = Mathf.CeilToInt(distance / GizmoLineInterval);
        Vector2 startPos = Perp(prev, current, false) * this.Radius;

        n = Mathf.Min(n, 250);
        for (int i = 0; i <= n; i++)
        {
            float lerpVal = (float)i / n;

            if (!(float.IsNaN(angle) || n == 0))
                DrawBar(To3D(current) + (Quaternion.Euler(0, Mathf.Lerp(0, angle, lerpVal), 0) * To3D(startPos)));
        }
    }

    private void DrawBar(Vector3 pos)
    {
        DrawLine(pos.ChangeY(-Depth), pos.ChangeY(Heigth));
    }

    private void DrawBar(Vector2 pos)
    {
        DrawBar(To3D(pos));
    }

    private void DrawCornerLine(Vector2 prev, Vector2 current, Vector2 next, float radius, float heigth)
    {
        Handles.DrawWireArc(
            transform.TransformPoint(To3D(current).ChangeY(heigth)),
            transform.TransformDirection(Vector3.up),
            transform.TransformDirection(To3D(Perp(prev, current, false))),
            FromToAngle(prev, current, next),
            radius
        );
    }

    private void DrawLine(Vector3 from, Vector3 to)
    {
        Gizmos.DrawLine(transform.TransformPoint(from), transform.TransformPoint(to));
    }


    private Vector3 To3D(Vector2 vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }

    private Vector2 To2D(Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    private Vector2 Perp(Vector2 dir, bool inverse)
    {
        if (inverse)
        {
            return new Vector2(dir.y, -dir.x);
        }

        return new Vector2(-dir.y, dir.x);
    }

    private Vector2 Perp(Vector2 from, Vector2 to, bool inverse)
    {
        Vector2 dir = to - from;
        return Perp(dir, inverse).normalized;
    }

    private float Atan2angle(Vector2 vector2)
    {
        return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
    }

    private float FromToAngle(Vector2 prev, Vector2 current, Vector2 next)
    {
        return (Atan2angle(prev - current) - Atan2angle(current - next) + 360) % 360;
    }

    private void OnDestroy()
    {
        if (colliderContainer)
        {
            if (Application.isPlaying)
                Destroy(colliderContainer.gameObject);
            else
                DestroyImmediate(colliderContainer.gameObject);
        }
    }
#endif
}