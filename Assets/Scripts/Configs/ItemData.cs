using UnityEngine;

namespace App1.Configs
{
    public abstract class ItemData : ScriptableObject
    {
        public string Id; // Уникальный ID для сохранений
        public string Name;
        public Sprite Icon;
        public float Weight;
        public int MaxStack = 1;
        public ItemType Type;
    }
}