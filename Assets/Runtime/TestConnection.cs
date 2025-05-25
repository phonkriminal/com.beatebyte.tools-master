using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System;

public class TestConnection : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private string _textURL;

    private Coroutine _coroutine;

    [Serializable]
    internal class ChangeLogInfo
    {
        public int Version;
        public string LastUpdate;

        public static ChangeLogInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<ChangeLogInfo>(jsonString);
        }

        public ChangeLogInfo(int version, string lastUpdate)
        {
            Version = version;
            LastUpdate = lastUpdate;
        }
    }
    public void StartConnection()
    {
        if (_coroutine != null) { StopCoroutine(_coroutine); _coroutine = null; }

        _coroutine = StartCoroutine(nameof(GetText));

    }

    private IEnumerator GetText()
    {
        _text.text = "Waiting for Response";
        using (UnityWebRequest request = UnityWebRequest.Get(_textURL))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
                _text.text = request.error.ToString();
            }
            else
            {
                Debug.Log("Successful Downloaded Text");
                var text = request.downloadHandler.text;
                ChangeLogInfo changeLogInfo = ChangeLogInfo.CreateFromJSON(text);
                _text.text = $"Version : {changeLogInfo.Version.ToString()} -- LastUpdate : {changeLogInfo.LastUpdate.ToString()}";
            }
        }
    }
}
