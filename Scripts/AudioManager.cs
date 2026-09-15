using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int poolSize=10;
    private List <AudioSource> sfxpool;
    private List<AudioClip> playingClips;
    
    private void Awake()
    {
        sfxpool = new List<AudioSource>();
        playingClips = new List<AudioClip>();
        for (int i=0; i < poolSize; i++)
        {
            sfxpool.Add(gameObject.AddComponent<AudioSource>());
            playingClips.Add(null);
        }
    }
    public void PlaySFX(AudioData data)
    {
        if (data == null || data.soundClip == null)
        {
            return;
        }

        for (int i = 0; i < sfxpool.Count; i++)
        {
            if (sfxpool[i].isPlaying && playingClips[i] == data.soundClip)
            {
                sfxpool[i].Stop();
                sfxpool[i].PlayOneShot(data.soundClip,data.volume);
                return;
            }
        }

        for (int i = 0; i < sfxpool.Count; i++)
        {
            if (!sfxpool[i].isPlaying)
            {
                playingClips[i] = data.soundClip;
                sfxpool[i].PlayOneShot(data.soundClip,data.volume);
                return;
            }
        }
    }

}
[System.Serializable]
public class AudioData
{
    public AudioClip soundClip;
    [Range(0,1)]public float volume=0.5f;
}
