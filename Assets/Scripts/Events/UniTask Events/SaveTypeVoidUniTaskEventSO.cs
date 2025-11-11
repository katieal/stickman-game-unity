using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "SaveTypeVoidUniTaskEventSO", menuName = "Events/UniTask/Saving/SaveType Void")]
    public class SaveTypeVoidUniTaskEventSO : GenericVoidUniTaskEventSO<SaveData.SaveType> { }
}