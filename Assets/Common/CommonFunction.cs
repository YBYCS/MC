
using LYcommon;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public static class CommonFunction {
    /// <summary>
    /// 绕z轴逆旋转一定度数
    /// </summary>
    /// <param name="original"></param>
    /// <param name="angle"></param>
    /// <returns>返回一个旋转后的向量</returns>
    public static Vector2 TurnV2(Vector2 original, float angle) {
        if (original == Vector2.zero) return Vector2.zero;
        angle *= Mathf.PI / 180;
        float mag = original.magnitude;
        angle += Mathf.Atan2(original.y, original.x);
        return new Vector2(mag * Mathf.Cos(angle), mag * Mathf.Sin(angle));
    }
    /// <summary>
    /// 创建一个文本文件,编码为UTF8,如果没有对应的文件夹则创建对应的文件夹
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="data">要写入的数据</param>
    /// <param name="fileMode">写入模式</param>
    public static void FileIO(string path, string data, FileMode fileMode = FileMode.Create) {
        FileInfo fileInfo = new FileInfo(path);
        if (File.Exists(path) && fileMode == FileMode.Create) {
            Debug.Log("注意,已存在:" + path + "该文件将会被覆盖");
        }
        var d = fileInfo.Directory;
        if (!d.Exists) {
            Debug.Log("文件目录" + d + "不存在,已自动创建");
            d.Create();
        }
        using FileStream file = new(path, FileMode.Create);
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        file.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// 抛出一个异常
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="CustomException"></exception>
    public static void ThrowException(string data) {
        throw new CustomException(data);
    }

    /// <summary>
    /// get the hit under the mouse in 2d
    /// </summary>
    /// <returns>return the hit, it may be null. You can get collider by using hit.collider</returns>
    public static RaycastHit2D GetHitUnderTheMouse2D() {
        var onScreenPosition = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(onScreenPosition);
        var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity);
        return hit;
    }

    /// <summary>
    /// move the go when the (awsd)key down. you don't need hundle the input. 不推荐使用此函数
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="speed"></param>
    /// <param name="isOverLook">是否是俯视角度</param>
    public static void MoveGameObject(GameObject gameObject, float speed, bool isOverLook) {
        Vector3 dir = Input.GetAxisRaw("Horizontal") * gameObject.transform.right;

        if (isOverLook) {
            dir += Input.GetAxisRaw("Virtual") * gameObject.transform.up;
        }

        gameObject.transform.Translate(dir * Time.deltaTime * speed, Space.World);
    }

    /// <summary>
    /// 获得带方向的夹角度数
    /// </summary>
    /// <param name="fromVector"></param>
    /// <param name="toVector"></param>
    /// <returns></returns>
    public static float GetV3Angle(Vector3 fromVector, Vector3 toVector) {
        float angle = Vector3.Angle(fromVector, toVector); //求出两向量之间的夹角
        Vector3 normal = Vector3.Cross(fromVector, toVector);//叉乘求出法线向量
        angle *= Mathf.Sign(Vector3.Dot(normal, Vector3.up)); //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向
        return angle;
    }

    /// <summary>
    /// Check if it is on the ground. The gameObject must have collider2d
    /// layermask参数设置的一些总结：1 << 10 打开第10的层。~(1 << 10) 打开除了第10之外的层。~(1 << 0) 打开所有的层。(1 << 10) | (1 << 8) 打开第10和第8的层。
    /// </summary>
    /// <param name="gameObject">object to be detected</param>
    /// <param name="layerMash">the layerMesh. If you want deceted the ground whose layer is 7, you should pass parameter "1<<7"</param>
    /// <param name="extraDistance">The distance to be deteted in addition to the distance to itself</param>
    /// <returns></returns>
    public static bool isOnGround2D(GameObject gameObject, int layerMash, float extraDistance = 0.1f) {
        var hit = Physics2D.Raycast(gameObject.transform.position, -gameObject.transform.up, extraDistance + gameObject.GetComponent<Collider2D>().bounds.extents.y, layerMash);
        return hit.collider != null;
    }
    /// <summary>
    /// 
    /// </summary>
    public static void DebugMultiple(params object[] log) {
        string s = "";
        foreach (var obj in log) {
            //if (obj is int || obj is float || obj is double || obj is decimal) {
            //    s += obj + " ";
            //}
            //else {
            //    s += obj.ToString() + " ";
            //}
            s += obj.ToString() + "   ";
        }
        Debug.Log(s);
    }

    public static float GetXOnBezierCurve(Vector2 startPoint, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint, float y) {
        // Calculate the x coordinates of the four points
        float x1 = startPoint.x;
        float x2 = controlPoint1.x;
        float x3 = controlPoint2.x;
        float x4 = endPoint.x;

        // Use binary search to find t, such that the y coordinate of the point on the curve is y
        float t = 0.5f; // start with t=0.5
        float y0 = 0;
        float error = 0.01f; // the maximum allowed error
        float step = 0.25f; // the step size for binary search
        while (Mathf.Abs(y0 - y) > error) {
            y0 = Mathf.Pow(1 - t, 3) * startPoint.y + 3 * t * Mathf.Pow(1 - t, 2) * controlPoint1.y + 3 * Mathf.Pow(t, 2) * (1 - t) * controlPoint2.y + Mathf.Pow(t, 3) * endPoint.y;
            if (y0 < y) {
                t += step;
            }
            else {
                t -= step;
            }
            step /= 2; // reduce the step size
        }

        // Return the x coordinate of the point on the curve at the found t
        return Mathf.Pow(1 - t, 3) * x1 + 3 * t * Mathf.Pow(1 - t, 2) * x2 + 3 * Mathf.Pow(t, 2) * (1 - t) * x3 + Mathf.Pow(t, 3) * x4;
    }

}


