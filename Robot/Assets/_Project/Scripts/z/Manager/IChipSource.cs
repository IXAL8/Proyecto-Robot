using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public interface IChipSource
    {
        GameObject ChipPrefab { get; }
        List<N_ChipsData> ChipsAvailables {  get; }
        float CellSize { get; }
        RectTransform ItemsContainer {  get; }
        RectTransform Visualizer {  get; }

        N_InventoryGridView GridUI { get; }
        N_InventoryGrid LogicGrid {  get; }


        bool TryGetGridPosition(PointerEventData eventData, out Vector2Int gridPos);
        bool CanPlaceItem(bool[,] shape, Vector2Int position);
        void ShowHighlight(bool[,] shape, Vector2Int position, bool valid);
        void ClearHighlight();

        N_ChipsClass PlaceItem(N_ChipsData data, bool[,] shape, Vector2Int position, N_InventoryItemView view);

        void BeginMove(N_ChipsClass chip);
        bool CanMoveItem(N_ChipsClass chip, bool[,] shape, Vector2Int position);
        void ConfirmMove(N_ChipsClass chip, Vector2Int position, N_InventoryItemView view);
        void CancelMove(N_ChipsClass chip, Vector2Int originalPosition);
    }
}
