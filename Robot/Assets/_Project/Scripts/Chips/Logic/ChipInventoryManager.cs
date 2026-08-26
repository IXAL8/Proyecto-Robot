using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Robot
{
    public class ChipInventoryManager : Singleton<IInventoryChipSource>, IInventoryChipSource
    {
        [SerializeField] private int _gridRows, _gridColumns;
        [Header("ChipDisplaySpawn")]
        [SerializeField] private GameObject _currentChipOnDisplay;
        //[SerializeField] private GameObject _chipPrefab;
        [SerializeField] private List<ChipData> _availableChips;


        private bool[,] _inventoryGrid;
        private int _listIndex;
        private Dictionary<string, Chip> _activeChips = new Dictionary<string, Chip>();

        public event Action<int, int> OnSlotOccupied;
        public event Action<int, int> OnSlotFreed;
        public event Action OnChipSpawned;
        public event Action OnListEmpty;
        public event Action OnListNotEmpty;
        public event Action<Chip> OnChipPlaced;
        public event Action<Chip> OnChipRemoved;

        public int InventoryRows => _gridRows;
        public int InventoryColumns => _gridColumns;
        public GameObject CurrentChipOnDisplay => _currentChipOnDisplay;

        private void Start()
        {
            LateStartSystem.ExecuteOnLateStart(InitializeChipsSystem);
        }

        private void InitializeChipsSystem()
        {
            GetChipsFromInventory();
            _inventoryGrid = new bool[_gridRows, _gridColumns];
            print("Inventario inicializado con: " + _inventoryGrid.Length);
            SpawnNewChip();
            PrintInventory();
        }

        private void GetChipsFromInventory()
        {
            _activeChips.Clear();
            //_availableChips = Inventory.Source.GetItemsOfType<ChipData>();
            if (_availableChips.Count == 0)
            {
                Debug.Log("No Chips In Inventory");
            }
        }

        public bool CanPlaceChip(int row, int column, Chip chip)
        {
            foreach (var offset in chip.ShapeRotation)
            {
                int x = row + offset.x;
                int y = column + offset.y;

                if (x >= _gridRows || y >= _gridColumns || x < 0 || y < 0) return false;
                if (_inventoryGrid[x, y]) return false;
            }
            return true;
        }

        public void PlaceChip(int row, int column, Chip chip)
        {
            chip.SetRowAndColumn(row, column);
            OccupySlotsForChip(chip, row, column);

            chip.SaveCurrentStepAndShape();
            _currentChipOnDisplay = null;
            PrintInventory();
            if (chip.HasBeenPlaced) return;

            ApplyChipEffects(chip);
            HandleChipInventory(chip);
            ChipInventoryUIManager.Source.RefreshPlayerStats();

            PrintInventory();
        }

        public void UnPlaceChip(Chip chip)
        {
            FreeChipSlots(chip);
            chip.CoordinatesOccupiedOnGrid.Clear();
            PrintInventory();
        }

        public bool IsWithinBounds(List<Vector2Int> coordinates)
        {
            foreach (var coord in coordinates)
            {
                if (coord.x < 0 || coord.y < 0 || coord.x >= _gridRows || coord.y >= _gridColumns)
                    return false;
            }
            return true;
        }

        public void RotateChip(Transform chipPivot, Chip chip)
        {
            for (int i = 0; i < chip.ShapeRotation.Count; i++)
            {
                var offset = chip.ShapeRotation[i];
                chip.ShapeRotation[i] = new Vector2Int(-offset.y, offset.x);
            }

            chip.ChangeRotationStep();

            chipPivot.position = Mouse.current.position.ReadValue();
        }

        public bool IsAlreadyAtPosition(List<Vector2Int> newCoords, List<Vector2Int> coordinatesOccupiedOnGrid)
        {
            return newCoords.Any(newCoord => coordinatesOccupiedOnGrid.Contains(newCoord));
        }

        public List<Vector2Int> GetNewChipCoordinates(int row, int column,Chip chip)
        {
            var coords = new List<Vector2Int>();
            foreach (var cell in chip.ShapeRotation)
            {
                coords.Add(new Vector2Int(row + cell.x, column + cell.y));
            }
            return coords;
        }

        private void SpawnNewChip()
        { 
            if(_availableChips.Count == 0)
            {
                OnListEmpty?.Invoke();
            }
            else
            {
                //_currentChipOnDisplay = Instantiate(_chipPrefab, ChipInventoryUIManager.Source.DisplayRectTransform);
                _listIndex = 0;
                _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
                OnChipSpawned?.Invoke();
            }

        }

        public void NextChipData()
        {
            _listIndex = (_listIndex + 1) % _availableChips.Count;
            _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
            _currentChipOnDisplay.GetComponentInChildren<Chip>().InitializeChip();
        }

        public void PreviousChipData()
        {
            _listIndex = (_listIndex - 1 + _availableChips.Count) % _availableChips.Count;
            _currentChipOnDisplay.GetComponentInChildren<Chip>().AssignChipData(_availableChips[_listIndex]);
            _currentChipOnDisplay.GetComponentInChildren<Chip>().InitializeChip();
        }

        private void OccupySlotsForChip(Chip chip, int startRow, int startColumn)
        {
            foreach (var offset in chip.ShapeRotation)
            {
                int x = startRow + offset.x;
                int y = startColumn + offset.y;

                _inventoryGrid[x, y] = true;
                chip.SaveCoordinates(new Vector2Int(x, y));
                OnSlotOccupied?.Invoke(y, x);
            }
        }

        private void FreeChipSlots(Chip chip)
        {
            foreach (var coord in chip.CoordinatesOccupiedOnGrid)
            {
                _inventoryGrid[coord.x, coord.y] = false;
                OnSlotFreed?.Invoke(coord.y, coord.x);
            }
        }

        private void ApplyChipEffects(Chip chip)
        {
            //PlayerStatsManager.Source.AddModifierToPlayer(chip.ChipData.BonusStatsChip);
            ChipInventoryUIManager.Source.RefreshPlayerStats();
            OnChipPlaced?.Invoke(chip);
            chip.SetPlaced(true);
        }

        private void RemoveChipEffects(Chip chip)
        {
            //PlayerStatsManager.Source.SubstractModifierToPlayer(chip.ChipData.BonusStatsChip);
            ChipInventoryUIManager.Source.RefreshPlayerStats();
            OnChipRemoved?.Invoke(chip);
        }

        public void ReturnChipToList(RectTransform pivotChip, Chip chip)
        {
            RemoveChipEffects(chip);
            UnPlaceChip(chip);
            OnListNotEmpty?.Invoke();
            Destroy(pivotChip.gameObject);

            if(_availableChips.Count <= 0)
            {
                _availableChips.Add(chip.ChipData);
                SpawnNewChip();
            }
            else if(_availableChips.Count > 0)
            {
                _availableChips.Add(chip.ChipData);
            }

        }

        private void HandleChipInventory(Chip chip)
        {
            _availableChips.RemoveAt(_listIndex);

            if (_availableChips.Count > 0)
            {
                OnListNotEmpty?.Invoke();
                SpawnNewChip();
            }
            else
            {
                OnListEmpty?.Invoke();
                Debug.Log("Ya no hay chips disponibles.");
            }
        }

        public void AddToAvailableChips(ChipData chipData)
        {
            _availableChips.Add(chipData);
            if (_currentChipOnDisplay == null) 
            {
                SpawnNewChip();
            }
        }

        private void PrintInventory()
        {
            string output = "";
            for (int y = 0; y < _gridRows; y++)
            {
                for (int x = 0; x < _gridColumns; x++)
                {
                    output += _inventoryGrid[y, x] ? "X " : ". ";
                }
                output += "\n";
            }
            Debug.Log(output);
        }
    }
}
