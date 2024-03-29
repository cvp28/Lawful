﻿using System.Xml;
using System.Text;
using System.Xml.Linq;
using Esprima.Ast;

namespace Lawful.GameLibrary;

// Standard file permissions set that is not hard to understand
public enum FilePermission : int
{
	Read,		// Able to get the file's contents
	Write,		// Able to modify the file's contents
	Execute		// Able to invoke the file as an executable, whether it is one or not
}

// Standard Windows-inspired directory permissions set
public enum DirectoryPermission : int
{
	Enter,			// Able to CD into the directory
	List,			// Able to list directory contents
	Modify			// Able to add/remove files/folders to/from this directory
}

// Might expand on this later
public enum FSNodeType
{
	File,
	Directory
}

public static class FSAPI
{
	public static dynamic Locate(UserSession Session, string Path)
	{
		XmlNode Traverser;

		if (Path.Length == 0)
			return null;

		if (Path[0] == '/')
			Traverser = Session.Host.FileSystemRoot;
		else
			Traverser = Session.PathNode;

		string[] PathElements = Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

		foreach (string Element in PathElements)
		{
			switch (Element)
			{
				case "*":
					if (Traverser.ChildNodes.Count == 0)
						return null;
					else if (Traverser.ChildNodes.Count == 1)
						return Traverser.FirstChild;
					else
						return Traverser.ChildNodes;

				case "..":
					if (Traverser.Name == "Root")
						return null;

					Traverser = Traverser.ParentNode;
					break;

				case ".":
					Traverser = Session.PathNode;
					break;

				default:
					var ChildNodes = Traverser.ChildNodes.Cast<XmlNode>();
					XmlNode Temp = ChildNodes.FirstOrDefault(node => node.Attributes["Name"].Value == Element);

					if (Temp is not null)
						Traverser = Temp;
					else
						return null;

					break;

					//	XmlNode TryDirectory = Traverser.SelectSingleNode($"Directory[@Name='{Element}']");
					//	XmlNode TryFile = Traverser.SelectSingleNode($"File[@Name='{Element}']");

					//	if (TryDirectory is not null)
					//		Traverser = TryDirectory;
					//	else if (TryFile is not null)
					//		return TryFile;
					//	else
					//		return null;
			}
		}

		return Traverser;
	}

	public static XmlNode LocateFile(UserSession Session, string Path)
	{
		dynamic Result = Locate(Session, Path);

		if (Result == null)
			return null;

		switch (Result)
		{
			case XmlNode n:
				if (n.Name != "File")
					return null;
				else
					return n;

			default:
				return null;
		}
	}

	public static XmlNode LocateDirectory(UserSession Session, string Path)
	{
		dynamic Result = Locate(Session, Path);

		if (Result == null)
			return null;

		switch (Result)
		{
			case XmlNode n:
				if (n.Name == "Root" || n.Name == "Directory")
					return n;
				else
					return null;

			default:
				return null;
		}
	}

	public static bool TryGetNode(UserSession Session, string Path, out dynamic Result)
	{
		Result = Locate(Session, Path);
		return Result != null;
	}

	public static bool TryGetNode(UserSession Session, string Path, FSNodeType Type, out XmlNode Node)
	{
		switch (Type)
		{
			case FSNodeType.File:
				Node = LocateFile(Session, Path);
				return Node != null;

			case FSNodeType.Directory:
				Node = LocateDirectory(Session, Path);
				return Node != null;

			default:
				Node = null;
				break;
		}

		return Node != null;
	}

	public static bool UserHasFilePermissions(UserSession Session, XmlNode File, params FilePermission[] Permissions)
	{
		var FilePermissions = File.GetFilePermissionsData();

		if (FilePermissions.Owner == Session.User.Username)
			return true;
		else if (Session.User.Username == "root")
			return TestPermissions(FilePermissions.RootPermissions, Permissions);
		else
			return TestPermissions(FilePermissions.OtherPermissions, Permissions);

		static bool TestPermissions(List<FilePermission> UserPermissions, FilePermission[] TestPermissions)
		{
			foreach (FilePermission fp in TestPermissions)
			{
				switch (fp)
				{
					case FilePermission.Read:
						if (!UserPermissions.Contains(FilePermission.Read))
							return false;
						break;

					case FilePermission.Write:
						if (!UserPermissions.Contains(FilePermission.Write))
							return false;
						break;

					case FilePermission.Execute:
						if (!UserPermissions.Contains(FilePermission.Execute))
							return false;
						break;
				}
			}

			return true;
		}
	}

	public static bool UserHasDirectoryPermissions(UserSession Session, XmlNode Directory, params DirectoryPermission[] Permissions)
	{
		var DirectoryPermissions = Directory.GetDirectoryPermissionsData();

		if (DirectoryPermissions.Owner == Session.User.Username)
			return true;
		else if (Session.User.Username == "root")
			return TestPermissions(DirectoryPermissions.RootPermissions, Permissions);
		else
			return TestPermissions(DirectoryPermissions.OtherPermissions, Permissions);

		static bool TestPermissions(List<DirectoryPermission> UserPermissions, DirectoryPermission[] TestPermissions)
		{
			foreach (DirectoryPermission dp in TestPermissions)
			{
				switch (dp)
				{
					case DirectoryPermission.Enter:
						if (!UserPermissions.Contains(DirectoryPermission.Enter))
							return false;
						break;

					case DirectoryPermission.List:
						if (!UserPermissions.Contains(DirectoryPermission.List))
							return false;
						break;

					case DirectoryPermission.Modify:
						if (!UserPermissions.Contains(DirectoryPermission.Modify))
							return false;
						break;
				}
			}

			return true;
		}
	}

