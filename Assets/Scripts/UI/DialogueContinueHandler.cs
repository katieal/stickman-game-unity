using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UserInterface
{
    public class DialogueContinueHandler : MonoBehaviour, IPointerClickHandler
    {
        [Title("Components")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProTypewriterEffect _typewriter;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_typewriter.isPlaying) 
            {
                // if typewriter is playing, stop typewriter effect and display entire text
                _typewriter.Stop();
            }
            else
            {
                // otherwise, invoke continue button
                _continueButton.onClick.Invoke();
            }
        }
    }
}