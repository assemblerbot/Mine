using System.Text.RegularExpressions;
using Migration;
using OdinSerializer;
using RedHerring.Studio.Models.Project;
using RedHerring.Studio.Models.Project.FileSystem;
using RedHerring.Studio.Models.ViewModels.Console;
using RedHerring.Studio.UserInterface.Attributes;

namespace Mine.Studio;

// serialized within definition asset!
[Serializable, SerializedClassId("5dc4705e-0dd6-4ccc-858b-231b6d915ffd")]
public sealed class DefinitionTemplate
{
	private const string _typeId    = "DefinitionTemplate";
	private const int    _version   = 1;
	private const string _dataBegin = "//--- data begin ---";
	private const string _dataEnd   = "//--- data end ---";

	[NonSerialized] private bool _declarationOnly = false;
	public                  bool IsDeclarationOnly => _declarationOnly;
	
	[OdinSerialize] private ProjectScriptFileHeader _header = null!;
	public                  ProjectScriptFileHeader Header => _header;
	
	[ShowInInspector, OdinSerialize] private string _namespaceName = null!;
	public                                   string NamespaceName => _namespaceName;
	
	[ShowInInspector, OdinSerialize] private string _className = null!;
	public                                   string ClassName => _className;
	
	[ShowInInspector, OdinSerialize] private List<DefinitionTemplateField?>          _fields = new();
	public                                   IReadOnlyList<DefinitionTemplateField?> Fields => _fields;

	private DefinitionTemplate()
	{
	}

	public DefinitionTemplate(string namespaceName, string className)
	{
		_header        = new ProjectScriptFileHeader(Guid.NewGuid().ToString(), _typeId, _version);
		_namespaceName = namespaceName;
		_className     = className;
	}

	public static DefinitionTemplate? CreateFromFile(string absolutePath, ProjectModel projectModel, bool declarationOnly)
	{
		DefinitionTemplate template = new();
		return template.ReadFromFile(absolutePath, projectModel, declarationOnly) ? template : null;
	}

	#region Validation
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
	#endregion
	
	public DefinitionTemplate CreateCopy(ProjectModel projectModel)
	{
		byte[] bytes;
		{
			using MemoryStream memoryStream = new();
			using StreamWriter writer       = new(memoryStream);
			WriteToStream(writer, _header.Guid, projectModel);
			writer.Flush();
			bytes = memoryStream.ToArray();
		}

		DefinitionTemplate result = new();
		{
			using MemoryStream memoryStream = new(bytes);
			using StreamReader reader       = new(memoryStream);
			result.ReadFromStream(reader, projectModel, false);
		}

		return result;
	}

	#region Write
	public void WriteToFile(string absolutePath, string? thisNodeGuid, ProjectModel projectModel)
	{
		using StreamWriter stream = File.CreateText(absolutePath);
		WriteToStream(stream, thisNodeGuid, projectModel);
	}
	
	public void WriteToStream(StreamWriter stream, string? thisNodeGuid, ProjectModel projectModel)
	{
		// header
		stream.WriteLine(_header.ToHeaderString());
		stream.WriteLine("//THIS FILE WAS GENERATED! DON'T MODIFY IT. ONLY THE NAMESPACE AND THE CLASS NAME CAN BE MODIFIED SAFELY!");

		// using
		stream.WriteLine("using Mine.Framework;");
		
		// namespace
		if (!string.IsNullOrEmpty(_namespaceName))
		{
			stream.WriteLine($"namespace {_namespaceName};");
		}

		// class
		stream.WriteLine("[Serializable]");
		stream.WriteLine($"public sealed class {_className} : Definition");
		stream.WriteLine("{");
		
		// fields
		{
			stream.WriteLine("	" + _dataBegin);
			foreach (DefinitionTemplateField? field in _fields)
			{
				if (field == null)
				{
					continue;
				}

				if (field.Type.HasGenericParameter())
				{
					string genericParameter = "";
					if (field.GenericParameter.Guid == thisNodeGuid)
					{
						genericParameter = $"{_namespaceName}.{_className}"; // self-reference
					}
					else
					{
						ProjectNode? scriptNode = projectModel.FindNodeByGuid(field.GenericParameter.Guid ?? "");
						if (scriptNode != null && scriptNode.Type == ProjectNodeType.ScriptDefinition)
						{
							// TODO - translate generic parameter to class name
							NodeIOScriptDefinition? io = scriptNode.GetNodeIO<NodeIOScriptDefinition>();
							if (io != null)
							{
								io.Load();
								if (io.Template != null)
								{
									genericParameter = $"{io.Template._namespaceName}.{io.Template._className}";
								}
							}
						}
					}

					stream.WriteLine($"	public {field.Type.ToCSharpType()}<{genericParameter}> {field.Name};");
				}
				else
				{
					stream.WriteLine($"	public {field.Type.ToCSharpType()} {field.Name};");
				}
			}

			stream.WriteLine("	" + _dataEnd);
		}
		
		// methods

		// static Create()
		{
			stream.WriteLine($"	public static List<{_className}>? Create(string path)");
			stream.WriteLine("	{");
			stream.WriteLine($"		return Definition.Create<{_className}>(path);");
			stream.WriteLine("	}");
		}

		// static CreateDefinitionsRecursive()
		{
			stream.WriteLine($"	public static List<{_className}>? CreateDefinitionsRecursive(string path)");
			stream.WriteLine("	{");
			stream.WriteLine($"		List<{_className}>? result = Definition.Create<{_className}>(path);");
			stream.WriteLine("		if (result is null) return null;");
			stream.WriteLine("		foreach(var definition in result)");
			stream.WriteLine("		{");
			WriteLoadDefinitionsRecursive(stream, "			definition.");
			stream.WriteLine("		}");
			stream.WriteLine("		return result;");
			stream.WriteLine("	}");
		}

		// static CreateAllRecursive()
		{
			stream.WriteLine($"	public static List<{_className}>? CreateAllRecursive(string path)");
			stream.WriteLine("	{");
			stream.WriteLine($"		List<{_className}>? result = Definition.Create<{_className}>(path);");
			stream.WriteLine("		if (result is null) return null;");
			stream.WriteLine("		foreach(var definition in result)");
			stream.WriteLine("		{");
			WriteLoadAllRecursive(stream, "			definition.");
			stream.WriteLine("		}");
			stream.WriteLine("		return result;");
			stream.WriteLine("	}");
		}
		
		// LoadDefinitionsRecursive()
		{
			stream.WriteLine("	public override void LoadDefinitionsRecursive()");
			stream.WriteLine("	{");
			WriteLoadDefinitionsRecursive(stream, "		");
			stream.WriteLine("	}");
		}

		// LoadAllRecursive()
		{
			stream.WriteLine("	public override void LoadAllRecursive()");
			stream.WriteLine("	{");
			WriteLoadAllRecursive(stream, "		");
			stream.WriteLine("	}");
		}
		
		stream.WriteLine("}");
	}

