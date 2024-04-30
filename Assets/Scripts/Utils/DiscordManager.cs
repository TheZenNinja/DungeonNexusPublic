using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=00gNK4JUVFg
public class DiscordManager : MonoBehaviour
{
    // move out of assembly def and use another script doing send/recive messages 
    // TODO - check if discord is running before doing anything. - 
    Discord.Discord discord;

    public string currentStatus = "Doing Dev Work";
    public string moreDetails = "Making the Warlock";

    void Start()
    {
        discord = new Discord.Discord(1196144504550461520, (ulong)Discord.CreateFlags.NoRequireDiscord);
        if (Application.isEditor)
            ChangeEditorStatus();
        else
            UpdateDiscordStatus("Warlock", 4);
    }
    private void OnDisable() => discord.Dispose();
    private void Update() => discord?.RunCallbacks();

    public void ChangeEditorStatus()
    {
        var activityMan = discord.GetActivityManager();
        var act = new Discord.Activity
        {
            Details = currentStatus,
            State = moreDetails,
            Assets = { LargeImage = "https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" }
        };

        activityMan.UpdateActivity(act, (res) => { Debug.Log($"Updated Status: {res}"); });
    }

    public void UpdateDiscordStatus(string classID, int level)
    {
        var activityMan = discord.GetActivityManager();
        var act = new Discord.Activity
        {
            Details = $"Playing: {classID.Substring(0, 1).ToUpper() + classID.Substring(1).ToLower()}",
            State = $"Level {level}",
            Assets = { LargeImage = "https://cdn.sanity.io/images/fuvbjjlp/production/bd6440647fa19b1863cd025fa45f8dad98d33181-2000x2000.png" }
        };

        activityMan.UpdateActivity(act, (res) => { Debug.Log($"Updated Status: {res}"); });
    }
}
