using UnityEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System;

namespace Characters.Dialogue
{
    public class LuaFunctionWrapper : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Managers.QuestManager _questManager;

        [Title("Invoked Events")]

        private void OnEnable()
        {
            Lua.RegisterFunction(nameof(CheckDay), this, SymbolExtensions.GetMethodInfo(() => CheckDay((double)0)));
            Lua.RegisterFunction(nameof(CheckForActiveQuest), this, SymbolExtensions.GetMethodInfo(() => CheckForActiveQuest(string.Empty)));
        }

        private void OnDisable()
        {
            //Lua.UnregisterFunction(nameof(CheckDay)); 
        }

        public bool CheckDay(double requiredDay)
        {
            int day = (int)requiredDay;
            if (Managers.DayManager.Instance.CurrentDay == day) { return true; }
            else { return false; }
        }

        public bool CheckForActiveQuest(string questGuid)
        {
            return _questManager.CheckActiveQuests(Guid.Parse(questGuid));
        }
    }
}