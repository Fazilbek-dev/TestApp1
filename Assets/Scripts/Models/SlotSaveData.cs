using System;

namespace App1.Models
{
    [Serializable]
    public class SlotSaveData
    {
        public string ItemId;
        public int Amount;
        public bool IsUnlocked;
    }
}