using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        // 🟢 Lấy vị trí mắt của enemy (cao hơn 1.6f)
        Vector3 enemyEyePos = fov.transform.position + Vector3.up * 1.6f;

        // ⚪ Vẽ vòng tròn bán kính tầm nhìn tại vị trí mắt
        Handles.color = Color.white;
        Handles.DrawWireArc(enemyEyePos, Vector3.up, Vector3.forward, 360, fov.radius);

        // 🟡 Tính 2 hướng biên của góc nhìn
        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        // 🟠 Vẽ 2 tia biên góc nhìn
        Handles.color = Color.yellow;
        Handles.DrawLine(enemyEyePos, enemyEyePos + viewAngle01 * fov.radius);
        Handles.DrawLine(enemyEyePos, enemyEyePos + viewAngle02 * fov.radius);

        // 🔵 Nếu đang thấy player → vẽ tia xanh nối enemy ↔ player
        if (fov.cannSeePlayer && fov.player != null)
        {
            // 🧍 Lấy vị trí mắt player (pivot dưới chân → cũng +1.6f)
            Vector3 playerEyePos = fov.player.transform.position + Vector3.up * 1.6f;

            Handles.color = Color.green;
            Handles.DrawLine(enemyEyePos, playerEyePos);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
