using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class KspModsLocalizationHelper : MonoBehaviour
{
    #region Fields
    public static string TechTreeNodePrefix = "#RP0TT_LOC_";
    public static string ContractTypePrefix = "#RP0CT_LOC_";
    public static string ContractParamMidfix = "_PARAM_";
    public static string TitlePosfix = "_Title";
    public static string DescriptionPosfix = "_Desc";
    public static string SynopsisPosfix = "_Syno";
    public static string CompletedPosfix = "_Comp";

    [SerializeField] private string orgFile;
    [SerializeField] private string curCN;
    [SerializeField] private string contractFolder;
    [SerializeField] private string outputFolder;

    private Dictionary<string, BaseLocalization> techTreeLocalizations = new();
    #endregion

    #region Tech Tree
    private void ProcessTechTree()
    {
        techTreeLocalizations.Clear();
        ReadTechTree();
        ReadChinese();

        WriteLinesToFile("RP0TechTree", outputFolder, ConstructTecTreeLines());
        WriteLinesToFile("en-us", outputFolder, ConstructTranslation("en-us"));
        WriteLinesToFile("zh-cn", outputFolder, ConstructTranslation("zh-cn"));

        Debug.Log("Tech Tree Node Count: " + techTreeLocalizations.Count);
    }

    private void ReadTechTree()
    {
        if (!File.Exists(orgFile))
        {
            Debug.LogError("There is no original file!");
            return;
        }

        var lines = File.ReadAllLines(orgFile);
        for (int i = 0; i < lines.Length; i++)
            if(lines[i].Trim() == "RDNode")
            {
                string id = "No id";
                string enTitle = "No Title";
                string enDesc = "No Desc";

                for (int j = i; j < lines.Length; j++)
                {
                    //if (lines[j].ToLower().Contains("id"))
                    //{
                    //    var split = lines[j].Split('=');
                    //    if (split.Length > 1)
                    //    {
                    //        id = split[1].Trim();
                    //        header = "    @RDNode:HAS[#id[" + id + "]]";
                    //        titleLine = "        @title = " + TechTreeNodePrefix + id;
                    //        descLine = "        @description = " + TechTreeNodePrefix + id + DescriptionPosfix;
                    //    }
                    //}

                    if (lines[j].ToLower().Contains("parent"))
                        continue;

                    // reading english contents
                    id = ReadConctent(lines[j], "id =", id);
                    enTitle = ReadConctent(lines[j], "title", enTitle);
                    enDesc = ReadConctent(lines[j], "description", enDesc);

                    // reading node ends here
                    if (lines[j].Contains('}'))
                    {
                        var local = new BaseLocalization();
                        local.title = new Localization(enTitle, enTitle);
                        local.description = new Localization(enDesc, enDesc);

                        techTreeLocalizations[id] = local;

                        //Debug.Log(header + " | " + titleLine + " | " + descLine);
                        //Debug.Log(enTitle + " | " + enDesc);
                        break;
                    }
                }
            }

        //Debug.Log("File has " + lines.Length + " lines");
    }
    private string[] ConstructTecTreeLines()
    {
        List<string> lines = new();

        lines.Add("@TechTree:FINAL"); // change tech tree header
        lines.Add("{");  // start marker

        // loop through all nodes data
        foreach ((string id, BaseLocalization local) in techTreeLocalizations)
        {
            var header = "    @RDNode:HAS[#id[" + id + "]]";
            local.title.key = TechTreeNodePrefix + id + TitlePosfix;
            var titleLine = "        @title = " + local.title.key;
            local.description.key = TechTreeNodePrefix + id + DescriptionPosfix;
            var descLine = "        @description = " + local.description.key;

            // construct single node
            lines.Add(header);
            lines.Add("    {");
            lines.Add(titleLine);
            lines.Add(descLine);
            lines.Add("    }");
        }

        lines.Add("}");  // end marker
        return lines.ToArray();
    }
    #endregion

    #region Core Functions
    private static string ReadConctent(string line, string key, string result)
    {
        if (line.ToLower().Contains(key))
        {
            var split = line.Split('=');
            if (split.Length > 1)
                result = split[1].Trim();
        }

        return result;
    }

    private void ReadChinese()
    {
        if (!File.Exists(curCN))
        {
            Debug.LogError("There is no cn translated file!");
            return;
        }

        HashSet<string> cnIds = new();
        var lines = File.ReadAllLines(curCN);
        for (int i = 0; i < lines.Length; i++)
            if (lines[i].Contains("#id"))
            {
                var id = "No id";
                var cnTitle = "No Title";
                var cnDesc = "No Desc";

                var idSplit = lines[i].Split("#id[");
                if (idSplit.Length > 1)
                {
                    id = idSplit[1];
                    id = id.Replace("]", "");
                }

                for (int j = i; j < lines.Length; j++)
                {
                    cnTitle = ReadConctent(lines[j], "title", cnTitle);
                    cnDesc = ReadConctent(lines[j], "description", cnDesc);

                    // reading ends
                    if (lines[j].Contains('}'))
                    {
                        cnIds.Add(id);
                        if (techTreeLocalizations.TryGetValue(id, out var data))
                        {
                            data.title.cn = cnTitle;
                            data.description.cn = cnDesc;
                        }
                        else
                            Debug.LogError($"ID[{id}] is in the chinese file but not in original tech tree file!");
                        break;
                    }
                }
            }

        // check if there is any ids in tech tree but has no cn translation
        foreach (var key in techTreeLocalizations.Keys)
        {
            if (!cnIds.Contains(key))
                Debug.LogError($"The key[{key}] is in tech tree nodes but has no chinese translation!");
        }
    }

    private string[] ConstructTranslation(string language)
    {
        bool isCN = language.Contains("cn");

        List<string> lines = new();

        lines.Add("Localization");
        lines.Add("{");

        lines.Add("    RP0conf = True");
        lines.Add($"    {language}");
        lines.Add("    {"); // translation start

        foreach ((string id, BaseLocalization local) in techTreeLocalizations)
        {
            var titleLine = "        " + local.title.key + " = " + (isCN ? local.title.cn : local.title.en);
            var descLine = "        " + local.description.key + " = " + (isCN ? local.description.cn : local.description.en);

            lines.Add(titleLine);
            lines.Add(descLine);
            lines.Add(" ");
        }

        lines.Add("    }"); // translation end
        lines.Add("}");
        return lines.ToArray();
    }

    private void WriteLinesToFile(string fileName, string path, string[] lines)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError("There is no output folder!");
            return;
        }

        var filePath = Path.Combine(path, fileName + ".txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }
    }
    #endregion

    #region Inspector
    public void DrawInspector()
    {
        orgFile = DrawFilePath("Original File: ", orgFile);
        curCN = DrawFilePath("Current CN File: ", curCN);

        outputFolder = DrawFilePath("Output Folder: ", outputFolder, true);

        if(GUILayout.Button("Read Tech Tree",GUILayout.Height(40)))
        {
            ProcessTechTree();
        }

        contractFolder = DrawFilePath("Contracts Folder: ", contractFolder, true);

    }

    private string DrawFilePath(string fieldName, string path, bool isFolder = false)
    {
        bool hasPath = !string.IsNullOrEmpty(path);
        GUILayout.BeginHorizontal();
        GUILayout.Label(fieldName + (hasPath ? path : "None"));
        if (GUILayout.Button("Open", GUILayout.Width(80)))
        {
            if (isFolder)
                path = EditorUtility.OpenFolderPanel("Select the " + fieldName, hasPath ? path : "Assets", "localization.txt");
            else
                path = EditorUtility.OpenFilePanel("Select the " + fieldName, hasPath ? path : "Assets", "txt");
        }
        GUILayout.EndHorizontal();

        return path;
    }
    #endregion
}
public class BaseLocalization
{
    public Localization title;
    public Localization description;
}

public class ContractLocalization: BaseLocalization
{
    public Localization synopsis;
    public Localization complete;

    public List<Localization> parameters = new();
}
public class Localization
{
    public Localization(string e, string c)
    {
        en = e;
        cn = c;
    }

    public string key;
    public string en;
    public string cn;
}

[CustomEditor(typeof(KspModsLocalizationHelper))]
public class KspModsLocalizationHelperEditor : Editor
{
    private KspModsLocalizationHelper tgt;

    private void OnEnable()
    {
        tgt = (KspModsLocalizationHelper)target;
    }

    public override void OnInspectorGUI()
    {
        tgt.DrawInspector();
        //base.OnInspectorGUI();
    }
}
