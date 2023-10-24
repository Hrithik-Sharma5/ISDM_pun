using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterProperties : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (photonView.IsMine)
        {
            FindAnyObjectByType<DeathmatchController>().RegisterLocalPlayer(this.gameObject);
        }
    }

}
