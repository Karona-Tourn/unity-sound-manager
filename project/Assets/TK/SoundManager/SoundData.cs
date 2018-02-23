using UnityEngine;

namespace TK.SoundManagement
{
	public class SoundData
	{
		public string name;
		public AudioClip clip;

		public SoundData(string name, AudioClip clip)
		{
			this.name = name;
			this.clip = clip;
		}
	}
}
