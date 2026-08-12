using UnityEngine;

namespace Robot
{
    [CreateAssetMenu(fileName = "TestNewChips", menuName = "Inventory/Item/TestChips")]
    public class N_ChipsData : ScriptableObject
    {
        [Header("Chip Data")]
        public string chipName;
        public Sprite icon;

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
    }
}
