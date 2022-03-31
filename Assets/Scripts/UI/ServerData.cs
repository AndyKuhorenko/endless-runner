using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;


[RequireComponent(typeof(TextMeshProUGUI))]
public class ServerData : MonoBehaviour
{
    TextMeshProUGUI uiText;
    private const string url = "https://jsonplaceholder.typicode.com/todos/";

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(GetData());
    }

    IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else if (request.isDone)
        {
            ProcessJson(request.downloadHandler.text);
            // ShowText();
        }
    }

    private void ProcessJson(string jsonString)
    {
        JsonData json = JsonMapper.ToObject(jsonString);

        for (int i = 0; i < 10; i++)
        {
            //uiText.text = "";
            uiText.text += $"{json[i]["id"]}: {json[i]["title"]} <br>";
        }

    }

    //private void ShowText()
    //{
    //    foreach(KeyValuePair<string, string> kvp in data)
    //    {
    //        uiText.text += $"title {kvp[]}";
    //    }
    //}
}
