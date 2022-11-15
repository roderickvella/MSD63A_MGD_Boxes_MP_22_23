using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSizes(string jsonSizes)
    {
        //send message to all connected players (even the master client) with new random sizes
        photonView.RPC("ChangeSizesRPC", RpcTarget.All, jsonSizes);
    }

    [PunRPC]
    public void ChangeSizesRPC(string jsonSizes)
    {
        List<PlayerInfo> playersInfo = JsonConvert.DeserializeObject<List<PlayerInfo>>(jsonSizes);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerController>().ChangeSizeFromMaster(playersInfo);
        }
    }

    public void DestroyPlayer(int destroyPlayerId)
    {
        //send message to all connected players with playerId to destroy
        photonView.RPC("DestroyPlayerRPC", RpcTarget.All, destroyPlayerId);
    }

    [PunRPC]
    public void DestroyPlayerRPC(int destroyPlayerId)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            if(player.GetComponent<PhotonView>().Owner.ActorNumber == destroyPlayerId)
            {
                if (player.GetComponent<PhotonView>().AmOwner)//only the owner of an object can destroy it
                    PhotonNetwork.Destroy(player);
            }
        }
    }



}
