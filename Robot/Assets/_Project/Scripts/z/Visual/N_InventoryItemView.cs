using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Robot
{
    [RequireComponent(typeof(RectTransform))]
    public class N_InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image iconImage;

        private N_ChipsClass _chip;
        private N_InventoryGrid _logicGrid;
        private N_InventoryGridView _gridView;

        private RectTransform _rectTransform;
        private Vector2Int _dragOrigin;

        public void Initialize(N_ChipsClass chip, N_InventoryGrid grid, N_InventoryGridView gridView)
        {
            _chip = chip;
            _logicGrid = grid;
            _gridView = gridView;
            _rectTransform = GetComponent<RectTransform>();

            _rectTransform.SetParent(gridView.ItemsContainer, false);

            iconImage.sprite = chip.Data.icon;
            UpdateVisualSize();
            UpdateVisualPosition();
        }

        private void UpdateVisualSize()
        {
            int w = _chip.Shape.GetLength(0);
            int h = _chip.Shape.GetLength(1);
            _rectTransform.sizeDelta = new Vector2(w * _gridView.CellSize, h * _gridView.CellSize);
        }

        private void UpdateVisualPosition()
        {
            _rectTransform.anchoredPosition = new Vector2(
                _chip.GridPosition.x * _gridView.CellSize,
                -_chip.GridPosition.y * _gridView.CellSize
            );
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragOrigin = _chip.GridPosition;
            _logicGrid.RemoveItem(_chip);
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gridView.ItemsContainer, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            Vector2Int gridPos = _gridView.WorldToGridPosition(localPoint);

            bool valid = _logicGrid.CanPlaceItem(_chip, gridPos);
            _gridView.HighlightCells(_chip.Shape, gridPos, valid);

            _rectTransform.anchoredPosition = new Vector2(
                gridPos.x * _gridView.CellSize,
                -gridPos.y * _gridView.CellSize
            );
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _gridView.ClearHighlights();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_gridView.ItemsContainer, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

            Vector2Int gridPos = _gridView.WorldToGridPosition(localPoint);

            if (_logicGrid.CanPlaceItem(_chip, gridPos))
                _logicGrid.PlaceItem(_chip, gridPos);
            else
                _logicGrid.PlaceItem(_chip, _dragOrigin);

            UpdateVisualPosition();
        }
    }
}
