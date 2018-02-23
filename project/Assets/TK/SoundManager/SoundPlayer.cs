using UnityEngine;

namespace TK.SoundManagement
{
	public class SoundPlayer : MonoBehaviour
	{
		public enum FadeState
		{
			None,
			FadingIn,
			FadingOut,
		}

		private AudioSource source = null;
		private float fadeTime = 0f;
		private float currFadeTime = 0f;
		private float fadeDir = 1f;
		private FadeState state = FadeState.None;
		private string keyName = "";

		public bool IsFadingIn { get { return state == FadeState.FadingIn; } }

		public bool IsFadingOut { get { return state == FadeState.FadingOut; } }

		public bool IsFading { get { return state != FadeState.None; } }

		public string KeyName { get { return keyName; } }

		public bool IsPlaying { get { return source.isPlaying; } }

		public void Play (string keyName, AudioClip clip, float fadeTime, float volume, bool loop)
		{
			this.keyName = keyName;
			this.fadeTime = fadeTime;
			currFadeTime = 0f;

			source.loop = loop;
			source.clip = clip;

			if (fadeTime > 0f)
			{
				fadeDir = 1f;
				state = FadeState.FadingIn;
				source.volume = 0f;
			}
			else
			{
				state = FadeState.None;
				source.volume = volume;
			}

			source.Play ();
		}

		public void Stop (float fadeTime = 0f)
		{
			if (IsFadingOut || !IsPlaying)
			{
				return;
			}

			if (fadeTime > 0f)
			{
				float p = Mathf.Clamp01 (currFadeTime / this.fadeTime);
				fadeDir = -1f;
				this.fadeTime = fadeTime;
				currFadeTime = this.fadeTime * p;
				state = FadeState.FadingOut;
			}
			else
			{
				source.Stop ();
			}
		}

		private void Init ()
		{
			source = gameObject.AddComponent<AudioSource> ();
			source.playOnAwake = false;
		}

		public void ExecuteUpdate (float volume)
		{
			float lastVolume = volume;

			if (IsFading)
			{
				currFadeTime += Time.deltaTime * fadeDir;
				currFadeTime = Mathf.Clamp (currFadeTime, 0f, fadeTime);
				float percent = (currFadeTime / fadeTime);
				lastVolume = percent * volume;
				if (percent <= 0f || percent >= 1f || !IsPlaying)
				{
					if (percent <= 0f)
						source.Stop ();
					state = FadeState.None;
				}
			}

			if (source.volume != lastVolume)
				source.volume = lastVolume;
		}

		public static SoundPlayer Create (Transform parent = null)
		{
			GameObject go = new GameObject ("SoundPlayer");
			SoundPlayer player = go.AddComponent<SoundPlayer> ();
			go.transform.SetParent (parent, false);
			player.Init ();
			return player;
		}
	}
}
