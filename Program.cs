using System;
using System.Collections.Generic;
using System.IO;

namespace DOS
{
	class Program
	{
		static void Main(string[] args)
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
			//max 4chars length
			bool directoryValid = false;
			string[] unterordner = Directory.GetDirectories(path);
			string changeDirectory = "cd";
			string exitDirectory = "cd..";
			string listDirectory = "dir";
			string createDirectory = "d";
			string deleteDirectory = "del";
			string listFiles = "list";
			string createFile = "f";
			string help = "help";

			Console.Write(path + @">");
			string input = Console.ReadLine();
			string command = "";
			string attribute = "";

			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsWhiteSpace(input[i]))
				{
					command = input.Remove(i).Trim().ToLower();
					attribute = input[i..].Trim().ToLower();
					break;
				}
				else
				{
					command = input;
				}
			}

			if (command == changeDirectory)
			{
				foreach (var i in unterordner)
				{
					if (i.ToLower().Contains(attribute))
					{
						path = i;
						directoryValid = true;
						break;
					}
				}
				if (directoryValid)
				{
					Directory.SetCurrentDirectory(path);
				}
				else
				{
					Console.WriteLine("Directory not found.");
				}
			}
			else if (command == exitDirectory)
			{
				DirectoryInfo directoryInfo = Directory.GetParent(path);
				path = directoryInfo.ToString();
				Directory.SetCurrentDirectory(directoryInfo.ToString());
			}
			else if (command == listDirectory)
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
			else if (command == deleteDirectory)
			{
				DirectoryInfo directoryInfo = Directory.GetParent(path);
				path = directoryInfo.ToString();
				Directory.SetCurrentDirectory(path);
				Directory.Delete(Path.Combine(path, attribute), true);
			}
			else if (command == createFile)
			{
				CreateFile(path, input);
			}
			else if (command == createDirectory)
			{
				CreateDirectory(path, input);
			}
			else if (command == listFiles)
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
			else if (command == help)
			{
				Help();
			}
			else
			{
				Console.WriteLine("Invalid input.");
				//exit;
				//inputValid = false;
			}
		}

		private static void Help()
		{
			Console.WriteLine("cd [name] = change directory");
			Console.WriteLine("cd.. = exit directory");
			Console.WriteLine("dir = list directory");
			Console.WriteLine("list = list files");
			Console.WriteLine("del [name] = delete directory");
			Console.WriteLine("f [name.extension] = create file");
			Console.WriteLine("d [name] = create dictonary");
		}

		private static void CreateDirectory(string path, string input)
		{
			string directoryName = "";
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsWhiteSpace(input[i]))
				{
					directoryName = input[i..].Trim();
					break;
				}
			}
			Directory.CreateDirectory(Path.Combine(path, directoryName));
		}

		private static void CreateFile(string path, string input)
		{
			FileStream fs;
			string fileName = "";
			string fileExtension = "";
			string temp = "";
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsWhiteSpace(input[i]))
				{
					temp = input[i..].Trim();
					break;
				}
			}
			for (int i = 0; i < temp.Length; i++)
			{
				if (char.IsPunctuation(temp[i]))
				{
					fileName = temp.Remove(i).Trim();
					fileExtension = temp[i..].Trim();
					break;
				}
			}
			fs = File.Create(Path.Combine(path, fileName + fileExtension));
			fs.Close();
		}
	}
}
