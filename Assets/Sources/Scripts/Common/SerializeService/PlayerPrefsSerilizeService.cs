using Potman.Common.SerializeService.Abstractions;
using UnityEngine;

namespace Potman.Common.SerializeService
{
	public class PlayerPrefsSerilizeService : ISerializeService
	{
		public void Patch(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		public string Get(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		public void Delete(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}

		public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public void Save()
		{
			PlayerPrefs.Save();
		}

		public bool TryGet(string key, out string result)
		{
			if (!HasKey(key))
			{
				result = string.Empty;
				return false;
			}

			result = Get(key);
			return true;
		}
	}
}