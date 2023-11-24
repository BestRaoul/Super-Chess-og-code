using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnBoard : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] Sprite p1;
    [SerializeField] Sprite p2;
    [SerializeField] Sprite p3;
    [SerializeField] Sprite p4;

    [SerializeField] Sprite victory;
    [SerializeField] Sprite defeat;


    private AudioSource audioSource;
    [SerializeField] AudioClip whoosh;

    [SerializeField] AudioSource turnSource;
    [SerializeField] AudioClip endTurn;
    [SerializeField] AudioClip startTurn;

    [SerializeField] AudioSource globalSource;
    [SerializeField] AudioClip StartGame;
    [SerializeField] AudioClip victorySFX;
    [SerializeField] AudioClip defeatSFX;

    private Animator anim;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
    }

    public void PlayEndTurnSFX()
    {
        turnSource.PlayOneShot(endTurn);
    }
    public void PlayStartTurnSFX()
    {
        turnSource.PlayOneShot(startTurn);
    }

    public void PlayAnim(int teamId)
    {
        switch (teamId)
        {
            case 1:
                sr.sprite = p1;
                break;
            case -1:
            case 2:
                sr.sprite = p2;
                break;
            case 3:
                sr.sprite = p3;
                break;
            case 4:
                sr.sprite = p4;
                break;
        }

        audioSource.PlayOneShot(whoosh);
        anim.SetTrigger("turn");
    }

    public void PlayEndGameAnim(bool victorious)
    {
        if (victorious)
        {
            sr.sprite = victory;
            globalSource.PlayOneShot(victorySFX);
        }
        else
        {
            sr.sprite = defeat;
            globalSource.PlayOneShot(defeatSFX);
        }

        anim.SetTrigger("endGame");
    }

    public void PlayStartGameSFX()
    {
        globalSource.PlayOneShot(StartGame);
    }
}
