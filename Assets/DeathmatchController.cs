using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Opsive.Shared.Events;
using TMPro;


public class DeathmatchController : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI playerKillCount;
    [SerializeField] TextMeshProUGUI remainingTime;

    private Dictionary<int, int> playerKillsData = new Dictionary<int, int>();

    int localPlayerScore;

    public GameObject localPlayer;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("EnteredRoom room registered");
            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }
        
        //EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(localPlayer, "OnDeath", OnDeath);
    }

    public void OnDisable()
    {
        //EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        //EventHandler.UnregisterEvent<Vector3, Vector3, GameObject>(gameObject, "OnDeath", OnDeath);
    }

    public void OnTimerCompleted()
    {
        photonView.RPC("RpcOnTimerComplete", RpcTarget.All);
    }

    public void OnGameOver()
    {
        EventHandler.ExecuteEvent(localPlayer, "OnEnableGameplayInput", false);
    }

    public void RegisterLocalPlayer(GameObject a_loacalplayer)
    {
        Debug.Log("OnDeath registered");
        localPlayer = a_loacalplayer;
        EventHandler.RegisterEvent<Vector3, Vector3, GameObject>(localPlayer, "OnDeath", OnDeath);
    }


    public void OnPlayerEnteredRoom(Player player, GameObject character)
    {
        ///TODO: Fix the below hotfix
        if (!playerKillsData.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
        {
            playerKillsData.Add(PhotonNetwork.LocalPlayer.ActorNumber, 0);
        }
        if (!playerKillsData.ContainsKey(player.ActorNumber))
        {
            playerKillsData.Add(player.ActorNumber, 0);
        }
        photonView.RPC("RpcOnPlayerJoined", RpcTarget.All, playerKillsData);
    }

    [PunRPC]
    //public void RpcOnPlayerJoined(string playerData)
    public void RpcOnPlayerJoined(Dictionary<int, int> dictionaryRef)
    {
        foreach (var receivedPlayerDictionary in dictionaryRef)
        {
            if (!playerKillsData.ContainsKey(receivedPlayerDictionary.Key))
            {
                playerKillsData.Add(receivedPlayerDictionary.Key, receivedPlayerDictionary.Value);
            }
        }
    }

    public void OnDeath(Vector3 position, Vector3 force, GameObject attacker)
    {
        Debug.Log("Player die: PlayerDie ");
        int attackerActorNumber = attacker.GetComponent<PhotonView>().OwnerActorNr;
        photonView.RPC("RpcOnPlayerDied", RpcTarget.All, attackerActorNumber);

    }

    [PunRPC]
    public void RpcOnPlayerDied(int attackerActorNumber)
    {
        if (playerKillsData.ContainsKey(attackerActorNumber))
        {
            playerKillsData[attackerActorNumber]++;

        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == attackerActorNumber)
        {
            localPlayerScore++;
            playerKillCount.text = localPlayerScore.ToString();
        }
    }

    [PunRPC]
    public void RpcOnTimerComplete()
    {
        OnGameOver();
    }
}