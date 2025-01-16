using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LLMManager
{
    private string HOST = "127.0.0.1";
    private int PORT = 12345;
    private TcpClient client;
    private NetworkStream stream;
    private bool isReceiving = false; // 메시지 수신 상태를 관리하는 플래그
    private string type = "현장안전확인"; // 검증할 타입
    private bool isValid = false; // 검증 결과

    /// <summary>
    /// 서버에 연결하여 메시지 수신을 시작합니다.
    /// </summary>
    public async Task ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            Debug.Log("Connecting to server...");
            await client.ConnectAsync(HOST, PORT);
            stream = client.GetStream();
            Debug.Log($"Connected to server at {HOST}:{PORT}");
            
            // 메시지 수신 시작
            isReceiving = true;
            StartReceivingMessages();
        }
        catch (Exception e)
        {
            Debug.LogError($"Connection error: {e.Message}");
        }
    }

    /// <summary>
    /// 메시지 수신 루프를 시작합니다.
    /// </summary>
    private async void StartReceivingMessages()
    {
        try
        {
            byte[] buffer = new byte[4096];
            while (isReceiving)
            {
                if (stream != null && stream.CanRead)
                {
                    // 메시지 수신
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) // 서버가 연결을 종료한 경우
                    {
                        Debug.LogWarning("Server closed the connection.");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log($"Received from server: {response}");

                    // 메시지 처리 및 검증 로직 호출
                    OnJsonReceived(response);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving message: {e.Message}");
        }
        finally
        {
            CloseConnection();
        }
    }

    /// <summary>
    /// JSON 데이터를 수신했을 때 호출되는 이벤트
    /// </summary>
    private void OnJsonReceived(string json)
    {
        Debug.Log($"Processing JSON: {json}");

        isValid = ValidateLLMResponse(json, type);

        Debug.Log($"Validation result for type '{type}': {isValid}");
    }

    /// <summary>
    /// JSON 데이터 검증
    /// </summary>
    public bool ValidateLLMResponse(string json, string type)
    {
        try
        {
            // JSON 검증 로직
            List<string> targets = ExtractTargets(json); // JSON에서 'targets' 추출
            Debug.Log($"Decoded targets: {string.Join(", ", targets)}");
            
            if (LLMTargetValidator.ValidateLLMWords(type, targets))
            {
                Debug.Log($"Validation passed for type: {type}");
                return true;
            }
            else
            {
                Debug.LogWarning($"Validation failed for type: {type}");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Validation error: " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 연결 종료
    /// </summary>
    private void CloseConnection()
    {
        try
        {
            isReceiving = false;
            stream?.Close();
            client?.Close();
            Debug.Log("Connection closed.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error closing connection: {e.Message}");
        }
    }

    private List<string> ExtractTargets(string json)
    {
        try
        {
            // JSON 파싱
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            if (jsonObject.ContainsKey("targets"))
            {
                var targetsRaw = jsonObject["targets"] as Newtonsoft.Json.Linq.JArray;
                var targets = new List<string>();

                foreach (var target in targetsRaw)
                {
                    // 유니코드 디코딩
                    var decodedTarget = Encoding.UTF8.GetString(Encoding.Default.GetBytes(target.ToString()));
                    targets.Add(decodedTarget);
                }

                return targets;
            }
            else
            {
                throw new Exception("'targets' key not found in JSON");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON parsing error: {ex.Message}");
            throw; // 예외를 다시 던져 호출자가 처리하도록 함
        }
    }

    public void setType(string type){
        this.type = type;
        isValid = false;
    }

    public bool getIsValid(){
        return isValid;
    }
}
