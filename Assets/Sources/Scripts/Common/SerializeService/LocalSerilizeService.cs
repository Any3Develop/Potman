using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Potman.Common.SerializeService.Abstractions;
using UnityEngine;

namespace Potman.Common.SerializeService
{
	public class LocalSerilizeService : ISerializeService
	{
		private readonly Dictionary<string, string> storage;

		public LocalSerilizeService()
		{
			try
			{
				var json = Decrypt(File.ReadAllText(SerilizedHelper.Path));
				storage = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			}
			catch (Exception e)
			{
				Debug.Log($"[{GetType().Name}] Can't load save data, Message : {e.Message}");
				storage = new Dictionary<string, string>();
			}
		}

		public void Patch(string key, string value)
		{
			AssertKey(key);
			storage[key] = value;
		}

		public string Get(string key)
		{
			AssertKey(key);
			return storage.TryGetValue(key, out var value) ? value : string.Empty;
		}

		public void Delete(string key)
		{
			AssertKey(key);
			storage.Remove(key);
		}

		public bool HasKey(string key)
		{
			return !string.IsNullOrEmpty(key) && storage.ContainsKey(key);
		}

		public void Save()
		{
			try
			{
				var json = JsonConvert.SerializeObject(storage);
				File.WriteAllText(SerilizedHelper.Path, Crypt(json));
			}
			catch (Exception e)
			{
				Debug.LogError($"[{GetType().Name}] Can't save data, Message : {e.Message}");
			}
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

		private void AssertKey(string value)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException($"[{GetType().Name}] Key is null or empty");
		}

		private static string Crypt(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
#if UNITY_EDITOR
			return text;
#else
			return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(text));
#endif
		}

		private static string Decrypt(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;
#if UNITY_EDITOR
			return text;
#else
			return System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(text));
#endif
		}
	}
}