using System;
using System.IO;
using System.Text;
using SFB;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScreenshotSaver : MonoBehaviour, IPointerDownHandler
{
    public static Action onPreFileDownload = delegate { };
    public static Action onFileDownloaded = delegate { };
    public Camera targetCamera;
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    public void OnPointerDown(PointerEventData eventData) {
        onPreFileDownload();
        Texture2D screenshot = CaptureScreenshotTexture();
        byte[] bytes = screenshot.EncodeToJPG(100);
        DownloadFile(gameObject.name, "OnFileDownload", "report.jpg", bytes, bytes.Length);
    }
#else

    public void OnPointerDown(PointerEventData eventData) { }

    private void SaveFile()
    {
        onPreFileDownload();
        Texture2D screenshot = CaptureScreenshotTexture();
        var path = StandaloneFileBrowser.SaveFilePanel("Save Report Image", "", "report", "jpg");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path, screenshot.EncodeToJPG(100));
        }
        OnFileDownload();
    }
    
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    public void OnClick()
    {
        SaveFile();
    }

#endif

    public Texture2D CaptureScreenshotTexture()
    {
        gameObject.SetActive(false);
        int resWidth = 3840;
        int resHeight = 2160;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);

        targetCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        targetCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        gameObject.SetActive(true);
        return screenShot;
    }

    public void OnFileDownload() {
        onFileDownloaded();
    }
}