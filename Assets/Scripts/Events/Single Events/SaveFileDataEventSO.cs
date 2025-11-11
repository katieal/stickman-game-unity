using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "SaveFileDataEventSO", menuName = "Events/Single/SaveFileData")]
    public class SaveFileDataEventSO : GenericSingleEventSO<SaveData.SaveFileData> { }
}