using UnityEngine;

namespace Robot
{
    public interface IUISource
    {
        GameObject ChipUI { get; }

        void OpenPause();
        void ClosePause();
        void OpenSetting();
        void CloseSetting();
        void OpenTutorialMenu();
        void OpenEndgame();
    }
}
