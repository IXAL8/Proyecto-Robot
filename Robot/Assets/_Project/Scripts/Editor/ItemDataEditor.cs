using UnityEditor;
using UnityEngine;

namespace Robot
{
    [CustomEditor(typeof(N_ChipsData))]
    public class ItemDataEditor : Editor
    {
        private const float CellButtonSize = 25f;

        public override void OnInspectorGUI()
        {
            N_ChipsData data = (N_ChipsData)target;

            // Dibuja los campos normales (itemName, icon) automáticamente
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Shape Editor (shape)", EditorStyles.boldLabel);

            // --- Controles de tamaño ---
            EditorGUI.BeginChangeCheck();
            int newWidth = EditorGUILayout.IntField("Width", data.shapeWidth);
            int newHeight = EditorGUILayout.IntField("Height", data.shapeHeight);

            newWidth = Mathf.Clamp(newWidth, 1, 10);
            newHeight = Mathf.Clamp(newHeight, 1, 10);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Resize Item Shape");
                data.ResizeShape(newWidth, newHeight);
                EditorUtility.SetDirty(data);
            }

            EditorGUILayout.Space(5);

            // --- Grid pintable ---
            for (int y = 0; y < data.shapeHeight; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < data.shapeWidth; x++)
                {
                    int index = y * data.shapeWidth + x;
                    bool current = data.shapeFlat[index];

                    bool newValue = GUILayout.Toggle(
                        current,
                        "",
                        GUI.skin.button,
                        GUILayout.Width(CellButtonSize),
                        GUILayout.Height(CellButtonSize)
                    );

                    if (newValue != current)
                    {
                        Undo.RecordObject(data, "Paint Item Shape");
                        data.shapeFlat[index] = newValue;
                        EditorUtility.SetDirty(data);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);

            // --- Botones útiles ---
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear all"))
            {
                Undo.RecordObject(data, "Clear Item Shape");
                for (int i = 0; i < data.shapeFlat.Length; i++)
                    data.shapeFlat[i] = false;
                EditorUtility.SetDirty(data);
            }

            if (GUILayout.Button("Fill"))
            {
                Undo.RecordObject(data, "Fill Item Shape");
                for (int i = 0; i < data.shapeFlat.Length; i++)
                    data.shapeFlat[i] = true;
                EditorUtility.SetDirty(data);
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
