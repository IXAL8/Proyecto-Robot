using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public interface IChipSource
    {
        GameObject ChipPrefab { get; }
        List<ChipData> ChipsAvailables {  get; }
        float CellSize { get; }
        RectTransform ItemsContainer {  get; }
        RectTransform Visualizer {  get; }

        InventoryGridView GridUI { get; }
        InventoryGrid LogicGrid {  get; }


        bool TryGetGridPosition(PointerEventData eventData, out Vector2Int gridPos);
        bool CanPlaceItem(bool[,] shape, Vector2Int position);
        void ShowHighlight(bool[,] shape, Vector2Int position, bool valid);
        void ClearHighlight();

        ChipsClass PlaceItem(ChipData data, bool[,] shape, Vector2Int position, InventoryChipView view);

        void BeginMove(ChipsClass chip);
        bool CanMoveItem(ChipsClass chip, bool[,] shape, Vector2Int position);
        void ConfirmMove(ChipsClass chip, Vector2Int position, InventoryChipView view);
        void CancelMove(ChipsClass chip, Vector2Int originalPosition);

        void ReturnChipDataToList(ChipsClass chip, ChipData chipData, InventoryChipView view);
    }
}
