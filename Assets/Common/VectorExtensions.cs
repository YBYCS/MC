using UnityEngine;

public static class VectorExtensions {
	public static Vector2 Floored(this Vector2 v) => new(Mathf.Floor(v.x), Mathf.Floor(v.y));
	public static Vector2 Fractioned(this Vector2 v) => new(v.x - Mathf.Floor(v.x), v.y - Mathf.Floor(v.y));
}
