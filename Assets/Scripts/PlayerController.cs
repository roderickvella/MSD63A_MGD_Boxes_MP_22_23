using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
{


    private Vector3 playerScale;
    private FixedJoystick fixedJoystick;
    private PhotonView photonView;
    private Rigidbody2D body;
    private Vector3 playerPos;
    private float horizontal;
    private float vertical;
    private float runSpeed = 5f;

    //is automatically called when the playerPrefab is instaniated on every client
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = info.photonView.Owner.NickName;

        object[] instantiationData = info.photonView.InstantiationData;
        string colour = (string)instantiationData[0];
        print("selected colour:" + colour);
        float boxRandomSize = (float)instantiationData[1];
        print("size:" + boxRandomSize);

        if (colour == "Red")
            GetComponent<SpriteRenderer>().color = Color.red;
        else if (colour == "Green")
            GetComponent<SpriteRenderer>().color = Color.green;
        else if (colour == "Blue")
            GetComponent<SpriteRenderer>().color = Color.blue;

        playerScale = new Vector3(boxRandomSize, boxRandomSize, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //we own this instance (this player prefab), therefore send this data to the other connected clients
            stream.SendNext(transform.position);
            stream.SendNext(transform.localScale);
            print("Sending Player Position:" + transform.position);
        }
        else if (stream.IsReading)
        {
            //we receiving data about this gameobject (prefab) from the owner of this gameobject
            playerPos = (Vector3)stream.ReceiveNext();
            playerScale = (Vector3)stream.ReceiveNext();
            print("Received Player Position:" + playerPos);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        if (photonView.IsMine)
        {
            //this player prefab owns this photon, therefore give control to the joystick
            body = GetComponent<Rigidbody2D>();
            fixedJoystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
        }
        else
        {
            //if player object is not mine, then it should automatically be controlled by photon data
            //this means that we can destroy the RigidBody
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = playerScale;
        //transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);

        if (photonView.IsMine)
        {
            //we own this instance, therefore move object with the data from the joystick
            horizontal = fixedJoystick.Horizontal;
            vertical = fixedJoystick.Vertical;
        }
        else
        {
            //if player object is not mine, then we need to manually change its position with the data from the server
            //basically the data we received from OnPhotonSerializeView()
            transform.position = Vector3.Lerp(transform.position, playerPos, Time.deltaTime * 10);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        
    }
}
