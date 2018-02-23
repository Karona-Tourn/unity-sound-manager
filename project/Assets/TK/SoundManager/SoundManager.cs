using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace TK.SoundManagement
{
	public partial class SoundManager : MonoBehaviour
	{
		[SerializeField]
		private List<SoundData> soundList = new List<SoundData> ();

		[SerializeField]
		private List<SoundData> musicList = new List<SoundData> ();

		private SoundPlayerGroup soundGroup = null;
		private SoundPlayerGroup musicGroup = null;

		private float soundVolume = 0;
		private float musicVolume = 0;
		private static SoundManager instance = null;

		public static UnityAction<float> onSoundVolumeChanged = null;
		public static UnityAction<float> onMusicVolumeChanged = null;

		public static SoundManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<SoundManager> ();
					if (instance == null)
					{
						GameObject go = new GameObject ("_SoundManager");
						instance = go.AddComponent<SoundManager> ();
					}
					DontDestroyOnLoad (instance.gameObject);
				}
				return instance;
			}
		}

		public float SoundVolume
		{
			get { return soundVolume; }
			set
			{
				soundVolume = Mathf.Clamp01 (value);
				soundGroup.Volume = soundVolume;
				if (onSoundVolumeChanged != null)
				{
					onSoundVolumeChanged (soundVolume);
				}
			}
		}

		public float MusicVolume
		{
			get { return musicVolume; }
			set
			{
				musicVolume = Mathf.Clamp01 (value);
				musicGroup.Volume = musicVolume;
				if (onMusicVolumeChanged != null)
				{
					onMusicVolumeChanged (musicVolume);
				}
			}
		}

		private void Awake ()
		{
			soundGroup = new SoundPlayerGroup (transform);
			musicGroup = new SoundPlayerGroup (transform);
			MusicVolume = 1f;
			SoundVolume = 1f;
		}

		public void AddSound (string name, AudioClip clip)
		{
			if (soundList.Exists (s => s.name.Equals (name)))
				return;
			soundList.Add (new SoundData (name, clip));
		}

		public void AddMusic (string name, AudioClip clip)
		{
			if (musicList.Exists (m => m.name.Equals (name)))
				return;
			musicList.Add (new SoundData (name, clip));
		}

		public bool IsPlayingMusic (string clipName)
		{
			return musicGroup.IsPlaying (clipName);
		}

		public SoundPlayer PlaySound (string name)
		{
			SoundData data = soundList.Find (s => s.name == name);
			if (data == null)
				return null;
			return soundGroup.Play (data, 0, false, soundVolume);
		}

		public SoundPlayer PlayMusic (string name, float fadeTime = 0, bool stopOther = true, bool random = false)
		{
			if (musicList.Count == 0)
				return null;
			SoundData data = null;
			if (random)
			{
				data = musicList[Random.Range (0, musicList.Count)];
			}
			else
			{
				data = musicList.Find (s => s.name == name);
				if (data == null)
					return null;
			}

			if (stopOther)
				musicGroup.Stop ();

			return musicGroup.Play (data, fadeTime, true, musicVolume);
		}

		public void StopMusic (float fadeTime)
		{
			musicGroup.Stop (fadeTime);
		}

		public void StopSound ()
		{
			soundGroup.Stop ();
		}

		private void Update ()
		{
			musicGroup.Update ();
			soundGroup.Update ();
		}
	}

}
