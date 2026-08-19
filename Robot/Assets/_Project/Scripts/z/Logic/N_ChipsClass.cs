using UnityEngine;

namespace Robot
{
    public class N_ChipsClass
    {
        public bool[,] Shape { get; set; }
        public Vector2Int GridPosition { get; set; }
        public N_ChipsData Data { get; set; } 

        public N_ChipsClass(N_ChipsData data, bool[,] overrideShape = null)
        {
            Data = data;
            Shape = overrideShape ?? data.GetShape2D();
        }

        public void Rotate()
        {
            Shape = RotateShape(Shape);
        }

        public void SetShape(bool[,] shape)
        {
            Shape = shape;
        }

        public static bool[,] RotateShape(bool[,] shape)
        {
            int w = shape.GetLength(0);
            int h = shape.GetLength(1);
            bool[,] rotated = new bool[h, w];

            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    rotated[y, w - 1 - x] = shape[x, y];

            return rotated;
        }

        public static bool ShapesAreEqual(bool[,] a, bool[,] b)
        {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
                return false;

            for (int x = 0; x < a.GetLength(0); x++)
                for (int y = 0; y < a.GetLength(1); y++)
                    if (a[x, y] != b[x, y])
                        return false;

            return true;
        }

        public static int CountUniqueRotations(bool[,] baseShape)
        {
            bool[,] rot90 = RotateShape(baseShape);
            bool[,] rot180 = RotateShape(rot90);

            if (ShapesAreEqual(baseShape, rot90)) return 1;
            if (ShapesAreEqual(baseShape, rot180)) return 2;
            return 4;
        }
    }
}
