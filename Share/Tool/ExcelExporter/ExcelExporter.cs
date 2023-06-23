using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson.Serialization;
using OfficeOpenXml;
using ProtoBuf;

namespace ET
{
    internal class Table
    {
        public string TableName;
        public Dictionary<string, FiledInfo> Fields = new();
        public List<TableData> DataList = new();
    }

    internal struct FiledInfo
    {
        public string FilePath;
        public string FieldDesc;
        public string FieldName;
        public string FieldType;
        public int FieldIndex;
        public int Col;
        public bool IndexKey;
    }

    internal class TableData
    {
        public string FileDir;
        public string FilePath;
        public List<List<(string, string)>> Data = new();
    }

    public static class ExcelExporter
    {
        private static string template;

        private const string ClassDir = "../Unity/Assets/Scripts/Codes/Model/Share/Config/ConfigDefine";

        private const string excelDir = "../Unity/Assets/Config/Excel/";

        private static Assembly configAssembly;

        private static readonly Dictionary<string, Table> tables = new();
        private static readonly Dictionary<string, ExcelPackage> packages = new();

        private static Table GetTable(string tableName)
        {
            if (!tables.TryGetValue(tableName, out var table))
            {
                table = new Table();
                tables[tableName] = table;
            }

            return table;
        }

        public static ExcelPackage GetPackage(string filePath)
        {
            if (!packages.TryGetValue(filePath, out var package))
            {
                using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                package = new ExcelPackage(stream);
                packages[filePath] = package;
            }

            return package;
        }

        public static void Export()
        {
            try
            {
                //防止编译时裁剪掉protobuf
                _ = WireType.Fixed64.ToString();

                template = File.ReadAllText("Template.txt");
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                List<string> files = FileHelper.GetAllFiles(excelDir);
                foreach (string path in files)
                {
                    string fileName = Path.GetFileName(path);
                    if (!fileName.EndsWith(".xlsx") || fileName.StartsWith("~$") || fileName.Contains("#"))
                    {
                        continue;
                    }

                    ExcelPackage p = GetPackage(Path.GetFullPath(path));

                    foreach (ExcelWorksheet worksheet in p.Workbook.Worksheets)
                    {
                        ExportSheetClass(worksheet, path);
                    }
                }

                if (Directory.Exists(ClassDir))
                {
                    Directory.Delete(ClassDir, true);
                }

                foreach (var kv in tables)
                {
                    ExportClass(kv.Value);
                }

                // 动态编译生成的配置代码
                configAssembly = DynamicBuild();

                foreach (var kv in tables)
                {
                    Table table = kv.Value;
                    foreach (TableData tableData in table.DataList)
                    {
                        ExportExcelJson(table.TableName, tableData);
                        ExportExcelProtobuf(table.TableName, tableData);
                    }
                }

                Log.Console("Export Excel Sucess!");
            }
            catch (Exception e)
            {
                Log.Console(e.ToString());
            }
            finally
            {
                tables.Clear();
                foreach (var kv in packages)
                {
                    kv.Value.Dispose();
                }

                packages.Clear();
            }
        }

        // 动态编译生成的cs代码
        private static Assembly DynamicBuild()
        {
            string classPath = ClassDir;
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (string classFile in Directory.GetFiles(classPath, "*.cs"))
            {
                syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(classFile)));
            }

