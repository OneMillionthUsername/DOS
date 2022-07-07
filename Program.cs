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
				path = Directory.GetCurrentDirectory();
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
			string getSetPath = "path";

			Console.Write(path + ">");
			string input = Console.ReadLine();

			RegexInput(input, out string command, out string attribute);
			if (command == changeDirectory)
			{
				if (attribute is null)
				{
					Console.WriteLine("Invalid syntax.");
					return;
				}
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
			else if (command == getSetPath)
			{
				if (attribute is null)
				{
					Console.WriteLine(Directory.GetCurrentDirectory());
				}
				else
				{
					if (Directory.Exists(attribute))
					{
						Directory.SetCurrentDirectory(attribute);
						return;
					}
					else
					{
						Console.WriteLine("Path not found.");
					}
				}
			}
			else if (command == move)
			{
				Move(input);
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

		private static void Move(string input)
		{
			string currentPath = Directory.GetCurrentDirectory();
			string destFName;
			string destDirName;
			string sourceDirName;

			RegexInput(input, out _, out string attribute);
			if (attribute is not null)
			{
				//One path
				if (!attribute.Contains(' '))
				{
					//for files
					if (File.Exists(attribute))
					{
						destFName = attribute[attribute.LastIndexOf('\\')..];
						if (!File.Exists(currentPath + destFName))
						{
							File.Move(attribute, currentPath + destFName);
							return;
						}
						else
						{
							Console.WriteLine("File exists allready.");
						}
					}
					//for folders
					if (Directory.Exists(attribute))
					{
						destFName = attribute[attribute.LastIndexOf('\\')..];
						if (!Directory.Exists(currentPath + destFName))
						{
							Directory.Move(attribute, currentPath + destFName);
							return;
						}
						else
						{
							Console.WriteLine("Folder exists allready.");
							return;
						}
					}
					Console.WriteLine("Invalid path, file or folder.");
				}
				else
				{
					for (int i = 0; i < attribute.Length; i++)
					{
						//Two paths
						if (char.IsWhiteSpace(attribute[i]))
						{
							destDirName = attribute[i..].Trim().ToLower();
							sourceDirName = attribute.Remove(i).Trim().ToLower();
							//for files
							if (File.Exists(sourceDirName))
							{
								destFName = sourceDirName[sourceDirName.LastIndexOf('\\')..];
								destDirName += destFName;
								if (!File.Exists(destDirName))
								{
									File.Move(sourceDirName, destDirName);
									return;
								}
								else
								{
									Console.WriteLine("File exists allready.");
								}
							}
							//for directories
							else if (Directory.Exists(sourceDirName))
							{
								destFName = sourceDirName[sourceDirName.LastIndexOf('\\')..];
								destDirName += destFName;
								if (!Directory.Exists(destDirName))
								{
									Directory.Move(sourceDirName, destDirName);
									return;
								}
								else
								{
									Console.WriteLine("Folder exists allready.");
								}
							}
							else
							{
								Console.WriteLine("Invalid path, file or folder.");
							}
						}
					}
				}
			}
			else
			{
				Console.WriteLine("Invalid syntax.");
			}
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
			if (attribute is not null)
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
			Console.WriteLine("SYNTAX: command [obligatory param] -optional param");
			Console.WriteLine("cd [name] = Change directory");
			Console.WriteLine("cd.. = Exit directory");
			Console.WriteLine("dir = List directories");
			Console.WriteLine("list -all = List files and directories");
			Console.WriteLine("del [name] -extension = Delete directory or file");
			Console.WriteLine("f [name.extension] = Create file");
			Console.WriteLine("d [name] = Create directory");
			Console.WriteLine("rename [name] [new name] = Renames a file or directory");
			Console.WriteLine("move [source path] -destination path = Moves a file or directory to the working or a specific path");
			Console.WriteLine("path -path = Returns the current path or sets a new working path");
			Console.WriteLine("exit = Exit");
		}

		private static void CreateDirectory(string path, string input)
		{
			RegexInput(input, out _, out string directoryName);

			if (directoryName is not null)
			{
				if (!Directory.Exists(Path.Combine(path, directoryName)))
				{
					Directory.CreateDirectory(Path.Combine(path, directoryName));
				}
				else
				{
					Console.WriteLine("Directory exists.");
				}
			}
			else
			{
				Console.WriteLine("Invalid syntax.");
			}
		}

		private static void CreateFile(string path, string input)
		{
			FileStream fs;
			RegexInput(input, out string _, out string fileExtension);
			if (fileExtension is not null)
			{
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
			else
			{
				Console.WriteLine("Invalid syntax.");
			}
		}

		private static void RegexInput(string input, out string command, out string attribute)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsWhiteSpace(input[i]))
				{
					command = input.Remove(i).Trim();
					attribute = input[i..].Trim();
					return;
				}
			}
			command = input.ToLower().Trim();
			attribute = null;
			return;
		}
	}
}
