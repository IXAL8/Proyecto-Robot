using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class N_ChipManager : Singleton<IChipSource>, IChipSource
    {
        [Header("Grid Configuration")]
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [Header("Chips available")]
        [SerializeField] private GameObject _chipPrefab;
        [SerializeField] private List<N_ChipsData> _chipsAvailables = new();

        [Header("References")]
        [SerializeField] private N_InventoryGridView _gridUI;
        [SerializeField] private N_PanelVisualizer _panelVisualizer;
        private N_InventoryGrid _logicGrid;

        public GameObject ChipPrefab => _chipPrefab;
        public List<N_ChipsData> ChipsAvailables => _chipsAvailables;
        public N_InventoryGridView GridUI => _gridUI;
        public N_InventoryGrid LogicGrid => _logicGrid;
        public float CellSize => _gridUI.CellSize;
        public RectTransform ItemsContainer => _gridUI.ItemsContainer;
        public RectTransform Visualizer => _panelVisualizer.VisualizerRectTransform;

        protected override void Awake()
        {
            base.Awake();

            InitializeGrid();
        }

        private void InitializeGrid()
        {
            _logicGrid = new N_InventoryGrid(_width, _height);
            _gridUI.Initialize(_logicGrid);
        }

        public bool TryGetGridPosition(PointerEventData eventData, out Vector2Int gridPos)
        {
            gridPos = default;

            bool isOverGrid = RectTransformUtility.RectangleContainsScreenPoint(_gridUI.ItemsContainer, eventData.position, eventData.pressEventCamera);

            if (!isOverGrid) return false;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gridUI.ItemsContainer, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            gridPos = _gridUI.WorldToGridPosition(localPoint);
            return true;
        }

        public bool CanPlaceItem(bool[,] shape, Vector2Int position)
        {
            return _logicGrid.CanPlaceItem(shape, position);
        }

        public void ShowHighlight(bool[,] shape, Vector2Int position, bool valid)
        {
            _gridUI.HighlightCells(shape, position, valid);
        }

        public void ClearHighlight()
        {
            _gridUI.ClearHighlights();
        }

        public N_ChipsClass PlaceItem(N_ChipsData data, bool[,] shape, Vector2Int position, N_InventoryItemView view)
        {
            N_ChipsClass chip = new N_ChipsClass(data, shape);
            _logicGrid.PlaceChip(chip, position);
            view.SetPlacedState(chip.GridPosition);
            _panelVisualizer.OnChipPlaced();
            return chip;
        }

        public void BeginMove(N_ChipsClass chip)
        {
            _logicGrid.RemoveChip(chip);
        }

        public bool CanMoveItem(N_ChipsClass chip, bool[,] shape, Vector2Int position)
        {
            return _logicGrid.CanPlaceItem(chip, position);
        }

        public void ConfirmMove(N_ChipsClass chip, Vector2Int position, N_InventoryItemView view)
        {
            _logicGrid.PlaceChip(chip, position);
            view.SetPlacedState(chip.GridPosition);
        }

        public void CancelMove(N_ChipsClass chip, Vector2Int originalPosition)
        {
            _logicGrid.PlaceChip(chip, originalPosition);
        }

    }
}
