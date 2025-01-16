using UnityEngine;

public class CameraSocketHandler : MonoBehaviour
{
    public CameraFrameProvider cameraFrameProvider;
    public SocketManager socketManager;

    private void Start()
    {
        // 카메라 시작
        cameraFrameProvider.StartCamera();

        // 서버 연결
        socketManager.ConnectToServer("127.0.0.1", 5000);
    }

    private void Update()
    {
        // 현재 프레임 가져와서 서버로 전송
        Texture2D currentFrame = cameraFrameProvider.GetCurrentFrame();
        if (currentFrame != null)
        {
            socketManager.SendFrame(currentFrame);
        }
    }

    private void OnDestroy()
    {
        // 리소스 정리
        cameraFrameProvider.StopCamera();
        socketManager.Disconnect();
    }
}
