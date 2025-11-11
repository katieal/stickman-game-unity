using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class ObjectiveEntry : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _objectiveText;
        [SerializeField] private TMP_Text _progressText;

        public void Init(string objective, string progress)
        {
            _objectiveText.text = objective;
            _progressText.text = progress;
        }

        public void DestroySelf() { Destroy(gameObject); }
    }
}