using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class PanelVisualizer : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private RectTransform _visualizerRectTransform;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        [Header("References")]
        [SerializeField] private ChipInfoPanel _chipInfoPanel;

        private GameObject _currentChip;
        private int _indexList = 0;

        public RectTransform VisualizerRectTransform => _visualizerRectTransform;

        private void Start()
        {
            InitializeVisualizer(); 
            SpawnChipIcon();
        }

        private void InitializeVisualizer()
        {
            _leftButton.onClick.AddListener(PreviousElement);
            _rightButton.onClick.AddListener(NextElement);
        }

        private void SpawnChipIcon()
        {
            if (ChipManager.Source.ChipsAvailables.Count == 0)
            {
                OnZeroChipsAvailables();
                return;
            }
            else
            {
                _leftButton.gameObject.SetActive(true);
                _rightButton.gameObject.SetActive(true);
            }

            if (_currentChip == null) _currentChip = Instantiate(ChipManager.Source.ChipPrefab, _visualizerRectTransform);

            ChipData data = ChipManager.Source.ChipsAvailables[_indexList];
            InventoryChipView itemview = _currentChip.GetComponent<InventoryChipView>();
            itemview.InitializeAsPreview(data);
            _chipInfoPanel.RefreshChipInformation(data.Name,data.Description);
        }

        private void PreviousElement()
        {
            if (ChipManager.Source.ChipsAvailables.Count == 0) return;
            _indexList = (_indexList - 1 + ChipManager.Source.ChipsAvailables.Count) % ChipManager.Source.ChipsAvailables.Count;

            SpawnChipIcon();
        }

        private void NextElement()
        {
            if (ChipManager.Source.ChipsAvailables.Count == 0) return;
            _indexList = (_indexList + 1) % ChipManager.Source.ChipsAvailables.Count;

            SpawnChipIcon();
        }

        public void OnChipPlaced()
        {
            ChipManager.Source.ChipsAvailables.Remove(ChipManager.Source.ChipsAvailables[_indexList]);
            _currentChip = null;
            _indexList = 0;
            SpawnChipIcon();
        }

        public void OnChipRemoved()
        {
            _indexList = 0;
            SpawnChipIcon();
        }

        private void OnZeroChipsAvailables()
        {
            _chipInfoPanel.RefreshChipInformation();
            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(false);
        }
    }
}
