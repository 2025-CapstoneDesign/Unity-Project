using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshPro 사용
public class TimerManager : MonoBehaviour
{

    public TextMeshProUGUI timerText; // UI에서 남은 시간 표시
    private float remainingTime;
    private bool isRunning;

    public void StartTimer(float duration)
    {
        remainingTime = duration;
        isRunning = true;
        StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();  // 🎯 Update() 없이 UI 업데이트

            yield return null;
        }

        isRunning = false;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"남은 시간: {remainingTime:F1} 초";
        }
    }

    public bool IsTimeUp()
    {
        return remainingTime <= 0;
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