	public static XmlNode GetNodeFromPath(this Computer Computer, string Query)
	{
		if (Query.Length == 0) { return null; }

		string[] QueryElements = Query.Split('/', StringSplitOptions.RemoveEmptyEntries);

		XmlNode Traverser = Computer.FileSystemRoot;

		foreach (string Element in QueryElements)
		{
			if (Traverser.SelectSingleNode($"File[@Name='{Element}']") != null)
			{
				Traverser = Traverser.SelectSingleNode($"File[@Name='{Element}']");
				// If we reach a file, then break because you cannot go further after finding a file
				break;
			}
			else if (Traverser.SelectSingleNode($"Directory[@Name='{Element}']") != null)
				Traverser = Traverser.SelectSingleNode($"Directory[@Name='{Element}']");
			else
				return null;
		}

		return Traverser;
	}

	#region XmlNode Extensions

	public static XmlNode GetNodeFromPath(this XmlNode Node, string Query)
	{
		if (Query.Length == 0) { return null; }

		string[] QueryElements = Query.Split('/', StringSplitOptions.RemoveEmptyEntries);

		XmlNode Traverser = Node;

		foreach (string Element in QueryElements)
		{
			var TraverserChildren = Traverser.ChildNodes.Cast<XmlNode>();

			XmlNode TryDirectory = TraverserChildren.FirstOrDefault(node => node.Name == "Directory" && node.Attributes["Name"].Value == Element);
			XmlNode TryFile = TraverserChildren.FirstOrDefault(node => node.Name == "File" && node.Attributes["Name"].Value == Element);

			if (TryDirectory is not null)
			{
				Traverser = TryDirectory;
			}
			else if (TryFile is not null)
			{
				Traverser = TryFile;
				break;
			}
			else
			{
				return null;
			}
		}

		return Traverser;
	}

	public static string GetPath(this XmlNode Node)
	{
		StringBuilder Path = new();

		if (Node == null)
			return string.Empty;

		if (Node.Name == "Root") { return "/"; }

		while (Node.Name != "Root")
		{
			Path.Insert(0, $"/{Node.Attributes["Name"].Value}");
			Node = Node.ParentNode;
		}

		return Path.ToString();
	}

	// Function to enumerate permissions data of a directory node
	public static (string Owner, List<DirectoryPermission> RootPermissions, List<DirectoryPermission> OtherPermissions) GetDirectoryPermissionsData(this XmlNode Node)
	{
		// <Directory Perms="root:elm:elm">

		string[] PermissionsElements = Node.Attributes["Perms"].Value.Split(':');

		if (PermissionsElements.Length != 3)
			return (null, null, null);

		string Owner = PermissionsElements[0];
		char[] RootPermissionsElement = PermissionsElements[1].ToArray();
		char[] OtherPermissionsElement = PermissionsElements[2].ToArray();

		List<DirectoryPermission> RootPermissions = ProcessDirectoryPermissions(RootPermissionsElement);
		List<DirectoryPermission> OtherPermissions = ProcessDirectoryPermissions(OtherPermissionsElement);

		return (Owner, RootPermissions, OtherPermissions);

		static List<DirectoryPermission> ProcessDirectoryPermissions(char[] PermissionsElement)
		{
			List<DirectoryPermission> Temp = new();

			foreach (char c in PermissionsElement)
			{
				switch (c)
				{
					case 'e':
					case 'E':
						Temp.Add(DirectoryPermission.Enter);
						break;

					case 'l':
					case 'L':
						Temp.Add(DirectoryPermission.List);
						break;

					case 'm':
					case 'M':
						Temp.Add(DirectoryPermission.Modify);
						break;

					case 'n':
					case 'N':
						Temp.Clear();
						return Temp;
				}
			}

			return Temp;
		}
	}

	public static (string Owner, List<FilePermission> RootPermissions, List<FilePermission> OtherPermissions) GetFilePermissionsData(this XmlNode Node)
	{
		// <File Perms="root:rwe:rwe">

		string[] PermissionsElements = Node.Attributes["Perms"].Value.Split(':');

		if (PermissionsElements.Length != 3)
			return (null, null, null);

		string Owner = PermissionsElements[0];
		char[] RootPermissionsElement = PermissionsElements[1].ToArray();
		char[] OtherPermissionsElement = PermissionsElements[2].ToArray();

		List<FilePermission> RootPermissions = ProcessFilePermissions(RootPermissionsElement);
		List<FilePermission> OtherPermissions = ProcessFilePermissions(OtherPermissionsElement);

		return (Owner, RootPermissions, OtherPermissions);

		static List<FilePermission> ProcessFilePermissions(char[] PermissionsElement)
		{
			List<FilePermission> Temp = new();

			foreach (char c in PermissionsElement)
			{
				switch (c)
				{
					case 'r':
					case 'R':
						Temp.Add(FilePermission.Read);
						break;

					case 'w':
					case 'W':
						Temp.Add(FilePermission.Write);
						break;

					case 'e':
					case 'E':
						Temp.Add(FilePermission.Execute);
						break;

					case 'n':
					case 'N':
						Temp.Clear();
						return Temp;
				}
			}

			return Temp;
		}
	}

	#endregion
}
