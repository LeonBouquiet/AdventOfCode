using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Shared;

namespace Day07
{
	public abstract class FileSystemObject
	{
		public string Name { get; set; }
		public virtual int Size { get; set; }

		public Directory Parent { get; set; }

		public FileSystemObject(string name, Directory parent)
		{
			Name = name;
			Parent = parent;
		}

		public static FileSystemObject Parse(string line, Directory parent)
		{
			string[] parts = line.Split(' ');
			if (parts[0] == "dir")
				return new Directory(parts[1], parent);
			else
				return new File(parts[1], Int32.Parse(parts[0]), parent);
		}
	}

	public class File: FileSystemObject
	{
		public File(string name, int size, Directory parent)
			: base(name, parent)
		{
			Size = size;
		}

		public override string ToString() => $"{Name} ({Size} bytes)";
	}

	public class Directory : FileSystemObject
	{
		public List<FileSystemObject> Contents { get; set; } = new List<FileSystemObject>();

		public override int Size
		{
			get { return Contents.Select(fso => fso.Size).Sum(); }
			set { throw new Exception(); }
		}
		public Directory(string name, Directory parent)
			: base(name, parent)
		{
		}

		public override string ToString() => $"dir {Name}";
	}

	public class Program
	{
		private static Directory _root = null!;
		private static List<Directory> _allDirectories = null!;

		public static void Main(string[] args)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			ParseFileSystemInfo();

			Part1();
			Part2();
			stopwatch.Stop();

			Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.ElapsedTicks} ticks).");
		}

		private static void ParseFileSystemInfo()
		{
			//Parse the input in commands with their output.
			var commandsWithOutput = new List<(string Command, List<string> Output)>();
			foreach(string line in InputReader.Read<Program>())
			{
				if (line.StartsWith("$"))
					commandsWithOutput.Add(new (line.Substring(2), new List<string>()));
				else
					commandsWithOutput.Last().Output.Add(line);
			}

			_root = new Directory("", null!);
			_allDirectories = new List<Directory>() { _root };

			Directory current = _root;
			foreach(var commandWithOutput in commandsWithOutput)
			{
				if (commandWithOutput.Command.StartsWith("cd"))
				{
					string argument = commandWithOutput.Command.Split(' ')[1];
					switch (argument)
					{
						case "/":
							current = _root;
							break;
						case "..":
							current = current.Parent;
							break;
						default:
							current = current.Contents
								.OfType<Directory>()
								.Where(dir => dir.Name == argument)
								.Single();
							break;
					}
				}
				else if (commandWithOutput.Command == "ls")
				{
					current.Contents = commandWithOutput.Output
						.Select(line => FileSystemObject.Parse(line, current))
						.ToList();

					_allDirectories.AddRange(current.Contents.OfType<Directory>());
				}
				else
					throw new Exception();
			}
		}

		private static void Part1()
		{
			var smallDirs = _allDirectories
				.Where(dir => dir.Size <= 100000)
				.ToList();

			int result = smallDirs.Select(dir => dir.Size).Sum();
			Console.WriteLine($"The result of part 1 is: {result}");
		}

		private static void Part2()
		{
			int usedSpace = _root.Size;
			int unusedSpace = 70_000_000 - usedSpace;
			int sizeToFreeUp = 30_000_000 - unusedSpace;

			Directory directoryToDelete = _allDirectories
				.Where(dir => dir.Size >= sizeToFreeUp)
				.OrderBy(dir => dir.Size)
				.First();

			int result = directoryToDelete.Size;
			Console.WriteLine($"The result of part 2 is: {result}");
		}

	}
}
