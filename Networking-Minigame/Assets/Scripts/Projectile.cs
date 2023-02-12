using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;

public class Projectile : MonoBehaviour, IPunObservable
{
    [SerializeField] private bool isCollectable = false;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float baseDuration;
    [SerializeField] private float duration;

    private Vector2 targetPosition;
    [SerializeField] private Collider2D ballCollider;
    private bool beenThrown;
    Vector3 movement;

    public PhotonView view;

    // Update is called once per frame
    void Update()
    {
        if (beenThrown)
        {
            if (duration > 0)
            {
                MoveBall();
            }
            else
            {
                moveSpeed = 0;
                isCollectable = true;
                beenThrown = false;
            }
        }
    }

    public void Throw(Vector3 target)
    {
        targetPosition = target;
        //view.RPC("Throw_RPC", RpcTarget.All);
        //transform.SetParent(null);
        duration = baseDuration;
        beenThrown = true;

        //point projectile to the direction it's facing
        Vector2 direction = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        transform.up = direction;
        //moveSpeed += FindObjectOfType<Player>().gameObject.GetComponent<Rigidbody2D>().velocity;
        isCollectable = false;
        moveSpeed = baseMoveSpeed;
        ballCollider.enabled = true;
    }
    [PunRPC]
    public void Throw_RPC()
    {
        transform.SetParent(null);
        duration = baseDuration;
        beenThrown = true;       

        //point projectile to the direction it's facing
        Vector2 direction = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
        transform.up = direction;
        //moveSpeed += FindObjectOfType<Player>().gameObject.GetComponent<Rigidbody2D>().velocity;
        isCollectable = false;
        moveSpeed = baseMoveSpeed;
        ballCollider.enabled = true;

        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isCollectable)
        {
            if (collision.gameObject.CompareTag("Player"))
            {              
                if (collision.gameObject.GetComponentInParent<Player>().hasBall == false)
                {
                    collision.gameObject.GetComponentInParent<Player>().SetBall();
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponentInParent<Player>().Death();
                moveSpeed = 0;
                isCollectable = true;
            }
            
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                moveSpeed = 0;
                isCollectable = true;
            }            
        }
    }

    [PunRPC]
    private void MoveBall()
    {
        movement = transform.up * moveSpeed * Time.deltaTime;
        transform.position += movement;
        duration -= Time.deltaTime;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(movement);
            stream.SendNext(beenThrown);
        }
        else
        {
            movement = (Vector3)stream.ReceiveNext();
            beenThrown = (bool)stream.ReceiveNext();
        }
    }
}
