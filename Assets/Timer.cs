using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using TMPro;

public class Timer : MonoBehaviour
{
    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    [SerializeField] double timer = 200;
    [SerializeField] TextMeshProUGUI timerTxt;
    ExitGames.Client.Photon.Hashtable CustomeValue;

    double remainingTIme;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }

    void Update()
    {
        if (!startTimer) return;
        timerIncrementValue = PhotonNetwork.Time - startTime;

        remainingTIme = timer - timerIncrementValue;
        timerTxt.text = remainingTIme.ToString();
        if (timerIncrementValue >= timer)
        {
            //Timer Completed

            ///TODO:Improve this hotfix
            FindAnyObjectByType<DeathmatchController>().OnTimerCompleted();
        }
    }
}
