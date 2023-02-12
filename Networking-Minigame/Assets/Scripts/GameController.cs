using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class GameController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject startCanvas;
    public GameObject sceneCamera;
    public TextMeshProUGUI pingText;
    public GameObject disconnectUI;
    private bool off = false;
    public GameObject playerFeed;
    public GameObject feedGrid;

    //Singleton pattern
    #region Singleton
    private static GameController _instance;
    public static GameController Instance
    {
        get //making sure that a weapon manager always exists
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
            }
            if (_instance == null)
            {
                GameObject go = new GameObject("GameController");
                _instance = go.AddComponent<GameController>();
            }
            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        if (_instance == null) //if there's no instance of the weapon manager, make this the weapons manager, ortherwise delete this to avoid duplicates
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ActivateStartMenu();
    }

    private void Update()
    {
        CheckInput();
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
    }

    private void CheckInput()
    {
        if(off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(false);
            off = false;
        }
        else if (!off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(true);
            off = true;
        }
    }

    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-5f, 5f);

        PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y * randomValue), Quaternion.identity);
        startCanvas.SetActive(false);
        sceneCamera.SetActive(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void ActivateStartMenu()
    {
        startCanvas.SetActive(true);
        sceneCamera.SetActive(true);
    }

    //public override void OnPlayerEnteredRoom(Player player)
    //{

    //}
}
