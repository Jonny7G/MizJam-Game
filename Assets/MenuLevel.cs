using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MenuLevel : MonoBehaviour
{
    public TMP_Text descriptionText;
    public Image selectedHighlight;
    [TextArea]public string description;
    public bool Unlocked;
    public bool Selected;

    public void Select(bool isSelected)
    {
        selectedHighlight.gameObject.SetActive(isSelected);
        Selected = isSelected;
    }
    public void SetUnlocked()
    {
        Unlocked = true;
        descriptionText.SetText(description);
    }
}
