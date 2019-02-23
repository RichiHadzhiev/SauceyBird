using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 1;
    public Vector3 startPos;
    public Rigidbody2D rigidBody;
    Quaternion downRotation;
    Quaternion forwardRotation;
    GameManager game;

    public AudioSource jumpAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        rigidBody.simulated = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.GameOver) return;

        if(Input.GetMouseButtonDown(0) && rigidBody.transform.position.y <= 4.41)
        {
            jumpAudio.Play();
            transform.rotation = forwardRotation;
            rigidBody.velocity = Vector3.zero;
            rigidBody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("ScoreZone"))
        {
            //register a score event
            OnPlayerScored(); //event sent to GameManager
            //sound
            scoreAudio.Play();
        }

        if (collision.gameObject.CompareTag("DeathZone"))
        {
            dieAudio.Play();
            rigidBody.simulated = false;
            //register a dead event
            OnPlayerDied(); // event sent to GameManager
            //sound
        }
    }
}
