using UnityEngine;
using System.Collections;

public class WebApiAccess : MonoBehaviour
{
    public static readonly string ApiUrl = "http://localhost:19642/api";

    public static string ToJsonElement(string tag, string value)
    {
        return "\"" + tag + "\":\"" + value + "\"";
    }

}
