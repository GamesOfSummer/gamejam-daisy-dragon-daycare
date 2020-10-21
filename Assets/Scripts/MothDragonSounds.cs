using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MothDragonSounds : MonoBehaviour
{

     public AudioMixer theMixer;
    public static MothDragonSounds instance;
    public AudioSource audioSource;

    //public AudioSource levelMusic, gameOverMusic, winMusic;


    public AudioClip[] clips;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(int sfxToPlay)
    {
        audioSource.PlayOneShot(clips[sfxToPlay], .7f);
    }
}

