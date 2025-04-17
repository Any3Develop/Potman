﻿namespace Potman.Common.SerializeService.Abstractions
{
	public interface ISerializeService
	{
		/// <summary>
		/// Patch by key or create with target key
		/// </summary>
		void Patch(string key, string value);
		
		/// <summary>
		/// Get by key if key does not exist return empty string
		/// </summary>
		string Get(string key);
		
		/// <summary>
		/// Remove by key
		/// </summary>
		void Delete(string key);
		bool HasKey(string key);
		void Save();
		bool TryGet(string key, out string result);
	}
}