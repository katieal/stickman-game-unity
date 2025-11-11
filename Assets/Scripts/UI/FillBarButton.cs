using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

using EventChannel;

namespace UserInterface
{
    public class FillBarButton : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private Button _leftArrow;
        [SerializeField] private Button _rightArrow;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Vector2 _sliderRange;
        [SerializeField] private int _segmentCount;

        [Title("Invoked Events")]
        [Tooltip("using an event for now, not sure if it will stay this way")]
        [SerializeField] private FloatEventSO _onFillChangedSO;

        [ShowInInspector] private int _fillCount = 6;
        [ShowInInspector] private float _fillChangeAmount;
        private float _segmentValue;

        private void Awake()
        {
            _fillChangeAmount = Mathf.Clamp01((100f / _segmentCount) / 100f);
            _segmentValue = (Mathf.Abs(_sliderRange.x) + Mathf.Abs(_sliderRange.y)) / _segmentCount;
        }

        private void OnEnable()
        {
            _leftArrow.onClick.AddListener(() => OnLeftArrow());
            _rightArrow.onClick.AddListener(() => OnRightArrow());
        }

        private void OnDisable()
        {
            _leftArrow.onClick.RemoveAllListeners();
            _rightArrow.onClick.RemoveAllListeners();
        }

        private void OnLeftArrow()
        {
            // do nothing if bar is already empty
            if (_fillCount == 0) { return; }

            _fillImage.fillAmount -= _fillChangeAmount;
            _fillCount--;
            // notify value changed
            if (_onFillChangedSO != null) { _onFillChangedSO.InvokeEvent(_sliderRange.x + (_fillCount * _segmentValue)); }
        }
        private void OnRightArrow()
        {
            // no nothing if bar is already full
            if (_fillCount == _segmentCount) { return; }

            _fillImage.fillAmount += _fillChangeAmount;
            _fillCount++;
            // notify value changed
            if (_onFillChangedSO != null) { _onFillChangedSO.InvokeEvent(_sliderRange.x + (_fillCount * _segmentValue)); }
        }
    }
}