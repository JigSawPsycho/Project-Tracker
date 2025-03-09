using System.IO;
using SFB;
using UnityEngine;

public class PMConfigSaver
{
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    private void SaveFile()
    {
        DownloadFile(gameObject.name, "OnFileDownload", "sample.txt", bytes, bytes.Length);
    }
#else
    private void SaveFile()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "sample", "txt");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllText(path, JsonUtility.ToJson(pmConfig));
        }
    }
#endif
    private ProjectManagerConfig pmConfig;

    public PMConfigSaver(ProjectManagerConfig pmConfig)
    {
        this.pmConfig = pmConfig;
    }

    public void Save()
    {
        SaveFile();
    }
}