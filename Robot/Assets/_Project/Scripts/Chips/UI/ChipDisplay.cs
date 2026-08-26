using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class ChipDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _chipName, _chipDescription;
        [SerializeField] private Button _leftArrow, _rightArrow;

        private void OnEnable()
        {
            ChipInventoryManager.Source.OnChipSpawned += RefreshChipDescription;
            ChipInventoryManager.Source.OnListEmpty += ListEmptyEvent;
            ChipInventoryManager.Source.OnListNotEmpty += ListNotEmptyEvent;
        }

        private void OnDisable()
        {
            ChipInventoryManager.Source.OnChipSpawned -= RefreshChipDescription;
            ChipInventoryManager.Source.OnListEmpty -= ListEmptyEvent;
            ChipInventoryManager.Source.OnListNotEmpty -= ListNotEmptyEvent;
        }

        public void RefreshChipDescription()
        {
            var currentChipObject = ChipInventoryManager.Source.CurrentChipOnDisplay;

            if (currentChipObject == null)
            {
                ListEmptyEvent();
                return;
            }

            var chip = currentChipObject.GetComponentInChildren<Chip>();

            if (chip != null)
            {
                //_chipName.text = chip.ChipData.ChipName;
                //_chipDescription.text = chip.ChipData.ChipEffectDescription;
            }
            else
            {
                Debug.LogWarning("El objeto actual no tiene componente 'Chip', ejecutando ListEmptyEvent()");
                ListEmptyEvent();
            }

        }

        public void LeftArrow()
        {
            ChipInventoryManager.Source.PreviousChipData();
            RefreshChipDescription();
        }

        public void RightArrow()
        {
            ChipInventoryManager.Source.NextChipData();
            RefreshChipDescription();
        }

        private void ListEmptyEvent()
        {
            _chipName.text = string.Empty;
            _chipDescription.text = string.Empty;
            _leftArrow.gameObject.SetActive(false);
            _rightArrow.gameObject.SetActive(false);
        }

        private void ListNotEmptyEvent()
        {
            _leftArrow.gameObject.SetActive(true);
            _rightArrow.gameObject.SetActive(true);
        }
    }
}
