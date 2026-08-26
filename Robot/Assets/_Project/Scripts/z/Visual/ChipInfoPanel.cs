using TMPro;
using UnityEngine;

namespace Robot
{

    public class ChipInfoPanel : MonoBehaviour
    {
        [Header("Chip information")]
        [SerializeField] private TextMeshProUGUI _nameChipTXT;
        [SerializeField] private TextMeshProUGUI _descriptionChipTXT;
        
        public void RefreshChipInformation(string name = "ChipName", string description = "ChipDescription")
        {
            if(ChipManager.Source.ChipsAvailables.Count == 0)
            {
                _nameChipTXT.text = "No chips available";
                _descriptionChipTXT.text = "";
            }
            else
            {
                _nameChipTXT.text = name;
                _descriptionChipTXT.text = description;
            }

        }
    }
}
