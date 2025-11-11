using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class QuestLogEntry : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Outline _outline;
        [SerializeField] private Toggle _toggle;

        [field: SerializeField] public Button Button { get; private set; }


        public void Init(string questName) 
        { 
            _titleText.text = questName; 
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected) { _outline.enabled = true; }
            else { _outline.enabled = false; }
        }

        public void DestroySelf() { Destroy(gameObject); }
    }
}