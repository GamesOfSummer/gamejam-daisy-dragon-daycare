using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DragonSFX : MonoBehaviour
{
        
     public AudioMixer theMixer;
        public static DragonSFX instance;
        public AudioSource dragonSource;
        public AudioSource[] dragonAudio;
        private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(dragonAudio[0]);
        dragonAudio[0].Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
