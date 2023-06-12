using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Audio
{
	public string id;
	public AudioClip clip;
}
[Serializable]
public class Music
{
	public string id;
	public AudioClip[] clips;
	public float fadeIn = 0f;
	public float fadeOut = 0f;
	public float maxVolume = 1f;
	[HideInInspector] public int playedIndex = 0;

	public void ShuffleClips()
	{
		System.Random rng = new();
		int n = clips.Length;
		while (n > 1)
		{
			int k = rng.Next(n--);
			AudioClip temp = clips[n];
			clips[n] = clips[k];
			clips[k] = temp;
		}
	}

	public AudioClip GetNextClip()
	{
		if (playedIndex >= clips.Length)
		{
			ShuffleClips();
			playedIndex = 0;
		}
		return clips[playedIndex++];
	}
}

public class SoundController : MonoBehaviour
{
	public enum FadeState
	{
		None,
		FadingIn,
		FadingOut
	}
	private const float MaxPitch = 2f;
	private const float DefaultPitch = 1f;
	private float LowPassFilterMaxFreq = 22000f;
	private float LowPassFilterMinFreq = 1000f;

	public static SoundController instance;

    public Audio[] audioClips;
	public Music[] musicClips;

    public AudioSource shortClipsAudioSource;
    public AudioSource musicAudioSource;
	public AudioSource countdownAudioSource;
	private AudioLowPassFilter lowPassFilter;

	private Music lastPlayedMusic = new();
	private bool isSwapping;

	private float fadeInDuration;
	private float fadeOutDuration;
	private AudioClip fadingInClip;
	private float fadingTimer;
	private FadeState fadeState;
	private FadeState filterFadeState;

	private Music loopOrderMusic;
	private bool playLoopOrder;

	public AudioClip countdownClip;
	private bool countdownStarted;
	private float calculatedPitchStep;
	private float countdownTimer;
	private float countdownStepTimer;

	private float maxVolume;
	private float filterFadeDuration;

	public void Awake()
	{
		lowPassFilter = musicAudioSource.GetComponent<AudioLowPassFilter>();
		instance = this;
		foreach (Music music in musicClips)
		{
			foreach (AudioClip clip in music.clips)
				clip.LoadAudioData();
			music.ShuffleClips();
		}
		foreach (Audio audio in audioClips)
		{
			audio.clip.LoadAudioData();
		}
	}

	public void Update()
	{
		if (!enabled)
			return;

		switch (fadeState)
		{
			case FadeState.FadingIn:
				if (!musicAudioSource.isPlaying)
				{
					musicAudioSource.volume = 0;
					musicAudioSource.clip = fadingInClip;
					musicAudioSource.Play();
				}
				musicAudioSource.volume = Mathf.Lerp(musicAudioSource.volume, maxVolume, fadingTimer / fadeInDuration);
				fadingTimer += Time.deltaTime;
				if (musicAudioSource.volume >= maxVolume)
				{
					fadeState = FadeState.None;
					fadingTimer = 0;
					return;
				}
				break;

			case FadeState.FadingOut:
				musicAudioSource.volume = Mathf.Lerp(musicAudioSource.volume, 0, fadingTimer / fadeOutDuration);
				fadingTimer += Time.deltaTime;
				if (musicAudioSource.volume <= 0)
				{
					fadeState = FadeState.None;
					musicAudioSource.Stop();
					if (isSwapping)
					{
						fadingTimer = 0;
						fadeState = FadeState.FadingIn;
						isSwapping = false;
						return;
					}
				}
				break;
		}

		switch (filterFadeState)
		{
			case FadeState.FadingIn:
				lowPassFilter.cutoffFrequency = 
					Mathf.Lerp(lowPassFilter.cutoffFrequency, LowPassFilterMaxFreq, fadingTimer / filterFadeDuration);
				fadingTimer += Time.deltaTime;
				if (lowPassFilter.cutoffFrequency >= LowPassFilterMaxFreq)
				{
					fadingTimer = 0;
					filterFadeState = FadeState.None;
				}
				break;
			case FadeState.FadingOut:
				lowPassFilter.cutoffFrequency = 
					Mathf.Lerp(lowPassFilter.cutoffFrequency, LowPassFilterMinFreq, fadingTimer / filterFadeDuration);
				fadingTimer += Time.deltaTime;
				if (lowPassFilter.cutoffFrequency <= LowPassFilterMinFreq)
				{
					fadingTimer = 0;
					filterFadeState = FadeState.None;
				}
				break;
		}

		if (countdownStarted)
		{
			if (countdownTimer <= 0)
				countdownStarted = false;

			if (countdownStepTimer <= 0 && countdownStarted)
			{
				countdownAudioSource.PlayOneShot(countdownClip, 0.7f);
				countdownAudioSource.pitch += calculatedPitchStep;
				countdownStepTimer = 1;
			}
			countdownTimer -= Time.deltaTime;
			countdownStepTimer -= Time.deltaTime;
		}

		if (playLoopOrder)
		{
			if (!musicAudioSource.isPlaying)
				PlayMusic(loopOrderMusic, true);
		}
	}

