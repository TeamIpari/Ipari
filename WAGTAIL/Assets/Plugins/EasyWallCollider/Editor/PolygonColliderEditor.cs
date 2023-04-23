#if UNITY_EDITOR
using PepijnWillekens.Extensions;
using UnityEditor;
using UnityEngine;

namespace EasyWallColliderUnity.Editor
{
    [CustomEditor(typeof(EasyWallCollider))]
    class PolygonColliderEditor : UnityEditor.Editor
    {
        protected virtual void OnSceneGUI()
        {
            EasyWallCollider easyWallCollider = (EasyWallCollider)target;
            if (!easyWallCollider.CanBeEdited() || !easyWallCollider.isActiveAndEnabled) return;
            var corners = easyWallCollider.Corners;
            if (corners.Count < 3) return;
            for (int i = 0; i < corners.Count + easyWallCollider.LoopInt(); i++)
            {
                Vector3 from = corners[i].position;
                Vector3 to = corners[(i + 1) % corners.Count].position;
                DrawButton(easyWallCollider, from, to, corners[i], corners[i].GetSiblingIndex() + 1);
            }

            if (!easyWallCollider.Loop)
            {
                DrawButton(easyWallCollider, corners[0].position,
                    corners[0].position + (corners[0].position - corners[1].position), corners[0],
                    corners[0].GetSiblingIndex());
                int lastIx = corners.Count - 1;
                DrawButton(easyWallCollider, corners[lastIx].position,
                    corners[lastIx].position + (corners[lastIx].position - corners[lastIx - 1].position),
                    corners[lastIx], corners[lastIx].GetSiblingIndex() + 1);
            }
        }

        public override void OnInspectorGUI()
        {
            EasyWallCollider easyWallCollider = (EasyWallCollider)target;
            if (easyWallCollider.CanBeEdited())
            {
                base.OnInspectorGUI();
                if (easyWallCollider.NoHide)
                {
                    EditorGUILayout.HelpBox(
                        "Use debug mode at your own risk. We cannot guarantee everything to stay error-free in debug mode, as this allows you to break things. It's only intended for investigation and debugging, if you know what you're doing.",
                        MessageType.Warning);
                }
            }
            else
            {
                GUILayout.Label(
                    "Editing polygon colliders is not allowed on Prefabs. Open the prefab and edit there instead");
            }
        }

        private void DrawButton(EasyWallCollider easyWallCollider, Vector3 from, Vector3 to, Transform cornerPrototype,
            int newIndex)
        {
            Color prevColor = Handles.color;
            Handles.color = ((EasyWallCollider)target).GizmoColor;

            float size = Vector3.Distance(from, to) / 20f;
            if (Handles.Button(((from + to) / 2).ChangeY((y) => y - 0.001f),
                    Quaternion.LookRotation(from - to) * Quaternion.Euler(90, 0, 0), size, size,
                    Handles.ConeHandleCap))
            {
                var newCorner = Instantiate(cornerPrototype, (from + to) / 2, Quaternion.identity,
                    easyWallCollider.transform);
                newCorner.SetSiblingIndex(newIndex);
                Selection.activeGameObject = newCorner.gameObject;
            }

            Handles.color = prevColor;
        }
    }
}
#endif