using UnityEngine;

namespace App1.Configs
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "App1/Items/Weapon")]
    public class WeaponData : ItemData
    {
        public AmmoData RequiredAmmo;
        public int Damage;
    }
}