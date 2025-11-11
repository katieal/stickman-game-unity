using UnityEngine;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "QuestBoolUniTaskEventSO", menuName = "Events/UniTask/Quest Bool")]
    public class QuestBoolUniTaskEventSO : GenericGenericUniTaskEventSO<Characters.Quests.QuestData, bool> { }
}