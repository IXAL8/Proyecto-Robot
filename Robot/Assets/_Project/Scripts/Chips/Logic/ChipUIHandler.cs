using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Robot
{
    public class ChipUIHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Canvas _chipCanva;
        private CanvasGroup _chipCanvasGroup;
        private RectTransform _chipPivotRectTransform;
        private Chip _chip;
        public bool _isDragging {  get; private set; }
        private bool _firsttouch;

        private void Awake()
        {
            _chipPivotRectTransform = GetComponent<RectTransform>();
            _chip = GetComponentInChildren<Chip>();
            _chipCanvasGroup = GetComponent<CanvasGroup>();
            _chipCanva = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            InputManager.Source.DeleteChip += DeleteChipPlaced;
            InputManager.Source.RotateChip += RotateChip;
        }

        private void OnDestroy()
        {
            InputManager.Source.DeleteChip -= DeleteChipPlaced;
            InputManager.Source.RotateChip -= RotateChip;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            if (!_firsttouch)
            {
                ChipInventoryUIManager.Source.ResizeUIOnHandle((RectTransform)_chip.transform, _chip.ChipData);
                _chip.transform.SetSiblingIndex(4);
                _firsttouch = true;
            }
            ChipInventoryUIManager.Source.ApplyVisualRotation(_chipPivotRectTransform, _chip.CurrentRotationStep);
            _chipCanvasGroup.blocksRaycasts = false;
            _chipPivotRectTransform.position = Mouse.current.position.ReadValue();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _chipPivotRectTransform.anchoredPosition += eventData.delta / _chipCanva.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _chipCanvasGroup.blocksRaycasts = true;
            _isDragging = false;
            
            var slotUI = GetSlotUnderPointer(eventData, LayerMask.GetMask("SlotLayer"));

            if (slotUI != null) {
                TryPlaceChip(slotUI, _chip);
            }
            else
            {
                if (_chip.HasBeenPlaced)
                {
                    ChipInventoryUIManager.Source.SnapToLastSlot(_chip.CurrentRowPlaced, _chip.CurrentColumnPlaced, _chipPivotRectTransform, _chipCanva, _chip);
                }
                else
                {
                    ChipInventoryUIManager.Source.ReturnToDisplay(_chipPivotRectTransform, _chip);
                    _firsttouch = false;
                }
                print("Slot no encontrado");
            }
        }

        private void TryPlaceChip(GridSlotUI slot, Chip chip)
        {
            var newCoords = ChipInventoryManager.Source.GetNewChipCoordinates(slot.Y, slot.X, chip);

            if (!ChipInventoryManager.Source.IsWithinBounds(newCoords) && chip.HasBeenPlaced)
            {
                ChipInventoryUIManager.Source.SnapToLastSlot(chip.CurrentRowPlaced, chip.CurrentColumnPlaced, _chipPivotRectTransform, _chipCanva, chip);
                Debug.Log("❌ El chip no cabe en el grid (fuera de los límites)");
                return;
            }

            if (ChipInventoryManager.Source.CanPlaceChip(slot.Y, slot.X, chip) ||  ChipInventoryManager.Source.IsAlreadyAtPosition(newCoords, chip.CoordinatesOccupiedOnGrid))
            {
                if (!chip.HasBeenPlaced)
                {
                    ChipInventoryManager.Source.PlaceChip(slot.Y, slot.X, chip);
                    ChipInventoryUIManager.Source.SnapToSlot(slot, _chipPivotRectTransform, _chipCanva);
                    Debug.Log("✅ El chip ha sido colocado en el grid");
                }
                else
                {
                    ChipInventoryManager.Source.UnPlaceChip(chip);
                    ChipInventoryManager.Source.PlaceChip(slot.Y, slot.X, chip);
                    ChipInventoryUIManager.Source.SnapToSlot(slot, _chipPivotRectTransform, _chipCanva);
                    Debug.Log("🔄 El chip ha cambiado de lugar");
                }
            }
            else
            {
                ChipInventoryUIManager.Source.ReturnToDisplay(_chipPivotRectTransform, _chip);
                _firsttouch = false;
            }
        }

        public GridSlotUI GetSlotUnderPointer(PointerEventData eventData, LayerMask layerMask)
        {
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Filtramos por Layer y obtenemos el primero que tenga GridSlotUI
            var slotResult = results
                .Where(r => ((1 << r.gameObject.layer) & layerMask) != 0)
                .Select(r => r.gameObject.GetComponent<GridSlotUI>())
                .FirstOrDefault(slot => slot != null);

            return slotResult;
        }

        private void RotateChip()
        {
            /*if (_isDragging && _chip.ChipData.IsRotable)
            {
                ChipInventoryManager.Source.RotateChip(_chipPivotRectTransform, _chip);
                ChipInventoryUIManager.Source.ApplyVisualRotation(_chipPivotRectTransform, _chip.CurrentRotationStep);
            }*/
        }

        private void DeleteChipPlaced()
        {
            if(_isDragging && _chip.HasBeenPlaced)
            {
                ChipInventoryManager.Source.ReturnChipToList(_chipPivotRectTransform, _chip);
            }
        }
    }
}
