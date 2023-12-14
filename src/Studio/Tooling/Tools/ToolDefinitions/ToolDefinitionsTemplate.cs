using System.Text.Json;
using System.Text.RegularExpressions;
using RedHerring.Studio.Models.Project.FileSystem;

namespace Mine.Studio;

public sealed class ToolDefinitionsTemplate
{
	private const string _typeId    = "DefinitionTemplate";
	private const int    _version   = 1;
	private const string _dataBegin = "//--- data begin ---";
	private const string _dataEnd   = "//--- data end ---";
	
	private ProjectScriptFileHeader.FileId   _fileId;
	private string _namespaceName;
	private string _className;

	private List<ToolDefinitionsTemplateField> _fields = new();

	public ToolDefinitionsTemplate(string namespaceName, string className)
	{
		_fileId      = new ProjectScriptFileHeader.FileId (Guid.NewGuid().ToString(), _typeId, _version);
		_namespaceName = namespaceName;
		_className      = className;
	}

	public void Write(string path)
	{
		using StreamWriter stream = File.CreateText(path);

		stream.WriteLine(ProjectScriptFileHeader.CreateHeaderString(_fileId));
		stream.WriteLine("//THIS FILE WAS GENERATED! DON'T MODIFY IT. ONLY THE NAMESPACE AND THE CLASS NAME CAN BE MODIFIED SAFELY!");

		if (!string.IsNullOrEmpty(_namespaceName))
		{
			stream.WriteLine($"namespace {_namespaceName};");
		}

		stream.WriteLine($"public sealed class {_className}");
		stream.WriteLine("{");
		stream.WriteLine("	" + _dataBegin);
		foreach (ToolDefinitionsTemplateField field in _fields)
		{
			stream.WriteLine($"	public {field.Type.Name} {field.Name} {{get; private set;}}");
		}
		stream.WriteLine("	" + _dataEnd);
		stream.WriteLine("}");
	}

	public bool Read(string path)
	{
		using StreamReader stream = File.OpenText(path);

		// header
		string? headerLine = stream.ReadLine();
		if (headerLine == null)
		{
			return false;
		}

		ProjectScriptFileHeader.FileId? fileId = ProjectScriptFileHeader.ReadFromStringLine(headerLine);
		if (fileId == null)
		{
			return false;
		}

		_fileId = fileId;

		// prepare regexes
		Regex classRegex     = new (@"class\s+(\w+)", RegexOptions.Compiled);
		Regex namespaceRegex = new (@"namespace\s+(\w+)", RegexOptions.Compiled);
		Regex propertyRegex  = new (@"public\s+(\w+)\s+(\w+)", RegexOptions.Compiled);
		
		// parse line by line
		bool classNameParsed     = false;
		bool namespaceNameParsed = false;
		bool parsingData         = false;
		while (!stream.EndOfStream)
		{
			string? line = stream.ReadLine();
			if (line == null)
			{
				break;
			}

			if (parsingData)
			{
				// parsing fields/properties
				if (line.Contains(_dataEnd))
				{
					parsingData = false;
					continue;
				}

				Match propertyMatch = propertyRegex.Match(line);
				if (!propertyMatch.Success)
				{
					continue;
				}

				string propertyType = propertyMatch.Groups[1].Captures[0].ToString();
				string propertyName = propertyMatch.Groups[2].Captures[0].ToString();

				_fields.Add(
					new ToolDefinitionsTemplateField
					{
						Name = propertyName,
						Type = typeof(int) // TODO
					}
				);
			}
			else
			{
				// parsing class/namespace/marks
				if (line.Contains(_dataBegin))
				{
					parsingData = true;
					continue;
				}

				if (!namespaceNameParsed)
				{
					Match namespaceMatch = namespaceRegex.Match(line);
					if (namespaceMatch.Success)
					{
						_namespaceName      = namespaceMatch.Groups[1].Captures[0].ToString();
						namespaceNameParsed = true;
						continue;
					}
				}

				if (!classNameParsed)
				{
					Match classMatch = classRegex.Match(line);
					if (classMatch.Success)
					{
						_className      = classMatch.Groups[1].Captures[0].ToString();
						classNameParsed = true;
						continue;
					}
				}
			}
		}

		return true;
	}
}