using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Robot
{
    [RequireComponent(typeof(RectTransform))]
    public class N_InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image iconImage;

        private N_ChipsData _chipData;
        private N_ChipsClass _chip;
        private RectTransform _rectTransform;

        private bool _isDragging, _isPlacedInGrid, _isOverGrid, _currentPlacementValid;
        private Vector2Int _currentGridPos, _dragOrigin;
        private bool[,] _currentShape;
        private int _rotationStep;

        private void OnEnable()
        {
            InputManager.Source.RotateChip += RotateCurrentShape;
        }

        private void OnDisable()
        {
            InputManager.Source.RotateChip -= RotateCurrentShape;
        }

        public void InitializeAsPreview(N_ChipsData data)
        {
            _chipData = data;

            _rectTransform = GetComponent<RectTransform>();
            iconImage.sprite = data.icon;
            _isPlacedInGrid = false;
            _currentShape = data.GetShape2D();

            SetRestSize();
        }

        public void RotateCurrentShape()
        {
            if (!_isDragging) return;
            if(!_chipData.canRotate) return;

            _currentShape = N_ChipsClass.RotateShape(_currentShape);
            _rotationStep = (_rotationStep + 1) % 4;

            int w = _currentShape.GetLength(0);
            int h = _currentShape.GetLength(1);
            _rectTransform.sizeDelta = new Vector2(w * N_ChipManager.Source.CellSize, h * N_ChipManager.Source.CellSize);

            iconImage.sprite = _chipData.GetSpriteForRotation(_rotationStep);

            if (_isOverGrid)
            {
                _currentPlacementValid = _isPlacedInGrid
                    ? N_ChipManager.Source.CanMoveItem(_chip, _currentShape, _currentGridPos)
                    : N_ChipManager.Source.CanPlaceItem(_currentShape, _currentGridPos);

                N_ChipManager.Source.ShowHighlight(_currentShape, _currentGridPos, _currentPlacementValid);
            }

        }

        private void SetRestSize()
        {
            _rectTransform.SetParent(N_ChipManager.Source.Visualizer, false);
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.pivot = new Vector2(0.5f, 0.5f);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        private void SetRealSize()
        {
            bool[,] shape = _chipData.GetShape2D();
            int w = shape.GetLength(0);
            int h = shape.GetLength(1);

            _rectTransform.anchorMin = new Vector2(0f, 1f);
            _rectTransform.anchorMax = new Vector2(0f, 1f);
            _rectTransform.pivot = new Vector2(0f, 1f);
            _rectTransform.sizeDelta = new Vector2(w * N_ChipManager.Source.CellSize, h * N_ChipManager.Source.CellSize);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;

            if (_isPlacedInGrid)
            {
                _dragOrigin = _chip.GridPosition;
                _currentShape = _chip.Shape; // parte de la forma actual del chip (puede ya estar rotada)
                N_ChipManager.Source.BeginMove(_chip);
            }
            else
            {
                SetRealSize();
            }

            _rectTransform.SetParent(UIManager.Source.ChipUI.transform, true);
            _rectTransform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.position = eventData.position;

            _isOverGrid = N_ChipManager.Source.TryGetGridPosition(eventData, out _currentGridPos);

            if (_isOverGrid)
            {
                _currentPlacementValid = _isPlacedInGrid
                    ? N_ChipManager.Source.CanMoveItem(_chip, _currentShape, _currentGridPos)
                    : N_ChipManager.Source.CanPlaceItem(_currentShape, _currentGridPos);

                N_ChipManager.Source.ShowHighlight(_currentShape, _currentGridPos, _currentPlacementValid);
            }
            else
            {
                N_ChipManager.Source.ClearHighlight();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            N_ChipManager.Source.ClearHighlight();

            if (_isPlacedInGrid)
            {
                if (_isOverGrid && _currentPlacementValid)
                {
                    _chip.SetShape(_currentShape);
                    N_ChipManager.Source.ConfirmMove(_chip, _currentGridPos, this);
                }
                else
                {
                    N_ChipManager.Source.CancelMove(_chip, _dragOrigin);
                }

                SetPlacedState(_chip.GridPosition);
            }
            else
            {
                if (_isOverGrid && _currentPlacementValid)
                {
                    _chip = N_ChipManager.Source.PlaceItem(_chipData, _currentShape, _currentGridPos, this);
                }
                else
                {
                    SetRestSize();
                }
            }
        }

        public void SetPlacedState(Vector2Int gridPosition)
        {
            _rectTransform.SetParent(N_ChipManager.Source.ItemsContainer, false);
            _rectTransform.anchorMin = new Vector2(0, 1);
            _rectTransform.anchorMax = new Vector2(0, 1);
            _rectTransform.pivot = new Vector2(0, 1);
            _rectTransform.anchoredPosition = new Vector2(
                gridPosition.x * N_ChipManager.Source.CellSize,
                -gridPosition.y * N_ChipManager.Source.CellSize
            );

            _isPlacedInGrid = true;
        }

    }
}
