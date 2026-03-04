using System;
using System.Collections.Generic;

namespace App1.Models
{
    [Serializable]
    public class InventorySaveData
    {
        public int Coins;
        public List<SlotSaveData> Slots = new List<SlotSaveData>();
    }
}