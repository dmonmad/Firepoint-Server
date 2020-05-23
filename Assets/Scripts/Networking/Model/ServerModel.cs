using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerModel : MonoBehaviour
{
    private string name;
    private string ip;
    private float lastupdate;

    public ServerModel(string name, string ip, float lastupdate)
    {
        this.name = name;
        this.ip = ip;
        this.lastupdate = lastupdate;
    }

    public string Name { get => name; set => name = value; }
    public string Ip { get => ip; set => ip = value; }
    public float Lastupdate { get => lastupdate; set => lastupdate = value; }
}
