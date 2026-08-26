using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class InventoryGridView : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private RectTransform _gridPanelRectTransform;
        [SerializeField] private RectTransform _itemsContainerRectTransform;
        [SerializeField] private GridLayoutGroup _gridPanelLayoutGroup;
        [SerializeField] private GameObject _cellPrefab;

        private InventoryGrid _logicGrid;
        private Image[,] _cellBackgrounds;

        public float CellSize => _gridPanelLayoutGroup.cellSize.x + _gridPanelLayoutGroup.spacing.x;
        public RectTransform RectTransform => _gridPanelRectTransform;
        public RectTransform ItemsContainer => _itemsContainerRectTransform;

        public void Initialize(InventoryGrid grid)
        {
            _logicGrid = grid;
            _cellBackgrounds = new Image[_logicGrid.Width, _logicGrid.Height];

            StretchToFillParent(_gridPanelRectTransform);
            StretchToFillParent(_itemsContainerRectTransform);

            _gridPanelLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridPanelLayoutGroup.constraintCount = grid.Width;

            for (int y = 0; y < _logicGrid.Height; y++)
            {
                for (int x = 0; x < _logicGrid.Width; x++)
                {
                    GameObject Cell = Instantiate(_cellPrefab, _gridPanelRectTransform);
                    _cellBackgrounds[x, y] = Cell.GetComponent<Image>();
                }
            }
        }

        public void HighlightCells(bool[,] shape, Vector2Int origin, bool valid)
        {
            ClearHighlights();

            int shapeWidth = shape.GetLength(0);
            int shapeHeight = shape.GetLength(1);
            Color highlightColor = valid ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);

            for (int x = 0; x < shapeWidth; x++)
            {
                for (int y = 0; y < shapeHeight; y++)
                {
                    if (!shape[x, y]) continue;

                    int gx = origin.x + x;
                    int gy = origin.y + y;

                    if (gx >= 0 && gx < _logicGrid.Width && gy >= 0 && gy < _logicGrid.Height)
                        _cellBackgrounds[gx, gy].color = highlightColor;
                }
            }
        }

        public void ClearHighlights()
        {
            for (int x = 0; x < _logicGrid.Width; x++)
                for (int y = 0; y < _logicGrid.Height; y++)
                    _cellBackgrounds[x, y].color = Color.white;
        }

        public Vector2Int WorldToGridPosition(Vector2 localPointInItemsContainer)
        {
            float cellSize = CellSize;
            int x = Mathf.FloorToInt(localPointInItemsContainer.x / cellSize);
            int y = Mathf.FloorToInt(-localPointInItemsContainer.y / cellSize);
            return new Vector2Int(x, y);
        }

        public void StretchToFillParent(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
