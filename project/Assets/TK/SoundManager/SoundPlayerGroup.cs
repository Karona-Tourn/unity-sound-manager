using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TK.SoundManagement
{
	public class SoundPlayerGroup
	{
		private Transform parent = null;
		private Queue<SoundPlayer> freePlayers = new Queue<SoundPlayer> ();
		private List<SoundPlayer> busyPlayers = new List<SoundPlayer> ();
		private float volume = 0;

		public float Volume
		{
			get { return volume; }
			set { volume = Mathf.Clamp01 (value); }
		}

		public SoundPlayerGroup (Transform parent)
		{
			this.parent = parent;
		}

		public SoundPlayer Play (SoundData data, float fadeTime, bool loop, float volume)
		{
			SoundPlayer player = null;

			lock (freePlayers)
			{
				if (freePlayers.Count > 0)
				{
					player = freePlayers.Dequeue ();
					player.gameObject.SetActive (true);
				}
				else
				{
					player = SoundPlayer.Create (parent);
				}
			}

			lock (busyPlayers)
			{
				player.Play (data.name, data.clip, fadeTime, volume, loop);
				busyPlayers.Add (player);
			}

			return player;
		}

		public void Stop (float fadeTime = 0)
		{
			for (int i = 0; i < busyPlayers.Count; i++)
			{
				busyPlayers[i].Stop (fadeTime);
			}
		}

		public bool IsPlaying (string name)
		{
			return busyPlayers.Exists (s => s.KeyName == name && s.IsPlaying);
		}

		public void Update ()
		{
			lock (busyPlayers)
			{
				for (int i = busyPlayers.Count - 1; i >= 0; i--)
				{
					SoundPlayer player = busyPlayers[i];
					if (player.IsPlaying == false)
					{
						busyPlayers.RemoveAt (i);
						player.gameObject.SetActive (false);
						lock (freePlayers)
						{
							freePlayers.Enqueue (player);
						}
						continue;
					}
					player.ExecuteUpdate (volume);
				}
			}
		}
	}
}
