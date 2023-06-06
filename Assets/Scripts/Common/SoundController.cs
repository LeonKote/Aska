using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Audio
{
	public string id;
	public AudioClip clip;
}
public class SoundController : MonoBehaviour
{
	public static SoundController instance;
    public Audio[] audioClips;
	public Audio[] musicClips;
    public AudioSource shortClipsAudioSource;
    public AudioSource musicAudioSource;

	public float volumeMultiplier = 1f;

	public void Start()
	{
		instance = this;
	}

	public void PlayShortClip(string id)
	{
		shortClipsAudioSource.PlayOneShot(GetShortClip(id), 0.5f * volumeMultiplier);
	}

	public AudioClip GetShortClip(string id)
	{
		foreach (var audio in audioClips)
			if (audio.id == id)
				return audio.clip;
		return null;
	}
}
