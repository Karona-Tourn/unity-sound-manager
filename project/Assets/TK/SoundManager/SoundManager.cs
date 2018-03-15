using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace TK.SoundManagement
{
	public class SoundManager : MonoBehaviour
	{
		[SerializeField]
		private List<SoundData> _soundList = new List<SoundData> ();

		[SerializeField]
		private List<SoundData> _musicList = new List<SoundData> ();

		private SoundPlayerGroup _soundGroup = null;
		private SoundPlayerGroup _musicGroup = null;

		private float _soundVolume = 0f;
		private float _musicVolume = 0f;
		static private SoundManager instance = null;

		static public UnityAction<float> onSoundVolumeChanged = null;
		static public UnityAction<float> onMusicVolumeChanged = null;

		static public SoundManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<SoundManager> ();
					if ( instance == null ) new GameObject ("_SoundManager", typeof(SoundManager));
					else DontDestroyOnLoad ( instance.gameObject );
				}
				return instance;
			}
		}

		public float SoundVolume
		{
			get { return _soundVolume; }
			set
			{
				_soundVolume = Mathf.Clamp01 (value);
				_soundGroup.Volume = _soundVolume;
				if (onSoundVolumeChanged != null)
				{
					onSoundVolumeChanged (_soundVolume);
				}
			}
		}

		public float MusicVolume
		{
			get { return _musicVolume; }
			set
			{
				_musicVolume = Mathf.Clamp01 (value);
				_musicGroup.Volume = _musicVolume;
				if (onMusicVolumeChanged != null)
				{
					onMusicVolumeChanged (_musicVolume);
				}
			}
		}

		private void Awake ()
		{
			if ( instance == null )
			{
				instance = this;
				DontDestroyOnLoad ( gameObject );

				_soundGroup = new SoundPlayerGroup ( transform );
				_musicGroup = new SoundPlayerGroup ( transform );
				MusicVolume = 1f;
				SoundVolume = 1f;
			}
		}

		private void Start ()
		{
			if ( !Equals ( instance ) ) Destroy ( gameObject );
		}

		public void AddSound (string name, AudioClip clip)
		{
			if ( _soundList.Exists ( s => s.name.Equals ( name ) ) ) return;
			_soundList.Add (new SoundData (name, clip));
		}

		public void AddMusic (string name, AudioClip clip)
		{
			if ( _musicList.Exists ( m => m.name.Equals ( name ) ) ) return;
			_musicList.Add (new SoundData (name, clip));
		}

		public bool IsPlayingMusic (string clipName)
		{
			return _musicGroup.IsPlaying (clipName);
		}

		public SoundPlayer PlaySound (string name)
		{
			SoundData data = _soundList.Find (s => s.name == name);
			if ( data == null ) return null;
			return _soundGroup.Play (data, 0, false, _soundVolume);
		}

		public SoundPlayer PlayMusic (string name, float fadeTime = 0, bool stopOther = true, bool random = false)
		{
			if ( _musicList.Count == 0 ) return null;

			SoundData data = null;
			if (random)
			{
				data = _musicList[Random.Range (0, _musicList.Count)];
			}
			else
			{
				data = _musicList.Find (s => s.name == name);
				if ( data == null ) return null;
			}

			if ( stopOther ) _musicGroup.Stop ();

			return _musicGroup.Play (data, fadeTime, true, _musicVolume);
		}

		public void StopMusic (float fadeTime)
		{
			_musicGroup.Stop (fadeTime);
		}

		public void StopSound ()
		{
			_soundGroup.Stop ();
		}

		private void Update ()
		{
			_musicGroup.Update ();
			_soundGroup.Update ();
		}
	}

}
