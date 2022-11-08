using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //called when we press the spawn button
    public void SpawnButton()
    {
        TMP_Dropdown dropdown = GameObject.Find("DropdownColour").GetComponent<TMP_Dropdown>();
        string colour = dropdown.options[dropdown.value].text; //return the selected value by the user

        //generate a random number for the box size
        float boxRandomSize = Random.Range(0.5f, 0.8f);

        //we need to tell photon to instantiate the square prefab (playerPrefab) to all players including us.
        //we need to pass data such as the boxRandomSize and the colour selected by the player. This data
        //will be sent over the network and every connected client will make use of this data to create the square prefab
        //according to the selected settings (selected colour & randomSize)

        object[] myCustomInitData = new object[2] { colour, boxRandomSize };
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f),
            Quaternion.identity, 0, myCustomInitData);
    }


}
