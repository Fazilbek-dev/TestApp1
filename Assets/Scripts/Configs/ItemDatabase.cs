using System.Collections.Generic;
using UnityEngine;

namespace App1.Configs
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "App1/ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemData> AllItems;
        public int SlotUnlockCost = 100; // Настраиваемая цена разблокировки
        public int AmmoStackSize = 50; // Настраиваемый стак патронов
    }
}