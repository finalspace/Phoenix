using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : SingletonBehaviour<MusicManager>
{
    [Header("Background Music")]
    public AudioSource audioSource_BG;
    public AudioClip audioClip_Game;
    public AudioClip audioClip_Title;

    [Header("Sound Effect")]
    public AudioSource audioSource_SE;
    public AudioClip audioClip_Jump;
    public AudioClip audioClip_Eat;
    public AudioClip audioClip_Damage;
    public AudioClip audioClip_Campfire;
    public AudioClip audioClip_Land;
    public AudioClip audioClip_Thunder;
    // Could use separate audiosource OR flag checks for thunder.
    // Flag checks may bring a speed boost, but they're more complicated.
    // Other sounds were stopping the thunder before it's heard, etc.
    public AudioSource audioSource_thunder;

    private bool fadingInBG = false; // TODO music ducking
    private float fadingInProgress;
    private bool damped = false; // whether music has been ducked to low volume
    private long bgTime;
    public Text timeText;
    private bool started = false;
    private bool thundering = false; // so we don't start new sound effects until thunder is done

    private void OnEnable()
    {
        //EventManager.OnPlayerLand += PlayLanding;
        EventManager.OnPlayerJump += PlayJump;
    }

    private void OnDisable()
    {
        //EventManager.OnPlayerLand -= PlayLanding;
        EventManager.OnPlayerJump -= PlayJump;
    }

    private void Start()
    {
        audioSource_thunder.clip = audioClip_Thunder;
        audioSource_thunder.volume = 0.6f;

        // cropping sounds like this works but was slow, have to check later
        //audioClip_Jump = MakeSubclip(audioClip_Jump, 0.8f, 0.942f);
    }

    private void Update()
    {
        //if (thundering)
        //{
        //    if (!audioSource_SE.isPlaying) thundering = false;
            // should not assume playcampfire is still valid, actually, handling in WaypointCollision
            //PlayCampfire();
        //}
        //Debug.Log(audioSource_BG.time);
        if (started && !audioSource_BG.isPlaying)
            PlayBGMusic();
    }

    public void PlayBGMusic()
    {
        // TODO music ducking
        //fadingInProgress = 0;
        //DOTween.To(() => fadingInProgress, x => fadingInProgress = x, 1, 5).SetDelay(1).SetEase(Ease.Linear).OnComplete(OnFadeInBGMusicFinish);
        //fadingInBG = true;

        //audioSource_BG.clip = audioClip_BG;
        //audioSource_BG.loop = true;

        audioSource_BG.Stop(); // just in case

        if (MainGameManager.Instance.gameState == GameState.Title)
        {
            TitleMusic();
        } else
        {
            GameMusic();
        }
        if (!damped) UndampenMusic(); // keep damped if on a campfire, else normal volume
        audioSource_BG.Play();
        started = true;
        bgTime = DateTimeUtil.GetUnixTime();
    }

    public void TitleMusic()
    {
        audioSource_BG.clip = audioClip_Title;
    }

    public void GameMusic()
    {
        audioSource_BG.clip = audioClip_Game;
    }

    public void PlayJump(Vector3 vel)
    {
        if (thundering) return; // not used currently but thunder is false by default
        audioSource_SE.clip = audioClip_Jump;
        //audioSource_SE.pitch = 0.2f; // low pitch gives flame-like sound
        //audioSource_SE.pitch = 0.1f;
        //audioSource_SE.volume = 0.3f;
        audioSource_SE.volume = 0.06f; // the wav is very loud, needs low vol
        audioSource_SE.Play();
    }

    public void PlayLanding()
    {

    }

    public void PlayEat()
    {
        if (thundering) return;
        audioSource_SE.clip = audioClip_Eat;
        audioSource_SE.volume = 0.9f;
        audioSource_SE.Play();
    }

    public void PlayDamage()
    {
        if (thundering) return;
        audioSource_SE.clip = audioClip_Damage;
        audioSource_SE.volume = 1f;
        audioSource_SE.Play();
    }

    public void PlayCampfire()
    {
        if (thundering) return;
        audioSource_SE.clip = audioClip_Campfire;
        DampenMusic();
        audioSource_SE.volume = 1f;
        audioSource_SE.Play();
    }

    public void PlayThunder()
    {
        //DampenMusic();

        //audioSource_SE.clip = audioClip_Thunder;
        //audioSource_SE.volume = 0.6f;
        //audioSource_SE.Play();
        //thundering = true;

        audioSource_thunder.volume = 0.6f;
        //audioSource_thunder.volume = 1f;
        audioSource_thunder.Play();

    }

    public void DampenMusic() // TODO: gradual volume ducking/tweening
    {
        audioSource_BG.volume = 0.1f;
        damped = true;
    }

    public void UndampenMusic()
    {
        audioSource_BG.volume = 0.6f;
        damped = false;
    }

    public void OnFadeInBGMusicFinish()
    {
        fadingInBG = false;
    }

    public int GetBGTime()
    {
        //return DateTimeUtil.MillisecondsElapse(bgTime);
        return (int)(audioSource_BG.time * 1000);
    }

    /**
     * Creates a sub clip from an audio clip based off of the start time
     * and the stop time. The new clip will have the same frequency as
     * the original.
     */
    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        /* Return the sub clip */
        return newClip;
    }
}
