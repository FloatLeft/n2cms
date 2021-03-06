﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace N2.Configuration
{
	public class FileSystemElement : ConfigurationElement
	{
		[ConfigurationProperty("textFileExtensions", DefaultValue = ".cs,.vb,.js,.html,.htm,.xml,.aspx,.ascx,.ashx,.php,.txt")]
		public string TextFileExtensions
		{
			get { return (string)base["textFileExtensions"]; }
			set { base["textFileExtensions"] = value; }
		}

		public bool IsTextFile(string virtualPath)
		{
			return Array.FindIndex(
				TextFileExtensions.Split(','),
				(extension) => virtualPath.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)) >= 0;
		}
	}
}
