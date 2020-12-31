using System;
using System.IO;
using System.Net;
using UnityEngine;

public abstract class RemoteConfig : ScriptableObject
{
    protected abstract string DownloadURL { get; }
    public void DownloadFromRemote(Action<bool> callback)
    {
        bool status;
        WebClient client = new WebClient();
        Stream data;
        try
        {
            data = client.OpenRead(DownloadURL);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            callback?.Invoke(false);
            return;
        }
		
        if (data == null)
        {
            callback?.Invoke(false);
            return;
        }
		
        StreamReader reader = new StreamReader(data);

        status = ParseString(reader);

        data.Close();
        reader.Close();

        callback?.Invoke(status);
    }

    protected abstract bool ParseString(StreamReader reader);
}