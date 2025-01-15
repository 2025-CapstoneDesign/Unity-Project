using System.Collections;
using UnityEngine;

public class AEDManager : MonoBehaviour
{
    private enum CPRState
    {
        CheckSafety,
        WearPPE,
        CheckConsciousness,
        Call119AndRequestAED,
        CheckBreathingAndPulse,
        ChestCompressions,
        OpenAirway,
        ProvideRescueBreaths,
        ContinueCPR,
        DirectAssistants,
        TurnOnAED,
        AttachPads,
        ClearArea,
        DeliverShock,
        ResumeChestCompressions,
        RecordDocuments,
        Completed
    }

    [SerializeField] private SensorManager sensorManager;
    private CPRState currentState;
    private bool externalInput;  // 외부 입력값 (True/False)
    private WhisperManager whisperManager;
    private LLMManager llmManager;

    void Start()
    {
        currentState = CPRState.CheckSafety;
        externalInput = false;  // 외부 입력은 기본적으로 false로 설정

        // WhisperManager로부터 이벤트 구독
        whisperManager = FindObjectOfType<WhisperManager>();
        if (whisperManager != null)
        {
            whisperManager.RandomInputGenerated += HandleRandomInput;  // 랜덤 입력값 받기
        }
        else
        {
            Debug.LogError("WhisperManager not found!");
        }
        llmManager = new LLMManager();
        StartLLMServer();
        StartCoroutine(CPRProcedure());
    }

    private async void StartLLMServer()
    {
        Debug.Log("Starting LLM server on port");
        await llmManager.ConnectToServer();
        Debug.Log("LLM server started successfully.");
    }

    // 외부에서 입력받는 메서드
    private void HandleRandomInput(bool input)
    {
        externalInput = input;  // WhisperManager로부터 받은 랜덤값을 저장
    }

    private IEnumerator CPRProcedure()
    {
        while (currentState != CPRState.Completed)
        {
            switch (currentState)
            {
                case CPRState.CheckSafety:
                    Debug.Log("1. 현장 안전을 확인한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    llmManager.setType("현장안전확인");
                    while (!llmManager.getIsValid())
                    {
                        yield return null;
                    }
                    break;

                case CPRState.WearPPE:
                    Debug.Log("2. 감염 방지를 위한 개인보호장구를 착용한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());

                    break;

                case CPRState.CheckConsciousness:
                    Debug.Log("3. 의식을 확인한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.Call119AndRequestAED:
                    Debug.Log("4. 119 신고 및 AED를 요청한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.CheckBreathingAndPulse:
                    Debug.Log("5. 호흡과 맥박을 동시에 확인한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.ChestCompressions:
                    Debug.Log("6. 가슴압박을 30회 실시한다.");
                    CPRManager cprManager = new CPRManager(sensorManager);
                    while (!cprManager.IsCPRPassed)
                    {
                        Debug.Log("wait 프레임");
                        yield return null; // 다음 프레임 대기
                    }
                    Debug.Log("CPR 통과!!!!!!!!!!!!");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.OpenAirway:
                    Debug.Log("7. 기도를 개방한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.ProvideRescueBreaths:
                    Debug.Log("8. 포켓마스크를 사용하여 인공호흡을 2회 실시한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.ContinueCPR:
                    Debug.Log("9. 가슴압박과 인공호흡을 30 : 2로 5주기 실시한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.DirectAssistants:
                    Debug.Log("10. 보조요원에게 CPR을 지시한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.TurnOnAED:
                    Debug.Log("11. AED의 전원을 켠다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.AttachPads:
                    Debug.Log("12. 제세동 패드를 부착한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.ClearArea:
                    Debug.Log("13. 분석 전과 제세동 전에 주위 사람들을 물러나도록 한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.DeliverShock:
                    Debug.Log("14. 쇼크 버튼을 누른다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.ResumeChestCompressions:
                    Debug.Log("15. 즉시 가슴압박을 시작한다.");
                    whisperManager.GenerateRandomInput();
                    yield return StartCoroutine(WaitForInput());
                    break;

                case CPRState.RecordDocuments:
                    Debug.Log("16. 의무기록지에 기록한다.");
                    whisperManager.GenerateRandomInput();
                    currentState = CPRState.Completed;
                    break;
            }
        }
    }

    // 외부 입력값을 기다리는 코루틴
    private IEnumerator WaitForInput()
    {
        // 외부 입력이 true가 될 때까지 대기
        while (!externalInput)
        {
            yield return null;  // 프레임마다 계속 대기
        }

        // 외부 입력이 true일 경우, 다음 단계로 넘어감
        externalInput = false;  // 입력을 받은 후, 상태를 초기화하여 재시도하지 않도록 함
        currentState++;  // 다음 단계로 넘어감
    }
}
