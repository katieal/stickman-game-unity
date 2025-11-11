using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

namespace Shop
{
    public class ShopButton : MonoBehaviour
    {
        [field: Header("Components")]
        [field: SerializeField] public Button Button { get; private set; }
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private int _characterLimit = 23;

        public void Init(Sprite icon, string name, int price)
        {
            Debug.Assert((name.Length <= _characterLimit), "Item name too long: " + name);

            _image.sprite = icon;
            _nameText.text = name;
            _priceText.text = price.ToString();
        }


        public void FlashRed()
        {
            //Button.
        }
    }
}