	private void WriteLoadDefinitionsRecursive(StreamWriter stream, string prefix)
	{
		foreach (DefinitionTemplateField? field in _fields)
		{
			if (field == null)
			{
				continue;
			}

			if (field.Type == DefinitionTemplateFieldType.Type_DefinitionReference)
			{
				stream.WriteLine($"{prefix}{field.Name}.LoadDefinitionsRecursive();");
			}
		}
	}

	private void WriteLoadAllRecursive(StreamWriter stream, string prefix)
	{
		foreach (DefinitionTemplateField? field in _fields)
		{
			if (field == null)
			{
				continue;
			}

			if (field.Type == DefinitionTemplateFieldType.Type_DefinitionReference)
			{
				stream.WriteLine($"{prefix}{field.Name}.LoadAllRecursive();");
				continue;
			}

			if (field.Type.IsReference())
			{
				stream.WriteLine($"{prefix}{field.Name}.Load();");
			}
		}
	}
	#endregion

	#region Read
	public bool ReadFromFile(string absolutePath, ProjectModel projectModel, bool declarationOnly)
	{
		using StreamReader stream = File.OpenText(absolutePath);
		return ReadFromStream(stream, projectModel, declarationOnly);
	}

	public bool ReadFromStream(StreamReader stream, ProjectModel projectModel, bool declarationOnly)
	{
		_declarationOnly = declarationOnly;

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
		Regex genericPropertyRegex  = new (@"public\s+(\w+)<([\w\.]+)>\s+(\w+)", RegexOptions.Compiled);
		
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
				if (propertyMatch.Success)
				{
					// type without generic parameter
					string propertyType = propertyMatch.Groups[1].Captures[0].ToString();
					string propertyName = propertyMatch.Groups[2].Captures[0].ToString();

					DefinitionTemplateField field = new(propertyType.ToTemplateType(), propertyName, new StudioScriptDefinitionReference());
					_fields.Add(field);
				}
				else if((propertyMatch = genericPropertyRegex.Match(line)).Success)
				{
					// type with generic parameter
					string propertyType     = propertyMatch.Groups[1].Captures[0].ToString();
					string genericParameter = propertyMatch.Groups[2].Captures[0].ToString();
					string propertyName     = propertyMatch.Groups[3].Captures[0].ToString();

					// generic parameter class name to GUID
					ProjectNode? templateNode = projectModel.FindNode(
						node => {
							if (node.Type != ProjectNodeType.ScriptDefinition)
							{
								return false;
							}

							NodeIOScriptDefinition? io = node.GetNodeIO<NodeIOScriptDefinition>();
							if (io is null || io.Template is null)
							{
								return false;
							}

							return $"{io.Template.NamespaceName}.{io.Template.ClassName}" == genericParameter;
						},
						false, true
					);
					
					// create field
					DefinitionTemplateField field = new(
						propertyType.ToTemplateType(),
						propertyName,
						templateNode is not null && templateNode.Meta?.Guid is not null ? new StudioScriptDefinitionReference(templateNode.Meta!.Guid) : new StudioScriptDefinitionReference()
					);
					_fields.Add(field);
				}
			}
			else
			{
				// parsing class/namespace/marks
				if (line.Contains(_dataBegin))
				{
					if (declarationOnly)
					{
						break; // skip the rest
					}

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
	#endregion
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