using UnityEngine;

namespace Robot
{
    public class N_ChipsClass
    {
        public bool[,] Shape { get; set; }
        public Vector2Int GridPosition { get; set; }
        public N_ChipsData Data { get; set; } 

        public N_ChipsClass(N_ChipsData data)
        {
            Data = data;
            Shape = data.GetShape2D();
        }

        public void Rotate()
        {
            int w = Shape.GetLength(0);
            int h = Shape.GetLength(1);
            bool[,] rotated = new bool[h, w];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    rotated[y, w - 1 - x] = Shape[x, y];

            Shape = rotated;
        }
    }
}