            List<PortableExecutableReference> references = new List<PortableExecutableReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }

                    if (assembly.Location == "")
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                PortableExecutableReference reference = MetadataReference.CreateFromFile(assembly.Location);
                references.Add(reference);
            }

            CSharpCompilation compilation = CSharpCompilation.Create(null,
                syntaxTrees.ToArray(),
                references.ToArray(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using MemoryStream memSteam = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memSteam);
            if (!emitResult.Success)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Diagnostic t in emitResult.Diagnostics)
                {
                    stringBuilder.Append($"{t.GetMessage()}\n");
                }

                throw new Exception($"动态编译失败:\n{stringBuilder}");
            }

            memSteam.Seek(0, SeekOrigin.Begin);

            Assembly ass = Assembly.Load(memSteam.ToArray());
            return ass;
        }

        #region 导出class

        static void ExportSheetClass(ExcelWorksheet worksheet, string filepath)
        {
            if (!worksheet.Name.StartsWith("@"))
            {
                return;
            }

            var fileName = Path.GetFileName(filepath);
            var fileDir = Path.GetDirectoryName(filepath);
            var tableName = worksheet.Name.Substring(1, worksheet.Name.Length - 1) + "Config";

            Table table = GetTable(tableName);
            table.TableName = tableName;
            foreach (var t in table.DataList)
            {
                if (t.FileDir == fileDir)
                {
                    throw new Exception($"{filepath}{t.FilePath}表{tableName}重复");
                }
            }

            TableData tableData = new();
            table.DataList.Add(tableData);
            tableData.FileDir = fileDir;
            tableData.FilePath = filepath;

            const int IgnoreRow = 1; // 忽略标记行
            const int FieldDescRow = 2; // 字段描述行
            const int FieldNameRow = 3; // 字段名行
            const int FieldTypeRow = 4; // 字段类型行
            const int IndexRow = 5; // 索引标记行
            const int DataBeginRow = 6; // 数据开始行

            int filedIndex = 1;
            for (int col = 2; col <= worksheet.Dimension.End.Column; ++col)
            {
                string ignoreFlag = worksheet.Cells[IgnoreRow, col].Text.Trim().ToLower();
                if (ignoreFlag.Length > 0)
                {
                    if (ignoreFlag == "#")
                    {
                        // 忽略列
                        continue;
                    }

                    throw new Exception($"{filepath}表{tableName}第{col}列忽略标记配置错误");
                }

                string fieldName = worksheet.Cells[FieldNameRow, col].Text.Trim();
                if (fieldName == "")
                {
                    throw new Exception($"{filepath}表{tableName}第{col}列字段名配置错误");
                }

                string fieldDesc = worksheet.Cells[FieldDescRow, col].Text.Trim();
                string fieldType = worksheet.Cells[FieldTypeRow, col].Text.Trim();

                bool indexFlag = false;
                string indexStr = worksheet.Cells[IndexRow, col].Text.Trim().ToLower();
                if (indexStr.Length > 0)
                {
                    if (indexStr == "t")
                    {
                        indexFlag = true;
                    }
                    else if (indexStr == "f")
                    {
                        indexFlag = false;
                    }
                    else
                    {
                        Log.Console("xxxxxxxxx " + worksheet.Cells[IndexRow, col].Text);
                        throw new Exception($"{filepath}表{tableName}第{col}列索引标记配置错误{indexStr}");
                    }
                }

                if (table.Fields.TryGetValue(fieldName, out FiledInfo filedInfo))
                {
                    if (filedInfo.FilePath == filepath)
                    {
                        throw new Exception($"{fileName}表{tableName}字段{fieldName}重复");
                    }

                    if (filedInfo.FieldType != fieldType)
                    {
                        throw new Exception($"{filepath} {filedInfo.FilePath}表{tableName}字段{fieldName}重复存在但类型不同");
                    }
                }
                else
                {
                    table.Fields[fieldName] = new FiledInfo
                    {
                        FieldName = fieldName,
                        FilePath = filepath,
                        FieldDesc = fieldDesc,
                        FieldType = fieldType,
                        FieldIndex = filedIndex++,
                        Col = col,
                        IndexKey = indexFlag,
                    };
                }
            }

            if (!table.Fields.ContainsKey("ID"))
            {
                throw new Exception($"{fileName}表{tableName}没有ID字段");
            }

            // 解析数据
            for (int row = DataBeginRow; row <= worksheet.Dimension.End.Row; ++row)
            {
                string ignoreFlag = worksheet.Cells[row, 1].Text.Trim().ToLower();
                if (ignoreFlag.Length > 0)
                {
                    if (ignoreFlag == "#")
                    {
                        // 忽略行
                        continue;
                    }

                    throw new Exception($"{fileName}表{tableName}第{row}行忽略标记配置错误");
                }

                List<FiledInfo> filedList = new(table.Fields.Values);
                filedList.Sort((f1, f2) => f1.Col.CompareTo(f2.Col));

                List<(string, string)> oneData = new();
                foreach (FiledInfo filedInfo in filedList)
                {
                    var data = Convert(filedInfo.FieldType, worksheet.Cells[row, filedInfo.Col].Text.Trim());
                    oneData.Add((filedInfo.FieldName, data));
                }

                tableData.Data.Add(oneData);
            }
        }

        static void ExportClass(Table table)
        {
            string protoName = table.TableName;
            Dictionary<string, FiledInfo> classField = table.Fields;

            string dir = ClassDir;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string exportPath = Path.Combine(dir, $"{protoName}.cs");

            using FileStream txt = new FileStream(exportPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);

            string content = template.Replace("(ConfigName)", protoName);

            {
                StringBuilder sb = new StringBuilder();
                foreach ((string _, FiledInfo filedInfo) in classField)
                {
                    sb.Append($"        /// <summary>{filedInfo.FieldDesc}</summary>\n");
                    sb.Append($"        [ProtoMember({filedInfo.FieldIndex})]\n");
                    sb.Append($"        public {filedInfo.FieldType} {filedInfo.FieldName} {{ get; set; }}\n");
                    sb.Append('\n');
                }

                content = content.Replace("(Fields)", sb.ToString().TrimEnd());
            }

            {
                StringBuilder sb = new StringBuilder();
                foreach ((string _, FiledInfo filedInfo) in classField)
                {
                    if (!filedInfo.IndexKey)
                    {
                        continue;
                    }

                    if (filedInfo.FieldName == "ID")
                    {
                        sb.Append($"        [ProtoIgnore]\n");
                        sb.Append($"        [BsonIgnore]\n");
                        sb.Append($"        private readonly Dictionary<int, {protoName}> dict = new();\n");
                        sb.Append('\n');
                        sb.Append($"        public {protoName} Get(int id)\n");
                        sb.Append("        {\n");
                        sb.Append($"            return this.dict[id];\n");
                        sb.Append("        }\n");
                        sb.Append('\n');
                    }
                    else
                    {
                        sb.Append($"        [ProtoIgnore]\n");
                        sb.Append($"        [BsonIgnore]\n");
                        sb.Append($"        private readonly Dictionary<{filedInfo.FieldType}, {protoName}> dictBy{filedInfo.FieldName} = new();\n");
                        sb.Append('\n');
                        sb.Append($"        public {protoName} GetBy{filedInfo.FieldName}({filedInfo.FieldType} {filedInfo.FieldName})\n");
                        sb.Append("        {\n");
                        sb.Append($"            return this.dictBy{filedInfo.FieldName}[{filedInfo.FieldName}];\n");
                        sb.Append("        }\n");
                        sb.Append('\n');
                    }
                }

                content = content.Replace("(DictOp)", sb.ToString().TrimEnd());
            }

            {
                StringBuilder sb = new StringBuilder();
                sb.Append("            foreach (var config in list)\n");
                sb.Append("            {\n");
                sb.Append("                config.AfterEndInit();\n");
                foreach ((string _, FiledInfo filedInfo) in classField)
                {
                    if (!filedInfo.IndexKey)
                    {
                        continue;
                    }

                    if (filedInfo.FieldName == "ID")
                    {
                        sb.Append($"                this.dict.Add(config.ID, config);\n");
                    }
                    else
                    {
                        sb.Append($"                this.dictBy{filedInfo.FieldName}.Add(config.{filedInfo.FieldName}, config);\n");
                    }
                }
                sb.Append("            }\n");

                content = content.Replace("(DictInit)", sb.ToString().TrimEnd());
            }

            sw.Write(content);
        }

        #endregion

        #region 导出json

        static void ExportExcelJson(string tableName, TableData tableData)
        {
            string relativeDir = Path.GetRelativePath(excelDir, Path.GetDirectoryName(tableData.FilePath));
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"list\":[\n");

            int i = 0;
            foreach (var oneData in tableData.Data)
            {
                sb.Append("\t{");
                sb.Append($"\"_t\":\"{tableName}\"");
                foreach ((string fieldName, object data) in oneData)
                {
                    if (fieldName == "ID")
                    {
                        sb.Append($",\"_id\":{data}");
                    }

                    sb.Append($",\"{fieldName}\":{data}");
                }

                sb.Append('}');

                if (i++ != tableData.Data.Count - 1)
                {
                    sb.Append(',');
                }

                sb.Append('\n');
            }

            sb.Append("]}\n");
            string dir = $"../Config/Json/{relativeDir}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string jsonPath = Path.Combine(dir, $"{tableName}.json");
            using FileStream txt = new FileStream(jsonPath, FileMode.Create);
            using StreamWriter sw = new StreamWriter(txt);
            sw.Write(sb.ToString());
        }

        private static string Convert(string type, string value)
        {
            switch (type)
            {
                case "uint[]":
                case "int[]":
                case "int32[]":
                case "long[]":
                    return $"[{value}]";
                case "string[]":
                case "int[][]":
                    return $"[{value}]";
                case "int":
                case "uint":
                case "int32":
                case "int64":
                case "long":
                case "float":
                case "double":
                    if (value == "")
                    {
                        return "0";
                    }

                    return value;
                case "string":
                    value = value.Replace("\\", "\\\\");
                    value = value.Replace("\"", "\\\"");
                    return $"\"{value}\"";
                default:
                    throw new Exception($"不支持此类型: {type}");
            }
        }

        #endregion

        // 根据生成的类，把json转成protobuf
        private static void ExportExcelProtobuf(string tableName, TableData tableData)
        {
            string relativeDir = Path.GetRelativePath(excelDir, Path.GetDirectoryName(tableData.FilePath));
            string dir = $"../Config/Excel/{relativeDir}";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Assembly ass = configAssembly;
            Type type = ass.GetType($"ET.{tableName}Category");
            Type subType = ass.GetType($"ET.{tableName}");

            Serializer.NonGeneric.PrepareSerializer(type);
            Serializer.NonGeneric.PrepareSerializer(subType);

            IMerge final = Activator.CreateInstance(type) as IMerge;

            string p = $"../Config/Json/{relativeDir}";
            string[] ss = Directory.GetFiles(p, $"{tableName}*.json");
            List<string> jsonPaths = ss.ToList();

            jsonPaths.Sort();
            jsonPaths.Reverse();
            foreach (string jsonPath in jsonPaths)
            {
                string json = File.ReadAllText(jsonPath);
                object deserialize = BsonSerializer.Deserialize(json, type);
                final.Merge(deserialize);
            }

            string path = Path.Combine(dir, $"{tableName}Category.bytes");

            using FileStream file = File.Create(path);
            Serializer.Serialize(file, final);
        }
    }
}