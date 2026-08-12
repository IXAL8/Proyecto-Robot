using UnityEngine;

namespace Robot
{
    public class N_InventoryGrid
    {
        public int width, height;
        public N_ChipsClass[,] cells;

        public int Width => width;
        public int Height => height;

        public N_InventoryGrid(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new N_ChipsClass[width, height];
        }

        public bool CanPlaceItem(N_ChipsClass item, Vector2Int position)
        {
            bool[,] shape = item.Shape;
            int shapeWidth = shape.GetLength(0);
            int shapeHeight = shape.GetLength(1);

            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (!shape[x, y]) continue;

                    int gridX = position.x + x;
                    int gridY = position.y + y;

                    if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                        return false;

                    if (cells[gridX, gridY] != null && cells[gridX, gridY] != item)
                        return false;
                }
            }
            return true;
        }
        public bool PlaceItem(N_ChipsClass item, Vector2Int position)
        {
            if (!CanPlaceItem(item, position))
                return false;

            bool[,] shape = item.Shape;
            int shapeWidth = shape.GetLength(0);
            int shapeHeight = shape.GetLength(1);

            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (shape[x, y])
                        cells[position.x + x, position.y + y] = item;
                }
            }

            item.GridPosition = position;
            return true;
        }
        public bool RemoveItem(N_ChipsClass item)
        {
            bool removed = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y] == item)
                    {
                        cells[x, y] = null;
                        removed = true;
                    }
                }
            }

            return removed;
        }

        
        public N_ChipsClass GetItemAt(Vector2Int position)
        {
            if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
                return null;

            return cells[position.x, position.y];
        }

        public bool MoveItem(N_ChipsClass item, Vector2Int newPosition)
        {
            Vector2Int oldPosition = item.GridPosition;

            RemoveItem(item);

            if (CanPlaceItem(item, newPosition))
            {
                PlaceItem(item, newPosition);
                return true;
            }

            PlaceItem(item, oldPosition);
            return false;
        }
    }
}
