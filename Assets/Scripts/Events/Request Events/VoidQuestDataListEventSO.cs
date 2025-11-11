using UnityEngine;
using System.Collections.Generic;

namespace EventChannel
{
    [CreateAssetMenu(fileName = "VoidQuestDataListEventSO", menuName = "Events/Request/Void QuestDataList")]
    public class VoidQuestDataListEventSO : VoidGenericRequestEventSO<List<Characters.Quests.QuestData>> { }
}