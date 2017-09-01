using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayList : MonoBehaviour {
	
	[Header("Audio play list")]
	[SerializeField]	protected AudioClip[] m_AudioList;

	[Header("Audio setting")]
	[SerializeField]	EAudioPlayType m_PlayType;
	[SerializeField]	protected bool m_PlayOnAwake = true;
	[Range(0f, 1f)]
	[SerializeField]	protected float m_Volume = 0.5f;

	protected AudioSource m_AudioSource;
	protected int m_PlayListIndex = 0;

	public bool playOnAwake {
		get { return this.m_PlayOnAwake; }
		set { 
			this.playOnAwake = value; 
			this.m_AudioSource.playOnAwake = value;
		}
	}

	public bool isPlaying {
		get { return this.m_AudioSource.isPlaying; }
	}

	public enum EAudioPlayType: int {
		JustFirst 	= 0,
		Random 		= 1,
		RoundRobin 	= 2
	}

	protected virtual void Awake() {
		this.m_AudioSource = this.GetComponent<AudioSource> ();
		this.m_AudioSource.playOnAwake 	= this.m_PlayOnAwake;
		this.m_AudioSource.volume 		= this.m_Volume;
		this.m_PlayListIndex 			= 0;
	}

	protected virtual void LateUpdate() {
		if (this.m_AudioList.Length > 0 
			&& this.m_AudioSource.isPlaying == false) {
			switch (this.m_PlayType) {
			case EAudioPlayType.JustFirst:
				this.PlayJustFirst ();
				break;
			case EAudioPlayType.Random:
				this.PlayRandom ();
				break;
			case EAudioPlayType.RoundRobin:
				this.PlayRoundRobin ();
				break;
			default:
				this.PlayJustFirst ();
				break;
			}
		}
		this.m_AudioSource.volume 		= this.m_Volume;
	}

	protected void PlayJustFirst() {
		this.m_AudioSource.loop = true;
		this.m_AudioSource.clip = this.m_AudioList [0];
		this.m_AudioSource.Play ();
	}

	protected void PlayRandom() {
		this.m_PlayListIndex = Random.Range (0, this.m_AudioList.Length);
		this.m_AudioSource.loop = false;
		this.m_AudioSource.clip = this.m_AudioList[this.m_PlayListIndex];
		this.m_AudioSource.Play ();
	}

	protected void PlayRoundRobin() {
		this.m_AudioSource.loop = false;
		this.m_AudioSource.clip = this.m_AudioList[this.m_PlayListIndex];
		this.m_PlayListIndex = (this.m_PlayListIndex + 1) % this.m_AudioList.Length;
		this.m_AudioSource.Play ();
	}

}
