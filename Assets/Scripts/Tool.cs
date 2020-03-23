using UnityEngine;

public class Tool
{
    public static Vector3 GetRayPointFromY(Vector3 knownpoint, Vector3 dir, float y)
    {
        var res = new Vector3(0, y, 0);
        res.x = (y - knownpoint.y) / dir.y * dir.x + knownpoint.x;
        res.z = (y - knownpoint.y) / dir.y * dir.z + knownpoint.z;
        return res;
    }

}
