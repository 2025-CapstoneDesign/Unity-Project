using UnityEngine;

public class WebCamManager : CameraFrameProvider
{
    private WebCamTexture webCamTexture;

    private void Start()
    {
        StartCamera();
    }

    public override void StartCamera()
    {
        if (webCamTexture == null)
        {
            webCamTexture = new WebCamTexture();
        }

        if (!webCamTexture.isPlaying)
        {
            webCamTexture.Play();
        }
    }

    public override void StopCamera()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }

    public override Texture2D GetCurrentFrame()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            Texture2D currentFrame = new Texture2D(webCamTexture.width, webCamTexture.height);
            currentFrame.SetPixels(webCamTexture.GetPixels());
            currentFrame.Apply();
            Debug.Log($"Captured Frame: {currentFrame.width}x{currentFrame.height}");
            return currentFrame;
        }
        return null;
    }

    private void OnDestroy()
    {
        StopCamera();
    }
}
