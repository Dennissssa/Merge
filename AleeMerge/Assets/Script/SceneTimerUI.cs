using UnityEngine;
using TMPro;

public class SceneTimerUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;

    [Header("Settings")]
    public int startNumber = 1;

    [Header("Display")]
    public string unit = " s";   // 单位（可以改成 " 秒"）

    private float _time;
    private int _currentValue;

    void Start()
    {
        _currentValue = startNumber;
        _time = 0f;
        UpdateText();
    }

    void Update()
    {
        _time += Time.deltaTime;

        int newValue = startNumber + Mathf.FloorToInt(_time);

        if (newValue != _currentValue)
        {
            _currentValue = newValue;
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (timerText != null)
            timerText.text = _currentValue.ToString() + unit;
    }
}
