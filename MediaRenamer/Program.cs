using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace MediaRenamer
{
	class Program
	{
		static void Main(string[] args)
		{
			string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//string path = @"F:\del hdd\delaram movies\Film\New folder";

			string[] directories = Directory.GetDirectories(path);

			for (int i = 0; i < directories.Length; ++i)
			{
				string directory = directories[i];
				string directoryName = Path.GetFileName(directory);

				string name = FindName(directoryName);

				if (name.GetHashCode() != directoryName.GetHashCode())
				{
					int count = 0;

					while (Directory.Exists(Path.GetDirectoryName(directory) + "/" + name))
						name += " (Copy " + ++count + ")";

					Directory.Move(directory, Path.GetDirectoryName(directory) + "/" + name);
				}

				Console.Clear();
				Console.WriteLine((i + 1) / (float)directories.Length * 100 + "% Renamed");
			}
		}

		//static void Main(string[] args)
		//{
		//	string path1 = @"F:\To Backup\#Movies";
		//	string path2 = @"H:\Movies";
		//	string[] duplicates = FindDuplicate(path1, path2);

		//	for (int i = 0; i < duplicates.Length; ++i)
		//		Console.WriteLine(duplicates[i]);

		//	Console.WriteLine("Ready to Delete from [" + path1 + "] ?");
		//	Console.ReadLine();

		//	for (int i = 0; i < duplicates.Length; ++i)
		//		Directory.Delete(path1 + "/" + duplicates[i]);
		//}

		private static string FindName(string Name)
		{
			WebClient client = new WebClient();
			string result = client.DownloadString("https://www.google.com/search?q=" + Name);

			//<a href="/url?q=https://www.imdb.com/title/tt0096895/&amp;sa=U&amp;ved=0ahUKEwjRr6v5ut3dAhXGEiwKHc5cDqUQFggrMAU&amp;usg=AOvVaw0caDEINhlRq0VRDJiTZpH1"><b>Batman</b> (1989) - IMDb
			int index = result.IndexOf(" - IMDb</a>");
			if (index == -1)
				return Name;

			result = result.Substring(0, index);

			index = result.LastIndexOf("<a href=\"/url?q=https://www.imdb.com");
			if (index == -1)
				return Name;

			result = result.Substring(index);

			index = result.IndexOf("<b>");
			if (index == -1)
				return Name;

			result = result.Substring(index + 3);

			result = result.Replace("<b>", "");
			result = result.Replace("</b>", "");
			result = result.Replace("&#8211; ", "");
			result = result.Replace("&#39;", "'");
			result = result.Replace(": ", " - ");
			result = result.Replace("/", "-");

			index = result.LastIndexOf("(");
			if (index == -1)
				return result;

			int closeIndex = result.LastIndexOf(")");

			result = "[" + result.Substring(index + 1, closeIndex - index - 1) + "] " + result.Substring(0, index - 1);

			return result;
		}

		private static string[] FindDuplicate(string Path1, string Path2)
		{
			string[] directories1 = Directory.GetDirectories(Path1);
			string[] directories2 = Directory.GetDirectories(Path2);

			List<string> names = new List<string>();

			for (int i = 0; i < directories1.Length; ++i)
			{
				string directoryName = Path.GetFileName(directories1[i]);

				for (int j = 0; j < directories2.Length; ++j)
					if (Path.GetFileName(directories2[j]) == directoryName)
						names.Add(directoryName);
			}

			return names.ToArray();
		}
	}
}
