using System.Text.RegularExpressions;
using Migration;
using OdinSerializer;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.UserInterface.Attributes;

namespace Mine.Studio;

[Serializable, SerializedClassId("5dc4705e-0dd6-4ccc-858b-231b6d915ffd")]
public sealed class DefinitionTemplate
{
	private const string _typeId    = "DefinitionTemplate";
	private const int    _version   = 1;
	private const string _dataBegin = "//--- data begin ---";
	private const string _dataEnd   = "//--- data end ---";
	
	[OdinSerialize] private ProjectScriptFileHeader _header;
	public                  ProjectScriptFileHeader Header => _header;
	
	[ShowInInspector, OdinSerialize] private string _namespaceName = null!;
	[ShowInInspector, OdinSerialize] private string _className     = null!;
	
	[ShowInInspector, OdinSerialize] private List<DefinitionTemplateField?>          _fields = new();
	public                                   IReadOnlyList<DefinitionTemplateField?> Fields => _fields;

	private DefinitionTemplate()
	{
	}

	public static DefinitionTemplate? CreateFromFile(string path)
	{
		DefinitionTemplate template = new();
		return template.Read(path) ? template : null;
	}

	public string? Validate()
	{
		if (!IsValidIdentifier(_namespaceName))
		{
			return "Invalid namespace!";
		}

		if (!IsValidIdentifier(_className))
		{
			return "Invalid class name!";
		}

		HashSet<string> names = new();
		for (int i = 0; i < _fields.Count; ++i)
		{
			if (_fields[i] == null)
			{
				return $"Field [{i}] is null!";
			}

			if (!IsValidIdentifier(_fields[i]!.Name))
			{
				return $"Invalid name of field [{i}]!";
			}

			if (!names.Add(_fields[i]!.Name))
			{
				return $"Field [{i}] has the same name as previous field!";
			}
		}

		return null;
	}

	public DefinitionTemplate(string namespaceName, string className)
	{
		_header        = new ProjectScriptFileHeader(Guid.NewGuid().ToString(), _typeId, _version);
		_namespaceName = namespaceName;
		_className     = className;
	}

	public void Write(string path)
	{
		using StreamWriter stream = File.CreateText(path);

		stream.WriteLine(_header.ToHeaderString());
		stream.WriteLine("//THIS FILE WAS GENERATED! DON'T MODIFY IT. ONLY THE NAMESPACE AND THE CLASS NAME CAN BE MODIFIED SAFELY!");

		if (!string.IsNullOrEmpty(_namespaceName))
		{
			stream.WriteLine($"namespace {_namespaceName};");
		}

		stream.WriteLine($"public sealed class {_className}");
		stream.WriteLine("{");
		stream.WriteLine("	" + _dataBegin);
		foreach (DefinitionTemplateField field in _fields)
		{
			stream.WriteLine($"	public {field.Type.ToCSharpType()} {field.Name} {{get; private set;}}");
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

		ProjectScriptFileHeader? header = ProjectScriptFileHeader.ReadFromStringLine(headerLine);
		if (header == null)
		{
			return false;
		}

		_header = header;

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

				DefinitionTemplateField  field = new(propertyType.ToTemplateType(), propertyName);
				_fields.Add(field);
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

	private bool IsValidIdentifier(string identifier)
	{
		if (string.IsNullOrEmpty(identifier))
		{
			return false;
		}

		if (identifier.Any(ch => ch <= 32))
		{
			return false;
		}

		if (identifier[0] >= '0' && identifier[0] <= '9')
		{
			return false;
		}

		return true;
	}
}

#region Migration
[MigratableInterface(typeof(DefinitionTemplate))]
public interface IDefinitionTemplateMigratable;
    
[Serializable, LatestVersion(typeof(DefinitionTemplate))]
public class DefinitionTemplate_000 : IDefinitionTemplateMigratable
{
	public IProjectScriptFileHeaderMigratable _header;
	
	public string _namespaceName;
	public string _className;
	
	[MigrateField] public List<IDefinitionTemplateFieldMigratable?> _fields;
}
#endregion