using System;
using System.Collections;
using SFB;
using UnityEngine;

public class ReportLoader
{
    string fileExtension;
    Action<Report> onLoadComplete;
    MonoBehaviour monoBehaviour;

    public ReportLoader(MonoBehaviour monoBehaviour, string fileExtension, Action<Report> onLoadComplete)
    {
        this.fileExtension = fileExtension;
        this.onLoadComplete = onLoadComplete;
        this.monoBehaviour = monoBehaviour;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
    
    private string StartImport()
    {
        UploadFile(gameObject.name, "OnFileUpload", $".{fileExtension}", false);
    }

    public void OnFileUpload(string url) {
        monoBehaviour.StartCoroutine(OutputRoutine(url));
    }
#else
    private void StartImport()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Report Config", "", fileExtension, false);
        if (paths.Length > 0) {
            monoBehaviour.StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        onLoadComplete(JsonUtility.FromJson<Report>(loader.text));
    }

    public void StartLoad()
    {
        StartImport();
    }

}
