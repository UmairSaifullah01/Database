using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.IO;

namespace THEBADDEST.DatabaseModule
{
    [CreateAssetMenu(menuName = "THEBADDEST/Database/SheetTableAdapter", fileName = "SheetTableAdapter")]
    public class SheetTableAdapter : TableAdapter
    {
        [SerializeField, TextArea(5, 20)] public string sheet = "";
        [SerializeField] public string csvFilePath = ""; // Relative to Assets or absolute

        public override void Serialize<T>(ITable<T> table)
        {
            var entries = table.Entries;
            if (entries == null || entries.Count == 0)
            {
                sheet = string.Empty;
                return;
            }

            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite).ToArray();

            var sb = new StringBuilder();
            // Header
            sb.AppendLine(string.Join(",", fields.Select(f => f.Name).Concat(properties.Select(p => p.Name))));
            // Rows
            foreach (var entry in entries)
            {
                var values = fields.Select(f => EscapeCsv(f.GetValue(entry)))
                    .Concat(properties.Select(p => EscapeCsv(p.GetValue(entry))));
                sb.AppendLine(string.Join(",", values));
            }
            sheet = sb.ToString();
        }

        public override void Deserialize<T>(ITable<T> table)
        {
            if (string.IsNullOrEmpty(sheet)) return;
            var lines = sheet.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) return; // No data
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite).ToArray();
            var headers = lines[0].Split(',');
            for (int i = 1; i < lines.Length; i++)
            {
                var values = SplitCsvLine(lines[i]);
                var obj = Activator.CreateInstance<T>();
                for (int j = 0; j < headers.Length && j < values.Length; j++)
                {
                    var header = headers[j];
                    var value = values[j];
                    var field = fields.FirstOrDefault(f => f.Name == header);
                    if (field != null)
                    {
                        field.SetValue(obj, Convert.ChangeType(value, field.FieldType));
                        continue;
                    }
                    var prop = properties.FirstOrDefault(p => p.Name == header);
                    if (prop != null)
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                    }
                }
                if (!table.Entries.Contains(obj))
                {
                    table.AddRecord(obj);
                }
            }
        }

        [ContextMenu("Load CSV From File")]
        public void LoadCsvFromFile()
        {
            string path = csvFilePath;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("CSV file path is empty.");
                return;
            }
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Application.dataPath, path);
            }
            if (!File.Exists(path))
            {
                Debug.LogWarning($"CSV file not found at: {path}");
                return;
            }
            sheet = File.ReadAllText(path, Encoding.UTF8);
            Debug.Log($"Loaded CSV from {path}");
        }

        [ContextMenu("Save CSV To File")]
        public void SaveCsvToFile()
        {
            string path = csvFilePath;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("CSV file path is empty.");
                return;
            }
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Application.dataPath, path);
            }
            File.WriteAllText(path, sheet, Encoding.UTF8);
            Debug.Log($"Saved CSV to {path}");
        }

        private static string EscapeCsv(object value)
        {
            if (value == null) return "";
            var str = value.ToString();
            if (str.Contains(",") || str.Contains("\"") || str.Contains("\n") || str.Contains("\r"))
            {
                str = "\"" + str.Replace("\"", "\"\"") + "\"";
            }
            return str;
        }

        private static string[] SplitCsvLine(string line)
        {
            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            var sb = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }
    }
}