using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    [SerializeField] private AudioSource audioSourcePrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform sourceTransform, float volume)
    {
        //source in gameObject
        AudioSource audioSource = Instantiate(audioSourcePrefab, sourceTransform.position, Quaternion.identity);

        //assign the audiClip
        audioSource.clip = audioClip;

        //assignVolume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get lenght of soundFX clip
        float clipLenght = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform sourceTransform, float volume)
    {
        //assign a random index
        int index = Random.Range(0, audioClip.Length);
    
        //source in gameObject
        AudioSource audioSource = Instantiate(audioSourcePrefab, sourceTransform.position, Quaternion.identity);

        //assign the audiClip
        audioSource.clip = audioClip[index];

        //assignVolume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get lenght of soundFX clip
        float clipLenght = audioSource.clip.length;

        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLenght);
    }

}
