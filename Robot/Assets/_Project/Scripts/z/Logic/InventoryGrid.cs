using System.Collections.Generic;
using UnityEngine;

namespace Robot
{
    public class InventoryGrid
    {
        public int width, height;
        public ChipsClass[,] cells;

        public int Width => width;
        public int Height => height;

        public InventoryGrid(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new ChipsClass[width, height];
        }

        public bool CanPlaceItem(bool[,] shape, Vector2Int position)
        {
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

                    if (cells[gridX, gridY] != null)
                        return false;
                }
            }
            return true;
        }

        public bool CanPlaceItem(ChipsClass chip, Vector2Int position)
        {
            bool[,] shape = chip.Shape;
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

                    if (cells[gridX, gridY] != null && cells[gridX, gridY] != chip)
                        return false;
                }
            }
            return true;
        }

        public bool CanPlaceItem(ChipsClass chip, bool[,] shape, Vector2Int position)
        {
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

                    if (cells[gridX, gridY] != null && cells[gridX, gridY] != chip)
                        return false;
                }
            }
            return true;
        }

        public bool PlaceChip(ChipsClass chip, Vector2Int position)
        {
            if (!CanPlaceItem(chip, position))
                return false;

            bool[,] shape = chip.Shape;
            int shapeWidth = shape.GetLength(0);
            int shapeHeight = shape.GetLength(1);

            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (shape[x, y])
                        cells[position.x + x, position.y + y] = chip;
                }
            }

            chip.GridPosition = position;
            return true;
        }

        public bool RemoveChip(ChipsClass chip)
        {
            bool removed = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cells[x, y] == chip)
                    {
                        cells[x, y] = null;
                        removed = true;
                    }
                }
            }

            return removed;
        }


        public ChipsClass GetChipAt(Vector2Int position)
        {
            if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
                return null;

            return cells[position.x, position.y];
        }

        public bool MoveChip(ChipsClass chip, Vector2Int newPosition)
        {
            Vector2Int oldPosition = chip.GridPosition;

            RemoveChip(chip);

            if (CanPlaceItem(chip, newPosition))
            {
                PlaceChip(chip, newPosition);
                return true;
            }

            PlaceChip(chip, oldPosition);
            return false;
        }

        public void PrintOccupiedCellsDetailed()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"=== Estado del grid ({width}x{height}) ===");

            // Asigna una letra única a cada chip distinto encontrado en el grid
            Dictionary<ChipsClass, char> chipSymbols = new Dictionary<ChipsClass, char>();
            char nextSymbol = 'A';

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ChipsClass chip = cells[x, y];
                    if (chip != null && !chipSymbols.ContainsKey(chip))
                    {
                        chipSymbols[chip] = nextSymbol;
                        nextSymbol++;
                    }
                }
            }

            // Dibuja la grilla usando esas letras
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ChipsClass chip = cells[x, y];
                    sb.Append(chip != null ? chipSymbols[chip].ToString() : ".");
                }
                sb.AppendLine();
            }

            // Leyenda: qué letra corresponde a qué ítem y en qué posición de origen está
            if (chipSymbols.Count > 0)
            {
                sb.AppendLine("\n--- Leyenda ---");
                foreach (var kvp in chipSymbols)
                {
                    ChipsClass chip = kvp.Key;
                    char symbol = kvp.Value;
                    sb.AppendLine($"{symbol} = {chip.Data.Name} (origen: {chip.GridPosition})");
                }
            }
            else
            {
                sb.AppendLine("(grid vacío)");
            }

            Debug.Log(sb.ToString());
        }
    }
}
