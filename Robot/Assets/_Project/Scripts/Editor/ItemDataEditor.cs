using UnityEditor;
using UnityEngine;

namespace Robot
{
    [CustomEditor(typeof(ChipData))]
    public class ItemDataEditor : Editor
    {
        private const float CellButtonSize = 25f;

        public override void OnInspectorGUI()
        {
            ChipData data = (ChipData)target;

            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor de forma (shape)", EditorStyles.boldLabel);

            // --- Controles de tamaño ---
            EditorGUI.BeginChangeCheck();
            int newWidth = EditorGUILayout.IntField("Ancho", data.shapeWidth);
            int newHeight = EditorGUILayout.IntField("Alto", data.shapeHeight);

            newWidth = Mathf.Clamp(newWidth, 1, 10);
            newHeight = Mathf.Clamp(newHeight, 1, 10);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Resize Chip Shape");
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
                        current, "", GUI.skin.button,
                        GUILayout.Width(CellButtonSize), GUILayout.Height(CellButtonSize)
                    );

                    if (newValue != current)
                    {
                        Undo.RecordObject(data, "Paint Chip Shape");
                        data.shapeFlat[index] = newValue;
                        EditorUtility.SetDirty(data);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Limpiar todo"))
            {
                Undo.RecordObject(data, "Clear Chip Shape");
                for (int i = 0; i < data.shapeFlat.Length; i++)
                    data.shapeFlat[i] = false;
                EditorUtility.SetDirty(data);
            }

            if (GUILayout.Button("Rellenar todo"))
            {
                Undo.RecordObject(data, "Fill Chip Shape");
                for (int i = 0; i < data.shapeFlat.Length; i++)
                    data.shapeFlat[i] = true;
                EditorUtility.SetDirty(data);
            }

            EditorGUILayout.EndHorizontal();

            // ---------- NUEVO: análisis de rotaciones únicas ----------
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Análisis de rotación", EditorStyles.boldLabel);

            if (!data.canRotate)
            {
                EditorGUILayout.HelpBox("Este ítem tiene 'canRotate' desactivado — no necesita sprites de rotación.", MessageType.None);
            }
            else
            {
                bool[,] baseShape = data.GetShape2D();
                int uniqueRotations = ChipsClass.CountUniqueRotations(baseShape);

                string message = uniqueRotations switch
                {
                    1 => "Esta forma es simétrica en las 4 rotaciones — solo necesitas 1 sprite (rotationSprites[0]). Los demás slots pueden quedar vacíos.",
                    2 => "Esta forma tiene simetría de 180° — solo necesitas 2 sprites únicos (índices 0 y 1). Los índices 2 y 3 pueden repetir los mismos.",
                    _ => "Esta forma es asimétrica — necesitas los 4 sprites únicos (uno por cada rotación de 90°)."
                };

                MessageType messageType = uniqueRotations == 4 ? MessageType.Warning : MessageType.Info;
                EditorGUILayout.HelpBox($"Rotaciones únicas necesarias: {uniqueRotations}\n{message}", messageType);

                // Resalta visualmente qué slots del array son redundantes
                DrawRotationSlotsHint(uniqueRotations);
            }
        }

        private void DrawRotationSlotsHint(int uniqueRotations)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            string[] labels = { "0°", "90°", "180°", "270°" };

            for (int i = 0; i < 4; i++)
            {
                bool isNeeded = uniqueRotations == 4
                    || (uniqueRotations == 2 && i < 2)
                    || (uniqueRotations == 1 && i == 0);

                GUI.enabled = false; // solo visual, no editable desde acá
                GUI.color = isNeeded ? Color.white : Color.gray;

                EditorGUILayout.LabelField(isNeeded ? $"{labels[i]} ✓" : $"{labels[i]} (no necesario)", EditorStyles.miniLabel);

                GUI.color = Color.white;
                GUI.enabled = true;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
