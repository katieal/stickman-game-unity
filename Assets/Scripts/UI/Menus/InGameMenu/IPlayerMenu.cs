using UnityEngine;

namespace UserInterface
{
    public interface IPlayerMenu
    {
        public void ShowWindow(bool isEnabled);
        public void Refresh();
    }
}