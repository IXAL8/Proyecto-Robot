using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class N_PanelVisualizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _visualizerRectTransform;

        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

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
            if (N_ChipManager.Source.ChipsAvailables.Count == 0) return;

            if (_currentChip == null) _currentChip = Instantiate(N_ChipManager.Source.ChipPrefab, _visualizerRectTransform);

            N_ChipsData data = N_ChipManager.Source.ChipsAvailables[_indexList];
            N_InventoryItemView itemview = _currentChip.GetComponent<N_InventoryItemView>();
            itemview.InitializeAsPreview(data);
        }

        private void PreviousElement()
        {
            if (N_ChipManager.Source.ChipsAvailables.Count == 0) return;
            _indexList = (_indexList - 1 + N_ChipManager.Source.ChipsAvailables.Count) % N_ChipManager.Source.ChipsAvailables.Count;

            SpawnChipIcon();
        }

        private void NextElement()
        {
            if (N_ChipManager.Source.ChipsAvailables.Count == 0) return;
            _indexList = (_indexList + 1) % N_ChipManager.Source.ChipsAvailables.Count;

            SpawnChipIcon();
        }

        public void OnChipPlaced()
        {
            N_ChipManager.Source.ChipsAvailables.Remove(N_ChipManager.Source.ChipsAvailables[_indexList]);
            _currentChip = null;
            SpawnChipIcon();
        }
    }
}
