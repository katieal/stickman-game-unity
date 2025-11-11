using UnityEngine;
using UnityEngine.UIElements;

public class TabTesting : MonoBehaviour
{
    private void OnEnable()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;
    }

}
