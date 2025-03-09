using System;
using System.IO;
using System.Text;
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
        byte[] bytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(pmConfig));
        DownloadFile(gameObject.name, "OnFileDownload", "pmconfig.pmcfg", bytes, bytes.Length);
    }
#else
    private void SaveFile()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "pmconfig", "pmcfg");
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