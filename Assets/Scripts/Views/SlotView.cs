using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace App1.Views
{
    public class SlotView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private GameObject _lockedOverlay;
        [SerializeField] private Button _slotButton;

        public event Action<int> OnSlotClicked;
        private int _index;

        public void Init(int index)
        {
            _index = index;
            _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(_index));
        }

        public void UpdateView(bool isUnlocked, Sprite icon, int amount)
        {
            _lockedOverlay.SetActive(!isUnlocked);
            
            if (!isUnlocked || icon == null)
            {
                _icon.gameObject.SetActive(false);
                _amountText.gameObject.SetActive(false);
                return;
            }

            _icon.gameObject.SetActive(true);
            _icon.sprite = icon;
            
            _amountText.gameObject.SetActive(amount > 0);
            _amountText.text = amount.ToString();
        }
    }
}