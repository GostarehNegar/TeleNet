﻿using GN.Library.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTProto.NET.Server.Infrastructure.Storage.Implementations.Internal.Internal
{

	public interface IDocumentStoreConnectionString
	{
		string Name { get; }
		string ConnectionString { get; }
	}
	public class DocumentStoreConnectionString : IDocumentStoreConnectionString
	{
		private string raw;
		public string Name { get; private set; }
		private KeyValueString keyValues;
		public DocumentStoreConnectionString(
			string connectionString = null,
			string name = null,
			string folder = null,
			Environment.SpecialFolder? folderOption = null)
		{
			init(connectionString, name, folder, folderOption);
		}

		public DocumentStoreConnectionString init(
			string connectionString,
			string name = null,
			string folder = null,
			Environment.SpecialFolder? folderOption = null)
		{
			this.raw = string.IsNullOrWhiteSpace(connectionString)
				? LibOptions.Current.DocumentStoreDefaultFileName
				: connectionString;
			if (!this.raw.Contains("="))
			{
				this.raw = $"Filename={this.raw}";
			}
			this.keyValues = new KeyValueString(this.raw);
			this.Name = name ?? "noname";
			if (folderOption.HasValue)
			{
				folder = Environment.GetFolderPath(folderOption.Value);
			}
			this.FileName = GetPath(folder);
			return this;
		}
		public bool IsConnectionString => this.raw != null && this.raw.Contains("=");
		public string ConnectionString => this.keyValues.ToString();

		public string GetPath(string folder = null)
		{
			var result = IsConnectionString ? this.keyValues.GetValue("filename") : this.raw;
			if (string.IsNullOrWhiteSpace(result))
			{
				result = Path
					.Combine(LibOptions.Default.DocumentStoreDefaultDirectory, LibOptions.Default.DocumentStoreDefaultFileName);
			}
			else if (string.IsNullOrWhiteSpace(Path.GetPathRoot(result)))
			{
				folder = string.IsNullOrWhiteSpace(folder)
					? LibOptions.Default.DocumentStoreDefaultDirectory
					: folder;
				result = Path
					.Combine(folder, result);
			}
			result = Path.GetFullPath(result);
			return result;
		}

		public string FileName
		{
			get
			{
				return this.keyValues.GetValue("filename");
			}
			set
			{
				this.keyValues.SetValue("Filename", value);
			}
		}

		public override string ToString()
		{
			return this.keyValues.ToString();
		}
	}

	class DocumentStoreConnectionStringHelper
	{
		public static string ValidateConnectionString(string connectionString)
		{
			var result = "";
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				connectionString =
					Path.Combine(LibOptions.Default.DocumentStoreDefaultDirectory, LibOptions.Default.DocumentStoreDefaultFileName);
			}
			else if (string.IsNullOrWhiteSpace(Path.GetPathRoot(connectionString)))
			{
				connectionString =
					Path.Combine(LibOptions.Default.DocumentStoreDefaultDirectory, connectionString);

			}
			result = Path.GetFullPath(connectionString);
			if (!Directory.Exists(Path.GetDirectoryName(result)))
				Directory.CreateDirectory(Path.GetDirectoryName(result));
			return result;
		}

	}

}
