using System.Collections.Generic;
using UnityEngine;

namespace TK.SoundManagement
{
	public class SoundPlayerGroup
	{
		private Transform _parent = null;
		private Queue<SoundPlayer> _freePlayers = new Queue<SoundPlayer> ();
		private List<SoundPlayer> _busyPlayers = new List<SoundPlayer> ();
		private float _volume = 0;
		private object _lock = new object();

		public float Volume
		{
			get { return _volume; }
			set { _volume = Mathf.Clamp01 ( value ); }
		}

		public SoundPlayerGroup ( Transform parent )
		{
			_parent = parent;
		}

		public SoundPlayer Play ( SoundData data, float fadeTime, bool loop, float volume )
		{
			SoundPlayer player = null;

			lock ( _lock )
			{
				if ( _freePlayers.Count > 0 )
				{
					player = _freePlayers.Dequeue ();
					player.gameObject.SetActive ( true );
				}
				else
				{
					player = SoundPlayer.Create ( _parent );
				}

				player.Play ( data.name, data.clip, fadeTime, volume, loop );
				_busyPlayers.Add ( player );
			}

			return player;
		}

		public void Stop ( float fadeTime = 0 )
		{
			for ( int i = 0; i < _busyPlayers.Count; i++ )
			{
				_busyPlayers[i].Stop ( fadeTime );
			}
		}

		public bool IsPlaying ( string name )
		{
			return _busyPlayers.Exists ( s => s.KeyName == name && s.IsPlaying );
		}

		public void Update ()
		{
			lock ( _lock )
			{
				for ( int i = _busyPlayers.Count - 1; i >= 0; i-- )
				{
					SoundPlayer player = _busyPlayers[i];
					if ( player.IsPlaying == false )
					{
						_busyPlayers.RemoveAt ( i );
						player.gameObject.SetActive ( false );
						_freePlayers.Enqueue ( player );
						continue;
					}
					player.ExecuteUpdate ( _volume );
				}
			}
		}
	}
}
