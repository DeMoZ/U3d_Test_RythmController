using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private List<TMP_Text> _texts = new();


    private void Awake()
    {
        ShowLog(false);
    }

    public void ShowLog(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ShowLog(int index, string str)
    {
        if (!gameObject.activeSelf)
            ShowLog(true);

        TryAddTextRecursively(index);
        _texts[index].text = str;
    }

    public void TryAddTextRecursively(int index)
    {
        if (_texts.Count <= index)
        {
            var tmpText = Instantiate(text, transform);
            tmpText.gameObject.SetActive(true);
            _texts.Add(tmpText);

            TryAddTextRecursively(index);
        }
    }
}
