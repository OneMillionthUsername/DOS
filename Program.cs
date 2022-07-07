using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

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
			string rename = "rename";
			string exit = "exit";
			string help = "help";
			string move = "move";
			string getPath = "path";

			Console.Write(path + ">");
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
			else if (command.Contains('.'))
			{
				string file = "";
				if (command.Contains(".txt"))
				{
					file = String.Format(@" /C notepad.exe {0}", command);
				}
				Process.Start("cmd.exe", file);
			}
			else if (command == listDirectory)
			{
				ListDirectory(unterordner);
				return;
			}
			else if (command == deleteDirectory)
			{
				Delete(path, attribute);
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
			else if (command == rename)
			{
				Rename(path, input);
				return;
			}
			else if (command == getPath)
			{
				Console.WriteLine(Directory.GetCurrentDirectory());
			}
			else if (command == move)
			{
				Move(path, input);
			}
			else if (command == help)
			{
				Help();
				return;
			}
			else if (command == exit)
			{
				inputValid = false;
			}
			else
			{
				Console.WriteLine("Invalid input.");
				return;
			}
		}

		private static void Move(string path, string input)
		{
			string sourceDirName = "";
			string destDirName = "";
			RegexInput(input, out _, out string name);
			if (String.IsNullOrEmpty(name) || !(name.Contains(' ')))
			{
				Console.WriteLine("Invalid syntax.");
				return;
			}
			for (int i = 0; i < name.Length; i++)
			{
				if (char.IsWhiteSpace(name[i]))
				{
					destDirName = name[i..].Trim().ToLower();
					sourceDirName = name.Remove(i).Trim().ToLower();
					break;
				}
			}
			string destFolderName = sourceDirName.Substring(sourceDirName.LastIndexOf('\\'));
			
			Directory.Move(sourceDirName, destDirName + destFolderName);
		}

		private static void Rename(string path, string input)
		{
			string oldName = "";
			string newName = "";
			RegexInput(input, out _, out string name);
			if (String.IsNullOrEmpty(name) || !(name.Contains(' ')))
			{
				Console.WriteLine("Invalid syntax.");
				return;
			}
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
			Console.WriteLine("Directory or file not found.");
		}

		private static string ExitDirectory(string path)
		{
			DirectoryInfo directoryInfo = Directory.GetParent(path);
			if (directoryInfo != null)
			{
				path = directoryInfo.ToString();
				Directory.SetCurrentDirectory(path);
			}
			else
			{
				Console.WriteLine("Invalid path.");
			}
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

		private static void Delete(string path, string attribute)
		{
			if (attribute.Length > 0)
			{

				if (File.Exists(Path.Combine(path, attribute)))
				{
					File.Delete(Path.Combine(path, attribute));
				}
				else if (Directory.Exists(Path.Combine(path, attribute)))
				{
					Directory.Delete(Path.Combine(path, attribute), true);
				}
				else
				{
					Console.WriteLine("Directory or file not found.");
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
			Console.WriteLine("cd [name] = Change directory");
			Console.WriteLine("cd.. = Exit directory");
			Console.WriteLine("dir = List directory");
			Console.WriteLine("list -all = List files");
			Console.WriteLine("del [name] -extension = Delete directory or file");
			Console.WriteLine("f [name.extension] = Create file");
			Console.WriteLine("d [name] = Create dictonary");
			Console.WriteLine("rename [name] [new name] = Renames a file or directory");
			Console.WriteLine("move [source path] [destination path] = Moves a file or directory to the specific path");
			Console.WriteLine("path = Returns the current path");
			Console.WriteLine("exit = Exit");
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

		private static void RegexInput(string input, out string command, out string attribute)
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
			attribute = temp;
			if (!whiteSpace)
			{
				command = input;
				return;
			}
			if (command.ToLower() == "f")
			{
				for (int i = 0; i < temp.Length; i++)
				{
					if (char.IsPunctuation(temp[i]))
					{
						command = prefix.ToLower();
						prefix = temp.Remove(i).Trim();
						string fileExtension = temp[i..].Trim();
						fileExtension = prefix + fileExtension;
						return;
					}
				}
			}
			//else if (prefix.ToLower() == "move")
			//{
			//	if (!temp.Contains(' '))
			//	{
			//		Console.WriteLine("Invalid syntax.");
			//		return;
			//	}
			//	//for (int i = 0; i < temp.Length; i++)
			//	//{
			//	//	if (char.IsWhiteSpace(temp[i]))
			//	//	{
			//	//		prefix = temp.Remove(i).Trim();
			//	//		string fileName = temp[i..].Trim();
			//	//		fileName = prefix + " " + fileName;
			//	//		return;
			//	//	}
			//	//}
			//}
			else
			{
				command = prefix.ToLower();
				attribute = temp.Trim();
			}
		}
	}
}
