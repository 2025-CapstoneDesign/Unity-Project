using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[4096];

    private int receivedCount = 0; // 수신된 데이터 개수 카운터
    private const int debugThreshold = 50; // 디버그 출력 기준 개수

    public void ConnectToServer(string ipAddress, int port)
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log($"Connected to server at {ipAddress}:{port}");

            // 서버로부터 데이터를 수신하는 비동기 작업 시작
            BeginReceive();
        }
        catch (SocketException e)
        {
            Debug.LogError($"Socket connection failed: {e.Message}");
        }
    }

    public void SendFrame(Texture2D frame)
    {
        if (client == null || stream == null || !client.Connected)
        {
            Debug.LogWarning("Socket not connected");
            return;
        }

        byte[] imageBytes = frame.EncodeToJPG();

        try
        {
            stream.Write(imageBytes, 0, imageBytes.Length);
            // Debug.Log($"Frame sent to server: {imageBytes.Length} bytes");
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to send frame: {e.Message}");
        }
    }

    private void BeginReceive()
    {
        if (stream == null || !stream.CanRead)
        {
            Debug.LogWarning("Stream is not available for reading");
            return;
        }

        try
        {
            // 비동기로 데이터 읽기 시작
            stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting receive: {e.Message}");
        }
    }

    private void OnDataReceived(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead > 0)
            {
                // 수신된 데이터를 문자열로 변환
                string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // 데이터 수신 카운터 증가
                receivedCount++;

                // 30번째 데이터마다 디버그 출력
                if (receivedCount % debugThreshold == 0)
                {
                    Debug.Log($"Received JSON Data (#{receivedCount}): {jsonData}");
                }

                // JSON 데이터 처리 추가 가능
                ParseAndDebugJson(jsonData);

                // 다음 데이터 읽기 시작
                BeginReceive();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving data: {e.Message}");
        }
    }

    private void ParseAndDebugJson(string jsonData)
    {
        try
        {
            // JSON 데이터를 Unity의 Debug.Log로 출력 (필요하면 삭제 가능)
            Debug.Log($"Parsed JSON Data: {jsonData}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing JSON: {e.Message}");
        }
    }

    public void Disconnect()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        Debug.Log("Disconnected from server");
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
