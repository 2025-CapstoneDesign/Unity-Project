using UnityEngine;

public abstract class CameraFrameProvider : MonoBehaviour
{
    // 카메라 시작
    public abstract void StartCamera();

    // 카메라 종료
    public abstract void StopCamera();

    // 현재 프레임 가져오기
    public abstract Texture2D GetCurrentFrame();
}
