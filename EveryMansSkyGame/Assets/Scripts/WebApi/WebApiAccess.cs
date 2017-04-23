using UnityEngine;
using System.Collections;

public class WebApiAccess : MonoBehaviour
{
    public static readonly string ApiUrl = "http://52.51.225.104:60000/api";

    public static string ToJsonElement(string tag, string value)
    {
        return "\"" + tag + "\":\"" + value + "\"";
    }

}
