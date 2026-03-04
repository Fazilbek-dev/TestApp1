using App1.Models;
using App1.Views;
using App1.Configs;
using UnityEngine;
using System.Linq;

namespace App1.Presenters
{
    public class InventoryPresenter
    {
        private InventoryModel _model;
        private HUDView _hudView;
        private SlotView[] _slots;
        private ItemDatabase _database;
        
        private int _lastClickedSlotIndex;

        public InventoryPresenter(InventoryModel model, HUDView hudView, SlotView[] slots, ItemDatabase database)
        {
            _model = model;
            _hudView = hudView;
            _slots = slots;
            _database = database;

            _hudView.OnShoot += _model.Shoot;
            _hudView.OnAddCoins += () => _model.AddCoins(50); 
            _hudView.OnAddItem += () => _model.AddItem(_database.AllItems[Random.Range(0, _database.AllItems.Count)].Id, 1);
            _hudView.OnRemoveItem += () => _model.RemoveItem("wpn_rifle", 1);

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i].Init(i);
                _slots[i].OnSlotClicked += HandleSlotClick;
            }

            _model.OnInventoryUpdated += RefreshUI;
            _model.OnLogMessage += Debug.Log;

            RefreshUI(); 
        }

        private void HandleSlotClick(int index)
        {
            if (!_model.Data.Slots[index].IsUnlocked)
            {
                _model.UnlockSlot(index);
            }

            _lastClickedSlotIndex = index;
        }

        private void RefreshUI()
        {
            _hudView.UpdateStats(_model.Data.Coins, _model.TotalWeight);

            for (int i = 0; i < _slots.Length; i++)
            {
                var slotData = _model.Data.Slots[i];
                Sprite icon = null;
                
                if (!string.IsNullOrEmpty(slotData.ItemId))
                {
                    var itemConfig = _database.AllItems.FirstOrDefault(item => item.Id == slotData.ItemId);
                    if (itemConfig != null)
                    {
                        icon = itemConfig.Icon;
                    }
                }
                
                _slots[i].UpdateView(slotData.IsUnlocked, icon, slotData.Amount);
            }
        }
    }
}