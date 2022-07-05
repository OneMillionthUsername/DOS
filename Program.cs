using System;
using System.Collections.Generic;
using System.IO;

namespace DOS
{
	class Program
	{
		static void Main()
		{
			bool inputValid = true;
			Directory.CreateDirectory("root");
			string path = Path.Combine(Directory.GetCurrentDirectory(), "root");
			Directory.SetCurrentDirectory(path);

			while (inputValid)
			{
				Menu(ref path, ref inputValid); 
			}
		}

		private static void Menu(ref string path, ref bool inputValid)
		{
			string[] unterordner = Directory.GetDirectories(path);
			string changeDirectory = "cd";
			string exitDirectory = "cd..";
			string listDirectory = "dir";
			string createDirectory = "d";
			string deleteDirectory = "del";
			string list = "list";
			string createFile = "f";
			string move = "move";
			string help = "help";

			Console.Write(path + @">");
			string input = Console.ReadLine();

			RegexInput(input, out string command, out string attribute);
			if (command == changeDirectory)
			{
				if (Directory.Exists(Path.Combine(path, attribute)))
				{
					Directory.SetCurrentDirectory(Path.Combine(path, attribute));
					path = Path.Combine(path, attribute);
					return;
				}
				else
				{
					Console.WriteLine("Directory not found.");
				}
			}
			else if (command == exitDirectory)
			{
				path = ExitDirectory(path);
			}
			else if (command == listDirectory)
			{
				ListDirectory(unterordner);
				return;
			}
			else if (command == deleteDirectory)
			{
				DeleteDirectory(path, attribute);
				return;
			}
			else if (command == createFile)
			{
				CreateFile(path, input);
				return;
			}
			else if (command == createDirectory)
			{
				CreateDirectory(path, input);
				return;
			}
			else if (command == list)
			{
				if (attribute == "all")
				{
					string[] allFile = Directory.GetFileSystemEntries(path);
					if (allFile.Length > 0)
					{
						foreach (var i in allFile)
						{
							Console.WriteLine(i);
						} 
					}
					else
					{
						Console.WriteLine("No files or directories found.");
					}
				}
				else
					ListFiles(path);
				return;
			}
			else if (command == move)
			{
				Move(path, input);
				return;
			}
			else if (command == help)
			{
				Help();
				return;
			}
			else
			{
				Console.WriteLine("Invalid input.");
				return;
			}
		}


		private static void Move(string path, string input)
		{
			string oldName = "";
			string newName = "";
			RegexInput(input, out _, out string name);
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsWhiteSpace(name[i]))
				{
					oldName = name.Remove(i).Trim().ToLower();
					newName = name[i..].Trim().ToLower();
					break;
				}
			}
			string[] allFiles = Directory.GetFileSystemEntries(path);
			foreach (var i in allFiles)
			{
				if (i.EndsWith(oldName))
				{
					Directory.Move(oldName, newName);
					return;
				}
			}
		}

		private static string ExitDirectory(string path)
		{
			DirectoryInfo directoryInfo = Directory.GetParent(path);
			path = directoryInfo.ToString();
			Directory.SetCurrentDirectory(path);
			return path;
		}

		private static void ListDirectory(string[] unterordner)
		{
			if (unterordner.Length < 1)
			{
				Console.WriteLine("No directories found.");
			}
			else
			{
				foreach (var i in unterordner)
				{
					Console.WriteLine(i);
				}
			}
		}

		private static void DeleteDirectory(string path, string attribute)
		{
			if (attribute.Length > 0)
			{
				if (Directory.Exists(Path.Combine(path, attribute)))
				{
					Directory.Delete(Path.Combine(path, attribute), true);
					//DirectoryInfo directoryInfo = Directory.GetParent(path);
					//path = directoryInfo.ToString();
					//Directory.SetCurrentDirectory(path); 
				}
				else
				{
					Console.WriteLine("Directory not found.");
				}
			}
			else
			{
				Console.WriteLine("Invalid syntax.");
			}
		}

		private static void ListFiles(string path)
		{
			string[] files = Directory.GetFiles(path);
			if (files.Length < 1)
			{
				Console.WriteLine("No files found.");
			}
			else
			{
				foreach (var i in files)
				{
					Console.WriteLine(i);
				}
			}
		}

		private static void Help()
		{
			Console.WriteLine("cd [name] = change directory");
			Console.WriteLine("cd.. = exit directory");
			Console.WriteLine("dir = list directory");
			Console.WriteLine("list -all = list files");
			Console.WriteLine("del [name] = delete directory");
			Console.WriteLine("f [name.extension] = create file");
			Console.WriteLine("d [name] = create dictonary");
			Console.WriteLine("move [old name] [new name] = renames a file or directory");
		}

		private static void CreateDirectory(string path, string input)
		{
			RegexInput(input, out _, out string directoryName);
			if (!Directory.Exists(Path.Combine(path, directoryName)))
			{
				Directory.CreateDirectory(Path.Combine(path, directoryName));
			}
			else
			{
				Console.WriteLine("Directory exists.");
			}
		}

		private static void CreateFile(string path, string input)
		{
			FileStream fs;
			RegexInput(input, out string _, out string fileExtension);
			if (!File.Exists(Path.Combine(path, fileExtension)))
			{
				fs = File.Create(Path.Combine(path, fileExtension));
				fs.Close();
			}
			else
			{
				Console.WriteLine("File exists.");
			}
		}

		private static void RegexInput(string input, out string command, out string output)
		{
			string prefix = "";
			string temp = "";
			bool whiteSpace = false;
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsWhiteSpace(input[i]))
				{
					prefix = input.Remove(i).Trim();
					temp = input[i..].Trim();
					whiteSpace = true;
					break;
				}
			}
			command = prefix;
			output = temp;
			if (!whiteSpace)
			{
				command = input;
				return;
			}
			if (prefix.ToLower() == "f")
			{
				for (int i = 0; i < temp.Length; i++)
				{
					if (char.IsPunctuation(temp[i]))
					{
						command = prefix.ToLower();
						prefix = temp.Remove(i).Trim();
						string attribute = temp[i..].Trim();
						output = prefix + attribute;
						return;
					}
				}
			}
			else if (prefix.ToLower() == "move")
			{
				for (int i = 0; i < temp.Length; i++)
				{
					if (char.IsWhiteSpace(temp[i]))
					{
						prefix = temp.Remove(i).Trim();
						string attribute = temp[i..].Trim();
						output = prefix + " " + attribute;
						return;
					}
				}
			}
			else
			{
				command = prefix.ToLower();
				output = temp.Trim();
			}
			//else if (prefix.ToLower() == "move")
			//{
			//	for (int i = 0; i < input.Length; i++)
			//	{
			//		if (char.IsWhiteSpace(input[i]))
			//		{
			//			output = input[i..].Trim();
			//			break;
			//		}
			//	}
			//}
		}
	}
}
