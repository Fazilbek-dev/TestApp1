using UnityEngine;
using App1.Models;
using App1.Views;
using App1.Presenters;
using App1.Services;
using App1.Configs;

namespace App1
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private ItemDatabase _database;
        [SerializeField] private HUDView _hudView;
        [SerializeField] private Transform _inventoryGrid; 
        [SerializeField] private SlotView _slotPrefab;

        private InventoryPresenter _presenter;

        private void Start()
        {
            SlotView[] slots = new SlotView[30];
            for (int i = 0; i < 30; i++)
            {
                slots[i] = Instantiate(_slotPrefab, _inventoryGrid);
            }

            var saveSystem = new JsonSaveSystem();
            var model = new InventoryModel(saveSystem, _database);

            _presenter = new InventoryPresenter(model, _hudView, slots, _database);
        }
    }
}