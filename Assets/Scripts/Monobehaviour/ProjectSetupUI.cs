using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectSetupUI : MonoBehaviour
{
    public TMP_InputField projectNameInputField;
    public TMP_InputField notesInputField;
    public TMP_Dropdown startWeekDropdown;
    public TMP_Dropdown endWeekDropdown;
    public TMP_Dropdown statusDropdown;
    public TMP_InputField progressInputField;
    public Button removeButton;
    public Button increasePrioButton;
    public Button decreasePrioButton;
    public TextMeshProUGUI projectPrioText;
}
