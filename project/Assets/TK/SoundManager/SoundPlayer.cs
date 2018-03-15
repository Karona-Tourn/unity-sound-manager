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

		private AudioSource _source = null;
		private float _fadeTime = 0f;
		private float _currFadeTime = 0f;
		private float _fadeDir = 1f;
		private FadeState _state = FadeState.None;
		private string _keyName = "";

		public bool IsFadingIn { get { return _state == FadeState.FadingIn; } }

		public bool IsFadingOut { get { return _state == FadeState.FadingOut; } }

		public bool IsFading { get { return _state != FadeState.None; } }

		public string KeyName { get { return _keyName; } }

		public bool IsPlaying { get { return _source.isPlaying; } }

		public void Play (string keyName, AudioClip clip, float fadeTime, float volume, bool loop)
		{
			_keyName = keyName;
			_fadeTime = fadeTime;
			_currFadeTime = 0f;

			_source.loop = loop;
			_source.clip = clip;

			if (fadeTime > 0f)
			{
				_fadeDir = 1f;
				_state = FadeState.FadingIn;
				_source.volume = 0f;
			}
			else
			{
				_state = FadeState.None;
				_source.volume = volume;
			}

			_source.Play ();
		}

		public void Stop (float fadeTime = 0f)
		{
			if (IsFadingOut || !IsPlaying)
			{
				return;
			}

			if (fadeTime > 0f)
			{
				float p = Mathf.Clamp01 (_currFadeTime / this._fadeTime);
				_fadeDir = -1f;
				_fadeTime = fadeTime;
				_currFadeTime = _fadeTime * p;
				_state = FadeState.FadingOut;
			}
			else
			{
				_source.Stop ();
			}
		}

		private void Init ()
		{
			_source = gameObject.AddComponent<AudioSource> ();
			_source.playOnAwake = false;
		}

		public void ExecuteUpdate (float volume)
		{
			float lastVolume = volume;

			if (IsFading)
			{
				_currFadeTime += Time.deltaTime * _fadeDir;
				_currFadeTime = Mathf.Clamp (_currFadeTime, 0f, _fadeTime);
				float percent = (_currFadeTime / _fadeTime);
				lastVolume = percent * volume;
				if (percent <= 0f || percent >= 1f || !IsPlaying)
				{
					if ( percent <= 0f ) _source.Stop ();
					_state = FadeState.None;
				}
			}

			if ( _source.volume != lastVolume ) _source.volume = lastVolume;
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
