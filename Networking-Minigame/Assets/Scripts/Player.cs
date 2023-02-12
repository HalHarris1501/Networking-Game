using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using TMPro;

public class Player : MonoBehaviourPun
{
    public PhotonView photonView;
    public float moveSpeed = 5f;
    public Rigidbody2D rigidBody;
    
    public GameObject playerCamera;
    public SpriteRenderer sr;
    public TextMeshProUGUI playerNameText;
    public GameObject playerSprite;
    public bool hasBall = false;
    public GameObject ballPosition;
    public Vector2 targetPosition;
    public GameObject dodgeBall;
    public GameObject holdBall;

    private void Awake()
    {
        if(photonView.IsMine)
        {
            playerCamera.SetActive(true);
            playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            playerNameText.text = photonView.Owner.NickName;
            playerNameText.color = Color.red;
            sr.color = Color.red;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            GetInput();
        }
    }

    private void GetInput()
    {
        Vector3 movement = new Vector3();

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        transform.position += movement.normalized * moveSpeed * Time.deltaTime;
        if(hasBall == true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                targetPosition = playerCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                GameObject ball = PhotonNetwork.Instantiate(dodgeBall.name, ballPosition.transform.position, Quaternion.identity);
                ball.GetComponent<Projectile>().Throw(targetPosition);
                hasBall = false;
                holdBall.SetActive(false);
            }
        }
    }    

    public void Death()
    {
        PhotonNetwork.Destroy(gameObject);
        GameController.Instance.ActivateStartMenu();
    }

    public void SetBall()
    {
        photonView.RPC("SetBall_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void SetBall_RPC()
    {
        hasBall = true;
        holdBall.SetActive(true);
    }
}
