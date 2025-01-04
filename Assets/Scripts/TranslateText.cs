using UnityEngine;
using TMPro;

public class TranslateText : MonoBehaviour
{
    public TMP_Text txt;

    public string engTxt = string.Empty;

    public void Translate(bool isRus)
    {
        if (!isRus) txt.text = engTxt;
    }
}
