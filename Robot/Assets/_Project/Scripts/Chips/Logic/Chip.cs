using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Robot
{
    public class Chip : MonoBehaviour
    {
        [SerializeField] private ChipData _chipData;
        [SerializeField] private int _currentColumnPlaced, _currentRowPlaced, _currentRotationStep;
        [SerializeField] private bool _hasBeenPlaced;
        [SerializeField] private List<Vector2Int> _coordinatesOccupiedOnGrid;

        private List<Vector2Int> _shapeRotation;
        private List<Vector2Int> _lastShapeRotation;
        private int _rotationStepsBeforeRotation;
        private Vector2 _sizeDeltaChip;


        public int CurrentColumnPlaced => _currentColumnPlaced;
        public int CurrentRowPlaced => _currentRowPlaced;
        public int CurrentRotationStep => _currentRotationStep;
        public ChipData ChipData => _chipData;
        public bool HasBeenPlaced => _hasBeenPlaced;
        public List<Vector2Int> CoordinatesOccupiedOnGrid => _coordinatesOccupiedOnGrid;
        public List<Vector2Int> ShapeRotation => _shapeRotation;
        public Vector2 SizeDeltaChip => _sizeDeltaChip;

        private void Start()
        {
            InitializeChip();
        }

        public void SetPlaced(bool value)
        {
            _hasBeenPlaced = value;
        }

        public void SaveCoordinates(Vector2Int coords)
        {
            _coordinatesOccupiedOnGrid.Add(coords);
        }

        public void SetRowAndColumn(int row, int column)
        {
            _currentRowPlaced = row;
            _currentColumnPlaced = column;
        }

        public void SaveCurrentStepAndShape()
        {
            _lastShapeRotation = new List<Vector2Int>(_shapeRotation);
            _rotationStepsBeforeRotation = _currentRotationStep;
        }

        public void RestoreLastStepAndShape() 
        {
            if (_lastShapeRotation.Count > 0)
            {
                _shapeRotation = new List<Vector2Int>(_lastShapeRotation);
                _currentRotationStep = _rotationStepsBeforeRotation;
            }
        }

        public void ChangeRotationStep()
        {
            _currentRotationStep = (_currentRotationStep + 1) % 4;
        }

        public void AssignChipData(ChipData chip)
        {
            _chipData = chip;
            //_currentRotationStep = _chipData.RotationSteps;
            //_shapeRotation = new List<Vector2Int>(_chipData.Shape);
        }

        public void InitializeChip()
        {
            _sizeDeltaChip = gameObject.GetComponent<RectTransform>().sizeDelta;
            //gameObject.GetComponent<Image>().sprite = _chipData.ChipSprite;
            //_shapeRotation = new List<Vector2Int>(_chipData.Shape);
            _coordinatesOccupiedOnGrid = new List<Vector2Int>();
            _lastShapeRotation = new List<Vector2Int>();
        }
    }
}
