using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Sam;

    public AudioSource source;

    [Header("Player")]
    public AudioSource moveStart, moveMiddle, moveEnd;
    public AudioClip shoot, crash;

    [Header("Obstacles")]
    public AudioClip[] largeAudio;
    public AudioClip[] medAudio;
    public AudioClip[] smallAudio;
    public AudioClip swordfish;
    public AudioClip eel;
    public AudioClip anglerfish;
    public AudioClip squid;


    [Header("Music/External")]
    public AudioSource menu;
    public AudioSource[] backgrounds;
    private AudioSource background;
    public AudioSource bossIntro;
    public AudioClip transition;

    [Header("Boss")]
    public AudioClip tenticleAttack;
    public AudioClip handAttack;

    void Awake()
    {
        if (Sam)
        {
            Destroy(this.gameObject);
        } else
        {
            Sam = this;
            DontDestroyOnLoad(this.gameObject);

            source = GetComponent<AudioSource>();
        }
    }

    public void playLargeTrash()
    {
        source.PlayOneShot(largeAudio[UnityEngine.Random.Range(0, largeAudio.Length)]);
    }

    public void playMediumTrash()
    {
        source.PlayOneShot(medAudio[UnityEngine.Random.Range(0, medAudio.Length)]);
    }

    public void playSmallTrash()
    {
        source.PlayOneShot(smallAudio[UnityEngine.Random.Range(0, smallAudio.Length)]);
    }

    public void playPlayerDeath()
    {
        StopMovingSounds();
        source.PlayOneShot(crash);
    }

    public void playMoveStart()
    {
        StartCoroutine(Transition(moveStart, moveMiddle, 1.5f, 1f));
    }

    public void playMoveEnd()
    {
        StartCoroutine(Transition(moveMiddle, moveEnd, 0.5f, 0f));
    }

    private IEnumerator Transition(AudioSource a, AudioSource b, float duration, float delay)
    {
        a.Play();
        b.Play();
        a.volume = 1;
        b.volume = 0;

        yield return new WaitForSeconds(delay);
        float timer = 0;
        while(timer < duration)
        {
            a.volume -= (1/duration) * Time.deltaTime;
            b.volume += (1/duration) * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            timer += Time.deltaTime;
        }

        a.Stop();
    }

    public void StopMovingSounds()
    {
        moveStart.Stop();
        moveMiddle.Stop();
        moveEnd.Stop();
    }

    public void playShoot()
    {
        source.PlayOneShot(shoot);
    }

    public void playSwordfish()
    {
        source.PlayOneShot(swordfish);
    }

    public void playEel()
    {
        source.PlayOneShot(eel);
    }

    public void playAngler()
    {
        source.PlayOneShot(anglerfish);
    }

    public void playSquid()
    {
        source.PlayOneShot(squid);
    }

    public void StartMenu()
    {
        menu.Play();
    }

    public void StopMenu()
    {
        menu.Stop();
    }

    public void StartBackground()
    {
        background = backgrounds[GameManager.Gary.level - 1];
        Debug.Log(GameManager.Gary.level - 1);
        background.Play();
    }

    public void StopBackground()
    {
        background.Stop();
    }

    public void PlayBossIntro()
    {
        bossIntro.Play();   
    }

    public void PlayTransition()
    {
        source.PlayOneShot(transition);
    }

    public void StopAllSounds()
    {
        source.Stop();
        StopMovingSounds();
        StopBackground();
    }

    public void PlayTenticleMove()
    {
        Tenticle();
    }

    public IEnumerator Tenticle()
    {
        source.PlayOneShot(tenticleAttack);

        yield return new WaitForSeconds(3.0f);

         source.PlayOneShot(tenticleAttack);

    }

    public void HandAttack()
    {
        source.PlayOneShot(handAttack);
    }
}
