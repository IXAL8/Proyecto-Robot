using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "Chips", menuName = "Inventory/Item/Chip")]
    public class ChipData : ScriptableObject
    {
        [Header("Chip Data")]
        public string Name;
        public Sprite icon;
        [TextArea(1, 2)]
        public string Description;

        [Header("Rotation")]
        public bool canRotate = true;
        public Sprite[] rotationSprites = new Sprite[4];

        [Header("Shape (Does not modify, use the grid of below)")]
        public int shapeWidth = 3;
        public int shapeHeight = 3;
        public bool[] shapeFlat = new bool[9];

        public bool[,] GetShape2D()
        {
            bool[,] shape = new bool[shapeWidth, shapeHeight];
            for (int x = 0; x < shapeWidth; x++)
                for (int y = 0; y < shapeHeight; y++)
                    shape[x, y] = shapeFlat[y * shapeWidth + x];
            return shape;
        }

        public void ResizeShape(int newWidth, int newHeight)
        {
            bool[] newFlat = new bool[newWidth * newHeight];

            for (int x = 0; x < Mathf.Min(shapeWidth, newWidth); x++)
            {
                for (int y = 0; y < Mathf.Min(shapeHeight, newHeight); y++)
                {
                    newFlat[y * newWidth + x] = shapeFlat[y * shapeWidth + x];
                }
            }

            shapeWidth = newWidth;
            shapeHeight = newHeight;
            shapeFlat = newFlat;
        }

        public Sprite GetSpriteForRotation(int rotationStep)
        {
            int index = ((rotationStep % 4) + 4) % 4;
            return rotationSprites[index] != null ? rotationSprites[index] : icon;
        }
    }
}
