using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ServerModel
{
    public int id;
    public string servername;
    public string ip;
    public float lastupdate;

    public ServerModel(string name, float lastupdate)
    {
        this.servername = name;
        this.ip = Constants.PORT.ToString();
        this.lastupdate = lastupdate;
    }

    public string Name { get => servername; set => servername = value; }
    public float Lastupdate { get => lastupdate; set => lastupdate = value; }
}
