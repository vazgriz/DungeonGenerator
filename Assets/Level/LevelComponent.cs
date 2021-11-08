using UnityEngine;

public class LevelComponent : MonoBehaviour
{
    public BoundsInt Bounds { get; }

    public LevelComponent(Vector3Int location, Vector3Int size)
    {
        Bounds = new BoundsInt(location, size);
    }

    public static bool Intersect(LevelComponent a, LevelComponent b)
    {
        return !((a.Bounds.position.x >= (b.Bounds.position.x + b.Bounds.size.x)) || ((a.Bounds.position.x + a.Bounds.size.x) <= b.Bounds.position.x)
            || (a.Bounds.position.y >= (b.Bounds.position.y + b.Bounds.size.y)) || ((a.Bounds.position.y + a.Bounds.size.y) <= b.Bounds.position.y)
            || (a.Bounds.position.z >= (b.Bounds.position.z + b.Bounds.size.z)) || ((a.Bounds.position.z + a.Bounds.size.z) <= b.Bounds.position.z));
    }
}
