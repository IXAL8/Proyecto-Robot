using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Robot
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] private ItemData _item;
        
        [SerializeField] private int _quantity;

        public ItemData Item => _item;
        public int Quantity => _quantity;

        public void SetQuantity(int quantity)
        {
            _quantity = quantity;
        }
        
        public void ModifyQuantity(int amount)
        {
            int newQuantity = _quantity+amount;
            SetQuantity(newQuantity);
        }

        public InventorySlot() { } // ← necesario para la deserialización

        public InventorySlot(ItemData item, int quantity)
        {
            _item = item;
            _quantity = quantity;
        }
    }

    
    public class Inventory : Singleton<IInventorySource>, IInventorySource
    {
        [SerializeField] private InventorySaveFileData _saveFileData;
        [SerializeField] private List<InventorySlot> _itemsList;
        [SerializeField] private GameObject _inventoryUIPrefab;
        private GameObject _inventoryUI;
        public event Action ConsumableAdded;

        private void Start()
        {
            LoadFromSaveSystem();
            InputManager.Source.OpenInventory += ShowOrHideUI;
            InputManager.Source.CloseInventory += ShowOrHideUI;
        }

        public void LoadFromSaveSystem()
        {
            if (!_saveFileData)
            {
                _saveFileData = SaveSystemManager.Source.GetFileData<InventorySaveFileData>();
                if (!_saveFileData)
                {
                    _saveFileData = ScriptableObject.CreateInstance<InventorySaveFileData>();
                    SaveToSaveFile();
                }
            }
            
            _itemsList = _saveFileData._currentItemsList;
        }

        public void SaveToSaveFile()
        {
            _saveFileData._currentItemsList = _itemsList;
            SaveSystemManager.Source.SaveFileData(_saveFileData);
        }

        private void ShowOrHideUI()
        {
            if (!_inventoryUI) SetupUI();
            _inventoryUI.SetActive(!_inventoryUI.activeSelf);
        }

        private void SetupUI()
        {
            _inventoryUI = Instantiate(_inventoryUIPrefab,transform);
            _inventoryUI.SetActive(false);
        }

        public void AddItemToInventory(ItemData newItem, int quantity = 1)
        {
            ScriptablesDataBase.Source.AddToDatabase(newItem);

            //if (newItem is ChipData) ChipInventoryManager.Source.AddToAvailableChips((ChipData)newItem);

            if (IsItemInInventory(newItem))
            {
                ModifyItemQuantity(newItem, quantity);
            }
            else
            {
                _itemsList.Add(new InventorySlot(newItem, quantity));
            }
            
            if (newItem is ConsumableData)
            {
                ConsumableAdded?.Invoke();
            }

            SaveToSaveFile();
        }

        public void RemoveItemFromInventory(ItemData newItem, int quantity = -1)
        {
            ModifyItemQuantity(newItem, quantity);
            SaveToSaveFile();
        }

        private void ModifyItemQuantity(ItemData newItem, int quantity)
        {
            foreach (var item in _itemsList)
            {
                if (item.Item != newItem) continue;
                item.ModifyQuantity(quantity);
                return;
            }
        }

        public bool IsItemInInventory(ItemData newItem)
        {
            foreach (var item in _itemsList)
            {
                if(item.Item == newItem) return true;
            }

            return false;
        }

        public bool IsItemQuantityInInventory(ItemData newItem, int quantity)
        {
            foreach (var item in _itemsList)
            {
                if (item.Item == newItem)
                {
                    return item.Quantity >= quantity;
                }
            }

            return false;
        }

        public int GetItemQuantity(ItemData item)
        {
            foreach (var i in _itemsList)
            {
                if(i.Item == item) return i.Quantity;
            }

            return 0;
        }

        public List<T> GetItemsOfType<T>() where T : ItemData
        {
            var result = new List<T>();
            foreach (var a in _itemsList)
            {
                if (a.Item.GetType() == typeof(T))
                {
                    for (int i = 0; i < a.Quantity; i++)
                    {
                        result.Add(a.Item as T);
                    }
                }
            }
            
            return result;
        }

        public List<T> GetUniqueItemsOfType<T>() where T : ItemData
        {
            var result = new List<T>();
            foreach (var a in _itemsList)
            {
                if (a.Item.GetType() == typeof(T) && a.Quantity > 0)
                {
                    result.Add(a.Item as T);
                }
            }
            
            return result;
        }

        public List<InventorySlot> GetInventory()
        {
            return _itemsList;
        }
    }
}
