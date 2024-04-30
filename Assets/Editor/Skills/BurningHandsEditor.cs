using Skills.Warlock;
using UnityEditor;
using UnityEngine;
using Player;

namespace ZenEditor.Skills
{
    [CustomEditor(typeof(BurningHands))]
    public class BurningHandsEditor : Editor
    {
        PlayerEntity _p;
        PlayerEntity player
        {
            get
            {
                if (_p == null)
                    _p = FindObjectOfType<PlayerEntity>();
                return _p;
            }
        }
        private void OnSceneGUI()
        {
            var bh = (BurningHands)target;

            Handles.color = Color.gray;

            Vector3 pos = bh.transform.position + Vector3.up;

            Handles.DrawWireArc(pos, Vector3.up, bh.transform.forward, 360, bh.range);
            Vector3 viewAngleA = DirFromAngle(player.transform.eulerAngles.y - bh.angle);
            Vector3 viewAngleB = DirFromAngle(player.transform.eulerAngles.y + bh.angle);

            Handles.DrawLine(pos, pos + viewAngleA * bh.range);
            Handles.DrawLine(pos, pos + viewAngleB * bh.range);
        }

        public Vector3 DirFromAngle(float angleInDegrees)
        {
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}