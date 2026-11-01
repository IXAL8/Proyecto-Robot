using UnityEngine;

namespace Robot
{
    public abstract class ChipEffect : ScriptableObject
    {
        public abstract void OnPlaced(ChipsClass chip);
        public abstract void OnRemoved(ChipsClass chip);
    }
}
