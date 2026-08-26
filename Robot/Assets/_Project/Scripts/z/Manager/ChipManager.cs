using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Robot
{
    public class ChipManager : Singleton<IChipSource>, IChipSource
    {
        [Header("Grid Configuration")]
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [Header("Chips available")]
        [SerializeField] private GameObject _chipPrefab;
        [SerializeField] private List<ChipData> _chipsAvailables = new();

        [Header("References")]
        [SerializeField] private InventoryGridView _gridUI;
        [SerializeField] private PanelVisualizer _panelVisualizer;
        private InventoryGrid _logicGrid;

        public GameObject ChipPrefab => _chipPrefab;
        public List<ChipData> ChipsAvailables => _chipsAvailables;
        public InventoryGridView GridUI => _gridUI;
        public InventoryGrid LogicGrid => _logicGrid;
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
            _logicGrid = new InventoryGrid(_width, _height);
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

        public ChipsClass PlaceItem(ChipData data, bool[,] shape, Vector2Int position, InventoryChipView view)
        {
            ChipsClass chip = new ChipsClass(data, shape);
            _logicGrid.PlaceChip(chip, position);
            view.SetPlacedState(chip.GridPosition);
            _panelVisualizer.OnChipPlaced();
            return chip;
        }

        public void BeginMove(ChipsClass chip)
        {
            _logicGrid.RemoveChip(chip);
        }

        public bool CanMoveItem(ChipsClass chip, bool[,] shape, Vector2Int position)
        {
            return _logicGrid.CanPlaceItem(chip, shape, position);
        }

        public void ConfirmMove(ChipsClass chip, Vector2Int position, InventoryChipView view)
        {
            _logicGrid.PlaceChip(chip, position);
            view.SetPlacedState(chip.GridPosition);
        }

        public void CancelMove(ChipsClass chip, Vector2Int originalPosition)
        {
            _logicGrid.PlaceChip(chip, originalPosition);
        }

        public void ReturnChipDataToList(ChipsClass chip, ChipData chipData, InventoryChipView view)
        {
            _logicGrid.RemoveChip(chip);
            _chipsAvailables.Add(chipData);
            Destroy(view.gameObject);
            _panelVisualizer.OnChipRemoved();
        }

        [ContextMenu("Debug: Mostrar celdas ocupadas")]
        private void DebugPrintGrid()
        {
            _logicGrid.PrintOccupiedCellsDetailed();
        }
    }
}
