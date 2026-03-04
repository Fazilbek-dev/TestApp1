using System;
using System.Linq;
using App1.Services;

namespace App1.Models
{
    public class InventoryModel
    {
        public InventorySaveData Data { get; private set; }
        public float TotalWeight { get; private set; }
        
        public event Action OnInventoryUpdated;
        public event Action<string> OnLogMessage;

        private JsonSaveSystem _saveSystem;
        private Configs.ItemDatabase _database;

        public InventoryModel(JsonSaveSystem saveSystem, Configs.ItemDatabase database)
        {
            _saveSystem = saveSystem;
            _database = database;
            LoadOrInitialize();
        }

        private void LoadOrInitialize()
        {
            Data = _saveSystem.Load();
            if (Data == null)
            {
                Data = new InventorySaveData { Coins = 0 };
                for (int i = 0; i < 30; i++)
                {
                    Data.Slots.Add(new SlotSaveData { ItemId = "", Amount = 0, IsUnlocked = i < 15 });
                }
            }
            RecalculateWeight();
        }
        
        public int AddItem(string itemId, int amount)
        {
            if (amount <= 0) return 0;

            var itemConfig = GetItemData(itemId);
            if (itemConfig == null)
            {
                OnLogMessage?.Invoke($"Ошибка: Предмет с ID '{itemId}' не найден в базе данных.");
                return amount; 
            }

            int remainingAmount = amount;

            // Шаг 1: Если предмет стакается, ищем УЖЕ существующие неполные стаки
            if (itemConfig.MaxStack > 1)
            {
                // Ищем только в разблокированных слотах, где уже лежит этот предмет и есть место
                var partialSlots = Data.Slots.Where(s => 
                    s.IsUnlocked && 
                    s.ItemId == itemId && 
                    s.Amount < itemConfig.MaxStack);

                foreach (var slot in partialSlots)
                {
                    int spaceInSlot = itemConfig.MaxStack - slot.Amount;
                    int amountToAdd = Math.Min(spaceInSlot, remainingAmount);

                    slot.Amount += amountToAdd;
                    remainingAmount -= amountToAdd;

                    if (remainingAmount <= 0) break;
                }
            }

            // Шаг 2: Если предметы еще остались, ищем ПУСТЫЕ разблокированные слоты
            if (remainingAmount > 0)
            {
                // Пустой слот мы определяем по пустой строке
                var emptySlots = Data.Slots.Where(s => 
                    s.IsUnlocked && 
                    string.IsNullOrEmpty(s.ItemId));

                foreach (var slot in emptySlots)
                {
                    int amountToAdd = Math.Min(itemConfig.MaxStack, remainingAmount);

                    slot.ItemId = itemId;
                    slot.Amount = amountToAdd;
                    remainingAmount -= amountToAdd;

                    if (remainingAmount <= 0) break;
                }
            }

            // Шаг 3: Проверяем, влезло ли всё
            if (remainingAmount > 0)
            {
                OnLogMessage?.Invoke($"Инвентарь полон! Не удалось добавить {remainingAmount} шт. предмета '{itemConfig.Name}'.");
            }
            else
            {
                OnLogMessage?.Invoke($"Успешно добавлено {amount} шт. предмета '{itemConfig.Name}'.");
            }

            NotifyAndSave();
            return remainingAmount; // Возвращаем остаток, чтобы выбросить его обратно на землю, если нужно
        }
        
        public bool RemoveItem(string itemId, int amount)
        {
            if (amount <= 0) return false;

            // Сначала проверяем, есть ли вообще в инвентаре нужное количество
            int totalAvailable = Data.Slots.Where(s => s.ItemId == itemId).Sum(s => s.Amount);
            if (totalAvailable < amount)
            {
                OnLogMessage?.Invoke($"Невозможно удалить {amount} шт. предмета '{itemId}'. В наличии только {totalAvailable}.");
                return false;
            }

            int remainingToRemove = amount;

            // Проходим по слотам, где есть этот предмет, и вычитаем
            // .ToList() нужен, чтобы безопасно менять элементы коллекции
            var slotsWithItem = Data.Slots.Where(s => s.ItemId == itemId).ToList();

            foreach (var slot in slotsWithItem)
            {
                if (slot.Amount >= remainingToRemove)
                {
                    slot.Amount -= remainingToRemove;
                    remainingToRemove = 0;
                }
                else
                {
                    remainingToRemove -= slot.Amount;
                    slot.Amount = 0;
                }

                // ВАЖНО: Если количество опустилось до нуля, "очищаем" слот
                if (slot.Amount == 0)
                {
                    slot.ItemId = ""; // Возвращаем ту самую пустую строку-маркер
                }

                if (remainingToRemove == 0) break;
            }

            NotifyAndSave();
            return true;
        }

        public void AddCoins(int amount)
        {
            Data.Coins += amount;
            NotifyAndSave();
        }

        public void UnlockSlot(int index)
        {
            if (Data.Coins >= _database.SlotUnlockCost && !Data.Slots[index].IsUnlocked)
            {
                Data.Coins -= _database.SlotUnlockCost;
                Data.Slots[index].IsUnlocked = true;
                NotifyAndSave();
            }
        }

        public void Shoot()
        {
            var weaponSlot = Data.Slots.FirstOrDefault(s => GetItemData(s.ItemId) is Configs.WeaponData);
            if (weaponSlot != null)
            {
                var weapon = (Configs.WeaponData)GetItemData(weaponSlot.ItemId);
                var ammoSlot = Data.Slots.FirstOrDefault(s => s.ItemId == weapon.RequiredAmmo.Id && s.Amount > 0);
                
                if (ammoSlot != null)
                {
                    ammoSlot.Amount--;
                    if (ammoSlot.Amount == 0) ammoSlot.ItemId = "";
                    OnLogMessage?.Invoke($"Выстрел из {weapon.Name}, патрон: {weapon.RequiredAmmo.Name}, урон: {weapon.Damage}");
                    NotifyAndSave();
                    return;
                }
            }
            OnLogMessage?.Invoke("Ошибка: не удалось сделать выстрел (нет подходящего оружия или патронов).");
        }

        private Configs.ItemData GetItemData(string id) => _database.AllItems.FirstOrDefault(i => i.Id == id);

        private void RecalculateWeight()
        {
            TotalWeight = 0;
            foreach (var slot in Data.Slots)
            {
                if (!string.IsNullOrEmpty(slot.ItemId))
                {
                    TotalWeight += GetItemData(slot.ItemId).Weight * slot.Amount;
                }
            }
        }

        private void NotifyAndSave()
        {
            RecalculateWeight();
            _saveSystem.Save(Data);
            OnInventoryUpdated?.Invoke();
        }
    }
}