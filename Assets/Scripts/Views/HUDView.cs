using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace App1.Views
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _weightText;
        
        [SerializeField] private Button _btnShoot;
        [SerializeField] private Button _btnAddAmmo;
        [SerializeField] private Button _btnAddItem;
        [SerializeField] private Button _btnRemoveItem;
        [SerializeField] private Button _btnAddCoins;

        public event Action OnShoot;
        public event Action OnAddAmmo;
        public event Action OnAddItem;
        public event Action OnRemoveItem;
        public event Action OnAddCoins;

        private void Awake()
        {
            _btnShoot.onClick.AddListener(() => OnShoot?.Invoke());
            _btnAddAmmo.onClick.AddListener(() => OnAddAmmo?.Invoke());
            _btnAddItem.onClick.AddListener(() => OnAddItem?.Invoke());
            _btnRemoveItem.onClick.AddListener(() => OnRemoveItem?.Invoke());
            _btnAddCoins.onClick.AddListener(() => OnAddCoins?.Invoke());
        }

        public void UpdateStats(int coins, float weight)
        {
            _coinsText.text = $"Монеты: {coins}";
            _weightText.text = $"Вес: {weight:F2} кг";
        }
    }
}