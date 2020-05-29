using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Constants
{
    public const int TICKS_PER_SEC = 30;
    public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
    public const float DROP_WEAPON_FORCE = 100f;

    public const string HOST_NAME = "Dude that's good";
    public const string MASTERSERVER_URL = "https://firepointmasterserver.herokuapp.com/server";
    //public const string MASTERSERVER_URL = "http://localhost:8080/server";
    public const int PORT = 26950;
    public const int MAX_PLAYERS = 40;
}
