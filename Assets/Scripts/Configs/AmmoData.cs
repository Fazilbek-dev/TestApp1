using UnityEngine;

namespace App1.Configs
{
    [CreateAssetMenu(fileName = "AmmoData", menuName = "App1/Items/Ammo")]
    public class AmmoData : ItemData
    {
        // Специфичные поля для патронов. Например, множитель урона или тип калибра.
        public float DamageMultiplier = 1.0f; 
        public string Caliber;
    }
}