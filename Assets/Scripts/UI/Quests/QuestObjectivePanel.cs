using Characters.Quests;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace UserInterface
{
    [Serializable]
    public struct ObjectiveStruct
    {
        [ChildGameObjectsOnly] public GameObject Panel;
        [ChildGameObjectsOnly] public TMP_Text ObjectiveText;
        [ChildGameObjectsOnly] public TMP_Text ProgressText;
    }

    public class QuestObjectivePanel : SerializedMonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private TMP_Text _objectiveLabel;
        [SerializeField][RequiredListLength(4)] private List<ObjectiveStruct> _objectiveEntries = new();

        public void SetObjectives(List<string> missions)
        {
            // make label singular or plural
            if (missions.Count > 1) { _objectiveLabel.text = "Objectives"; }
            else { _objectiveLabel.text = "Objective"; }

            for (int index = 0; index < _objectiveEntries.Count; index++)
            {
                if (index < missions.Count)
                {
                    // enable panel
                    _objectiveEntries[index].Panel.SetActive(true);

                    //SetObjective(_objectiveEntries[index], objectives[index]);

                    // set mission text - add progress later
                    _objectiveEntries[index].ObjectiveText.text = missions[index];
                    _objectiveEntries[index].ProgressText.enabled = false;
                }
                else
                {
                    // if number of objectives is less than 4, disable extra objective entries
                    _objectiveEntries[index].Panel.SetActive(false);
                }
            }
        }

        /*
        private void SetObjective(ObjectiveStruct entry, ObjectiveData data)
        {
            entry.ObjectiveText.text = data.Objective;

            // show objective with current progress
            // TODO: change text to green or something if quest can be completed???
            string progress = "(" + data.Current.ToString() + "/" + data.Total.ToString() + ")";
            entry.ProgressText.text = progress;
        }
        */
    }
}
