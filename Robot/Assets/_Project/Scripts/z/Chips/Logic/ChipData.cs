using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "Chips", menuName = "Inventory/Item/Chip")]
    public class ChipData : ScriptableObject
    {
        [Header("Chip Data")]
        public string Name;
        public Sprite Icon;
        [TextArea(1, 2)]
        public string Description;

        [Header("Rotation")]
        public bool CanRotate = true;
        public Sprite[] RotationSprites = new Sprite[4];

        [Header("Shape (Does not modify, use the grid of below)")]
        public int ShapeWidth = 3;
        public int ShapeHeight = 3;
        public bool[] ShapeFlat = new bool[9];

        [Header("Chip Effects")]
        public List<ChipEffect> Effects = new List<ChipEffect>();

        public bool[,] GetShape2D()
        {
            bool[,] shape = new bool[ShapeWidth, ShapeHeight];
            for (int x = 0; x < ShapeWidth; x++)
                for (int y = 0; y < ShapeHeight; y++)
                    shape[x, y] = ShapeFlat[y * ShapeWidth + x];
            return shape;
        }

        public void ResizeShape(int newWidth, int newHeight)
        {
            bool[] newFlat = new bool[newWidth * newHeight];

            for (int x = 0; x < Mathf.Min(ShapeWidth, newWidth); x++)
            {
                for (int y = 0; y < Mathf.Min(ShapeHeight, newHeight); y++)
                {
                    newFlat[y * newWidth + x] = ShapeFlat[y * ShapeWidth + x];
                }
            }

            ShapeWidth = newWidth;
            ShapeHeight = newHeight;
            ShapeFlat = newFlat;
        }

        public Sprite GetSpriteForRotation(int rotationStep)
        {
            int index = ((rotationStep % 4) + 4) % 4;
            return RotationSprites[index] != null ? RotationSprites[index] : Icon;
        }
    }
}
