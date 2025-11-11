using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Characters.Quests;

namespace UserInterface
{
    public class RewardEntry : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _quantityText;

        public void Init(Sprite icon, string name, int quantity)
        {
            _image.sprite = icon;
            _nameText.text = name;
            if (quantity > 1)
            {
                _quantityText.enabled = true;
                _quantityText.text = quantity.ToString();
            }
            else { _quantityText.enabled = false; }
        }

        public void Init(Sprite icon, string name)
        {
            _image.sprite = icon;
            _nameText.text = name;
            _quantityText.enabled = false; 
        }

        public void Init(string skill)
        {
            _nameText.text = skill;
        }

        public void DestroySelf() { Destroy(gameObject); }
    }
}