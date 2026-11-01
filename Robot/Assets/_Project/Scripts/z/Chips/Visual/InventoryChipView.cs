using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Robot
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryChipView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _iconImage;

        private ChipData _chipData;
        private ChipsClass _chip;
        private RectTransform _rectTransform;

        private bool _isDragging, _isPlacedInGrid, _isOverGrid, _currentPlacementValid;
        private Vector2Int _currentGridPos, _dragOrigin;
        private bool[,] _currentShape;
        private int _rotationStep, _lastRotationStep;

        private void OnEnable()
        {
            InputManager.Source.RotateChip += RotateCurrentShape;
            InputManager.Source.DeleteChip += OnDeleteButtonPressed;
        }

        private void OnDisable()
        {
            InputManager.Source.RotateChip -= RotateCurrentShape;
            InputManager.Source.DeleteChip -= OnDeleteButtonPressed;
        }

        public void InitializeAsPreview(ChipData data)
        {
            _chipData = data;

            _rectTransform = GetComponent<RectTransform>();
            _iconImage.sprite = data.Icon;
            _iconImage.alphaHitTestMinimumThreshold = 0.9f;

            _isPlacedInGrid = false;
            _currentShape = data.GetShape2D();

            SetRestSize();
        }

        private void RotateCurrentShape()
        {
            if (!_isDragging) return;
            if(!_chipData.CanRotate) return;

            _currentShape = ChipsClass.RotateShape(_currentShape);
            _rotationStep = (_rotationStep + 1) % _chipData.RotationSprites.Length;

            int w = _currentShape.GetLength(0);
            int h = _currentShape.GetLength(1);
            _rectTransform.sizeDelta = new Vector2(w * ChipManager.Source.CellSize, h * ChipManager.Source.CellSize);

            _iconImage.sprite = _chipData.GetSpriteForRotation(_rotationStep);

            if (_isOverGrid)
            {
                _currentPlacementValid = _isPlacedInGrid
                    ? ChipManager.Source.CanMoveItem(_chip, _currentShape, _currentGridPos)
                    : ChipManager.Source.CanPlaceItem(_currentShape, _currentGridPos);

                ChipManager.Source.ShowHighlight(_currentShape, _currentGridPos, _currentPlacementValid);
            }

        }

        private void SetRestSize()
        {
            _rectTransform.SetParent(ChipManager.Source.Visualizer, false);
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
            _rectTransform.sizeDelta = new Vector2(w * ChipManager.Source.CellSize, h * ChipManager.Source.CellSize);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;

            if (_isPlacedInGrid)
            {
                _dragOrigin = _chip.GridPosition;
                _currentShape = _chip.Shape;
                ChipManager.Source.BeginMove(_chip);
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

            _isOverGrid = ChipManager.Source.TryGetGridPosition(eventData, out _currentGridPos);

            if (_isOverGrid)
            {
                _currentPlacementValid = _isPlacedInGrid
                    ? ChipManager.Source.CanMoveItem(_chip, _currentShape, _currentGridPos)
                    : ChipManager.Source.CanPlaceItem(_currentShape, _currentGridPos);

                ChipManager.Source.ShowHighlight(_currentShape, _currentGridPos, _currentPlacementValid);
            }
            else
            {
                ChipManager.Source.ClearHighlight();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            ChipManager.Source.ClearHighlight();

            if (_isPlacedInGrid)
            {
                if (_isOverGrid && _currentPlacementValid)
                {
                    _lastRotationStep = _rotationStep;
                    _chip.SetShape(_currentShape);
                    ChipManager.Source.ConfirmMove(_chip, _currentGridPos, this);
                }
                else
                {
                    _rotationStep = _lastRotationStep;
                    ChipManager.Source.CancelMove(_chip, _dragOrigin);
                }

                SetPlacedState(_chip.GridPosition);
            }
            else
            {
                if (_isOverGrid && _currentPlacementValid)
                {
                    _chip = ChipManager.Source.PlaceItem(_chipData, _currentShape, _currentGridPos, this);
                    _lastRotationStep = _rotationStep;
                }
                else
                {
                    SetRestSize();
                }
            }
        }

        public void SetPlacedState(Vector2Int gridPosition)
        {
            _rectTransform.SetParent(ChipManager.Source.ItemsContainer, false);
            _rectTransform.anchorMin = new Vector2(0, 1);
            _rectTransform.anchorMax = new Vector2(0, 1);
            _rectTransform.pivot = new Vector2(0, 1);
            _rectTransform.anchoredPosition = new Vector2( gridPosition.x * ChipManager.Source.CellSize,-gridPosition.y * ChipManager.Source.CellSize);

            _isPlacedInGrid = true;
            _iconImage.sprite = _chipData.GetSpriteForRotation(_rotationStep);
        }

        private void OnDeleteButtonPressed()
        {
            if (!_isDragging) return;
            if (!_isPlacedInGrid) return;

            ChipManager.Source.ReturnChipDataToList(_chip, _chipData, this);
            ChipManager.Source.ClearHighlight();
        }

    }
}