	public void SetSound(bool enabled)
	{
		shortClipsAudioSource.mute = enabled;
		musicAudioSource.mute = enabled;
		countdownAudioSource.mute = enabled;
	}

	public void PlayShortClip(string id)
	{
		shortClipsAudioSource.PlayOneShot(GetShortClip(id), 0.7f);
	}
	
	public void PlayMusic(string id, bool loopOrder = false)
	{
		var temp = GetMusic(id);
		var clip = temp.GetNextClip();

		SetLoopOrder(loopOrder, temp);
		maxVolume = temp.maxVolume;
		if (musicAudioSource.isPlaying && fadeState == FadeState.None)
			SwapMusic(lastPlayedMusic.fadeOut, temp.fadeIn, clip);
		else
			FadeIn(temp.fadeIn, clip);

		lastPlayedMusic = temp;
	}

	private void SetLoopOrder(bool loopOrder, Music music = null)
	{
		playLoopOrder = loopOrder;
		musicAudioSource.loop = !loopOrder;

		if (loopOrder)
			loopOrderMusic = music;
	}

	public void PlayMusic(Music music, bool loopOrder = false)
	{
		SetLoopOrder(loopOrder, music);
		maxVolume = music.maxVolume;
		if (musicAudioSource.isPlaying)
			SwapMusic(lastPlayedMusic.fadeOut, music.fadeIn, music.GetNextClip());
		else
			FadeIn(music.fadeIn, music.GetNextClip());

		lastPlayedMusic = music;
	}

	public void StopMusic(bool needFade = true, bool loopOrder = false)
	{
		SetLoopOrder(loopOrder, loopOrderMusic);
		fadingTimer = 0;
		if (!needFade)
		{
			musicAudioSource.Stop();
			return;
		}
		FadeOut(lastPlayedMusic.fadeOut);
	}

	private void FadeIn(float duration, AudioClip clip)
	{
		StopMusic(false, playLoopOrder);
		fadingTimer = 0;
		fadeInDuration = duration;
		fadingInClip = clip;
		fadeState = FadeState.FadingIn;
	}

	private void FadeOut(float duration)
	{
		fadingTimer = 0;
		fadeOutDuration = duration;
		fadeState = FadeState.FadingOut;
	}

	private void SwapMusic(float fadeOut, float fadeIn, AudioClip clip, bool needFade = true)
	{
		fadingTimer = 0;
		fadeOutDuration = needFade ? fadeOut : 0.1f;
		fadeInDuration = fadeIn;
		fadingInClip = clip;
		isSwapping = true;
		fadeState = FadeState.FadingOut;
	}

	public void StartCountdown(int seconds)
	{
		if (countdownStarted)
			return;

		countdownAudioSource.pitch = DefaultPitch;
		countdownTimer = seconds;
		countdownStepTimer = 0;
		calculatedPitchStep = (MaxPitch - DefaultPitch) / seconds;
		countdownStarted = true;
	}

	public void ForceCountdownStop()
	{
		if (!countdownStarted)
			return;
		countdownStarted = false;
	}

	public void SetLowPassFilter(bool enable, float duration, bool needFade = true)
	{
		fadingTimer = 0;
		if (!needFade)
		{
			lowPassFilter.cutoffFrequency = enable ? LowPassFilterMinFreq : LowPassFilterMaxFreq;
			filterFadeState = FadeState.None;
			return;
		}

		filterFadeDuration = duration;
		filterFadeState = enable ? FadeState.FadingOut : FadeState.FadingIn;
	}

	public AudioClip GetShortClip(string id)
	{
		foreach (var audio in audioClips)
			if (audio.id == id)
				return audio.clip;
		throw new Exception("incorrect shortclip id");
	}

	public Music GetMusic(string id)
	{
		foreach (var music in musicClips)
			if (music.id == id)
				return music;
		throw new Exception("incorrect music id");
	}
}
