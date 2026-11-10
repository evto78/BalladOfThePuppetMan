using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using static PlayerSkill;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class SpreadsheetToScriptableObject
{
    private static string enemyDataCSVPath = "/Editor/CSVs/Enemies.csv", attackPatternsDataCSVPath = "/Editor/CSVs/Attack Patterns.csv", skillsCSVPath = "/Editor/CSVs/Player Skills.csv", enemyEncountersDataCSVPath = "/Editor/CSVs/Enemy Encounters.csv", itemDataCSVPath, statusEffectsDataCSVPath = "/Editor/CSVs/Status Effects.csv"; //String paths for CSV files
    private static string spreadsheetPath = "/Editor/Ballad of the Puppet Man Data.xlsx", csvFolderPath = "/Editor/CSVs/";
    private const string dataListPath = "Assets/Scripts/Spreadsheet Importing/DataList.asset"; //String path for the data list housing all created assets

    //Function that returns the data list from said path if found
    private static DataList GetDataList()
    {
        DataList dl = AssetDatabase.LoadAssetAtPath<DataList>(dataListPath);
        if (dl == null)
            Debug.LogError("DataList.asset not found!");
        return dl;
    }

    private static string[] ParseCSVLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder currentField = new StringBuilder();

        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char character = line[i];

            if (character == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }
            }
            else if (character == ',' && !insideQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(character);
            }
        }

        fields.Add(currentField.ToString());

        return fields.ToArray();
    }

    public static void GeneratePlayerSkills()
    {
        DataList dataList = GetDataList(); //Calls for data list refrence

        //Gets each line from the skills CSV file and seperates them into an array
        string[] allLines = File.ReadAllLines(Application.dataPath + skillsCSVPath);

        dataList.allPlayerSkills.Clear(); //Clears previous list of skills

        //Adds logic to delete all previous scriptable objects
        string folderPath = "Assets/Resources/Skills";
        string[] existingAssets = Directory.GetFiles(folderPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string assetPath in existingAssets)
        {
            string unityAssetPath = assetPath.Replace('\\', '/');

            if (!AssetDatabase.DeleteAsset(unityAssetPath))
            {
                Debug.LogError("Failed to delete asset: " + unityAssetPath);
                return;
            }
        }

        //Loop that skips the first line of the CSV file (since they are headers) and creates a new scriptable object per line in the CSV
        for (int i = 1; i < allLines.Length; i++)
        {
            //Splits the data in the line by the "," divider
            string[] splitData = ParseCSVLine(allLines[i]);

            //Creates instance of the skills scriptable object and sets the data from the CSV into the it using the split data
            PlayerSkill skill = ScriptableObject.CreateInstance<PlayerSkill>();
            skill.id = int.Parse(splitData[0]);
            skill.skillName = splitData[1];
            skill.elementalType = Enum.Parse<ElementalTypes>(splitData[2]);
            skill.cost = int.Parse(splitData[3]);
            skill.prepTime = float.Parse(splitData[4]);
            skill.target = Enum.Parse<TargetType>(splitData[5]);
            skill.intensity = int.Parse(splitData[6]);
            skill.perfectMultiplier = float.Parse(splitData[7]);
            skill.perfectable = bool.Parse(splitData[8]);

            skill.skillDescription = splitData[9];

            //Set skill file name
            string fileName = $"{skill.id} - {skill.skillName}";

            //Creates the skill scriptable object as a new assets inside of its respective folder
            AssetDatabase.CreateAsset(skill, $"Assets/Resources/Skills/{fileName}.asset");
            dataList.allPlayerSkills.Add(skill); //Adds the current skill to the skill list in the data list
        }
        //Saves the assets
        EditorUtility.SetDirty(dataList); //Overwrites the data list
        AssetDatabase.SaveAssets();
    }

    public static void GenerateEnemies()
    {
        DataList dataList = GetDataList();

        string[] allEnemyLines = File.ReadAllLines(Application.dataPath + enemyDataCSVPath);
        string[] allAttackPatternLines = File.ReadAllLines(Application.dataPath + attackPatternsDataCSVPath);

        dataList.allEnemies.Clear();

        string folderPath = "Assets/Resources/Enemies";
        string[] existingAssets = Directory.GetFiles(folderPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string assetPath in existingAssets)
        {
            string unityAssetPath = assetPath.Replace('\\', '/');

            if (!AssetDatabase.DeleteAsset(unityAssetPath))
            {
                Debug.LogError("Failed to delete asset: " + unityAssetPath);
                return;
            }
        }

        for (int i = 1; i < allEnemyLines.Length; i++)
        {
            string[] enemySplitData = ParseCSVLine(allEnemyLines[i]);

            EnemyData enemy = ScriptableObject.CreateInstance<EnemyData>();
            enemy.id = int.Parse(enemySplitData[0]);
            enemy.enemyName = enemySplitData[1];
            enemy.type = Enum.Parse<ElementalTypes>(enemySplitData[2]);
            enemy.maxHP = int.Parse(enemySplitData[3]);
            enemy.enemyDescription = enemySplitData[5];

            string[] patternByRoundSplitData = enemySplitData[4].Split('-');
            for (int j = 0; j < patternByRoundSplitData.Length; j++)
            {
                enemy.patternByRound.Add(int.Parse(patternByRoundSplitData[j]));
            }

            int previousPatternID = -1;
            Enemy.AttackPattern previousPatten = null;

            for (int j = 1; j < allAttackPatternLines.Length; j++)
            {
                string[] attackPatternSplitData = ParseCSVLine(allAttackPatternLines[j]);

                if (int.Parse(attackPatternSplitData[0]) == enemy.id)
                {

                    if (previousPatternID != int.Parse(attackPatternSplitData[1]))
                    {
                        Enemy.AttackPattern newPattern = new Enemy.AttackPattern();
                        newPattern.timelineChangePriority = int.Parse(attackPatternSplitData[2]);
                        newPattern.timelineCombatTimePref = float.Parse(attackPatternSplitData[3]);
                        newPattern.timelineCombatSpeedPref = float.Parse(attackPatternSplitData[4]);

                        Enemy.Action newAction = new Enemy.Action();
                        newAction.actionType = Enum.Parse<TimelineManager.EventType>(attackPatternSplitData[5]);
                        newAction.damage = int.Parse(attackPatternSplitData[6]);
                        newAction.delayBeforeAction = float.Parse(attackPatternSplitData[7]);
                        newAction.delayAfterAction = float.Parse(attackPatternSplitData[8]);
                        newAction.playAnim = bool.Parse(attackPatternSplitData[9]);
                        newAction.animWindupStart = float.Parse(attackPatternSplitData[10]);
                        newAction.animWinddownEnd = float.Parse(attackPatternSplitData[11]);
                        newAction.oneOff = bool.Parse(attackPatternSplitData[12]);

                        if (previousPatten != null)
                        {
                            enemy.attackPatterns.Add(previousPatten);
                        }
                        previousPatten = newPattern;
                        previousPatten.actionSequence.Add(newAction);
                    }
                    else
                    {
                        Enemy.Action newAction = new Enemy.Action();
                        newAction.actionType = Enum.Parse<TimelineManager.EventType>(attackPatternSplitData[5]);
                        newAction.damage = int.Parse(attackPatternSplitData[6]);
                        newAction.delayBeforeAction = float.Parse(attackPatternSplitData[7]);
                        newAction.delayAfterAction = float.Parse(attackPatternSplitData[8]);
                        newAction.playAnim = bool.Parse(attackPatternSplitData[9]);
                        newAction.animWindupStart = float.Parse(attackPatternSplitData[10]);
                        newAction.animWinddownEnd = float.Parse(attackPatternSplitData[11]);
                        newAction.oneOff = bool.Parse(attackPatternSplitData[12]);

                        previousPatten.actionSequence.Add(newAction);
                    }

                    previousPatternID = int.Parse(attackPatternSplitData[1]);
                }
            }
            if (previousPatten != null && !enemy.attackPatterns.Contains(previousPatten))
            {
                enemy.attackPatterns.Add(previousPatten);
            }

            string fileName = $"{enemy.id} - {enemy.enemyName}";
            AssetDatabase.CreateAsset(enemy, $"Assets/Resources/Enemies/{fileName}.asset");
            dataList.allEnemies.Add(enemy);
        }

        EditorUtility.SetDirty(dataList);
        AssetDatabase.SaveAssets();
    }

    public static void GenerateEncounters()
    {
        DataList dataList = GetDataList();

        string[] allLines = File.ReadAllLines(Application.dataPath + enemyEncountersDataCSVPath);

        dataList.allEncounters.Clear();
        dataList.allRandomEncounters.Clear();
        dataList.allScriptedEncounters.Clear();

        string folderPath = "Assets/Resources/Encounters";
        string[] existingAssets = Directory.GetFiles(folderPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string assetPath in existingAssets)
        {
            string unityAssetPath = assetPath.Replace('\\', '/');

            if (!AssetDatabase.DeleteAsset(unityAssetPath))
            {
                Debug.LogError("Failed to delete asset: " + unityAssetPath);
                return;
            }
        }

        for (int i = 1; i < allLines.Length; i++)
        {
            string[] splitData = ParseCSVLine(allLines[i]);

            EncountersData encounter = ScriptableObject.CreateInstance<EncountersData>();
            encounter.encounterID = int.Parse(splitData[0]);
            encounter.encounterType = Enum.Parse<Encounters.EncounterType>(splitData[11]);
            for (int j = 1; j <= 5; j++)
            {
                string enemyName = splitData[j];
                for (int k = 0; k < dataList.allEnemies.Count; k++)
                {
                    if (enemyName == dataList.allEnemies[k].enemyName)
                    {
                        encounter.enemies.Add(dataList.allEnemies[k]);
                        encounter.offset.Add(float.Parse(splitData[j + 5]));
                    }
                }
            }

            string fileName = $"Encounter {encounter.encounterID}";
            dataList.allEncounters.Add(encounter);
            if (encounter.encounterType == Enum.Parse<Encounters.EncounterType>("Regular"))
            {
                dataList.allRandomEncounters.Add(encounter);
                AssetDatabase.CreateAsset(encounter, $"Assets/Resources/Encounters/{fileName}.asset");
            }
            else if (encounter.encounterType == Enum.Parse<Encounters.EncounterType>("Scripted"))
            {
                dataList.allScriptedEncounters.Add(encounter);
                AssetDatabase.CreateAsset(encounter, $"Assets/Resources/Encounters/{fileName} - Scripted.asset");
            }
        }

        EditorUtility.SetDirty(dataList);
        AssetDatabase.SaveAssets();
    }

    public static void GenerateStatusEffects()
    {
        DataList dataList = GetDataList();

        string[] allLines = File.ReadAllLines(Application.dataPath + statusEffectsDataCSVPath);

        dataList.allStatusEffects.Clear();

        string folderPath = "Assets/Resources/StatusEffects";
        string[] existingAssets = Directory.GetFiles(folderPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string assetPath in existingAssets)
        {
            string unityAssetPath = assetPath.Replace('\\', '/');

            if (!AssetDatabase.DeleteAsset(unityAssetPath))
            {
                Debug.LogError("Failed to delete asset: " + unityAssetPath);
                return;
            }
        }

        for (int i = 1; i < allLines.Length; i++)
        {
            string[] splitData = ParseCSVLine(allLines[i]);

            StatusEffect status = ScriptableObject.CreateInstance<StatusEffect>();
            status.id = int.Parse(splitData[0]);
            status.statusName = splitData[1];
            status.statusDescription = splitData[2];

            string fileName = $"{status.id} - {status.statusName}";
            AssetDatabase.CreateAsset(status, $"Assets/Resources/StatusEffects/{fileName}.asset");
            dataList.allStatusEffects.Add(status);
        }
        EditorUtility.SetDirty(dataList);
        AssetDatabase.SaveAssets();
    }

    public static void UpdateCSVs()
    {
        string fullSpreadsheetPath = Application.dataPath + spreadsheetPath;
        string csvFolder = Application.dataPath + csvFolderPath;

        using (FileStream stream = File.OpenRead(fullSpreadsheetPath))
        {
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                List<string> sharedStrings = new List<string>();
                ZipArchiveEntry sharedStringsEntry = archive.GetEntry("xl/sharedStrings.xml");

                if (sharedStringsEntry != null)
                {
                    using (Stream sharedStringsStream = sharedStringsEntry.Open())
                    {
                        XDocument sharedStringsDoc = XDocument.Load(sharedStringsStream);

                        XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                        foreach (XElement stringItem in sharedStringsDoc.Root.Elements(ns + "si"))
                        {
                            string text = string.Concat(
                                stringItem.Descendants(ns + "t").Select(t => t.Value)
                            );

                            sharedStrings.Add(text);
                        }
                    }
                }

                ZipArchiveEntry workbookEntry = archive.GetEntry("xl/workbook.xml");
                ZipArchiveEntry relationshipsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels");
                XDocument relationshipsDoc;

                using (Stream relationshipsStream = relationshipsEntry.Open())
                {
                    relationshipsDoc = XDocument.Load(relationshipsStream);
                }

                XNamespace relNs = "http://schemas.openxmlformats.org/package/2006/relationships";
                XNamespace rNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";

                using (Stream workbookStream = workbookEntry.Open())
                {
                    XDocument workbookDoc = XDocument.Load(workbookStream);
                    XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                    XElement sheets = workbookDoc.Root.Element(ns + "sheets");

                    foreach (XElement sheet in sheets.Elements(ns + "sheet"))
                    {
                        string sheetName = sheet.Attribute("name").Value;
                        if (sheetName.Equals("Design", StringComparison.OrdinalIgnoreCase) || sheetName.Equals("Tools", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string relationshipId = sheet.Attribute(rNs + "id").Value;

                        XElement relationship = relationshipsDoc.Root.Elements(relNs + "Relationship").FirstOrDefault(r => r.Attribute("Id").Value == relationshipId);

                        string target = relationship.Attribute("Target").Value;
                        string worksheetPath = target.StartsWith("/") ? target.TrimStart('/') : "xl/" + target;

                        ZipArchiveEntry worksheetEntry = archive.GetEntry(worksheetPath);

                        using (Stream worksheetStream = worksheetEntry.Open())
                        {
                            XDocument worksheetDoc = XDocument.Load(worksheetStream);
                            XElement sheetData = worksheetDoc.Root.Element(ns + "sheetData");

                            List<string> csvLines = new List<string>();

                            foreach (XElement row in sheetData.Elements(ns + "row"))
                            {
                                string rowNumber = row.Attribute("r")?.Value ?? "?";

                                List<string> rowValues = new List<string>();

                                foreach (XElement cell in row.Elements(ns + "c"))
                                {

                                    string cellAddress = cell.Attribute("r")?.Value ?? "?";

                                    string cellType = cell.Attribute("t")?.Value ?? "n";

                                    string cellValue = cell.Element(ns + "v")?.Value ?? cell.Element(ns + "is")?.Value ?? "";

                                    if (cellType == "s")
                                    {
                                        if (int.TryParse(cellValue, out int stringIndex) && stringIndex >= 0 && stringIndex < sharedStrings.Count)
                                        {
                                            cellValue = sharedStrings[stringIndex];
                                        }
                                    }
                                    else if (cellType == "b")
                                    {
                                        if (cellValue == "1")
                                        {
                                            cellValue = "True";
                                        }
                                        else if (cellValue == "0")
                                        {
                                            cellValue = "False";
                                        }
                                    }

                                    int columnIndex = 0;

                                    foreach (char character in cellAddress)
                                    {
                                        if (!char.IsLetter(character))
                                        {
                                            break;
                                        }

                                        columnIndex = columnIndex * 26
                                            + (char.ToUpperInvariant(character) - 'A' + 1);
                                    }

                                    columnIndex--;

                                    while (rowValues.Count <= columnIndex)
                                    {
                                        rowValues.Add("");
                                    }

                                    rowValues[columnIndex] = cellValue;

                                    
                                }

                                string csvLine = string.Join(",", rowValues.Select(value =>
                                {
                                    if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
                                    {
                                        return "\"" + value.Replace("\"", "\"\"") + "\"";
                                    }

                                    return value;
                                }));

                                csvLines.Add(csvLine);
                            }
                            string csvFileName = sheetName + ".csv";
                            string csvFilePath = csvFolder + csvFileName;

                            bool needsUpdate = !File.Exists(csvFilePath) || !File.ReadAllLines(csvFilePath).SequenceEqual(csvLines);

                            if (needsUpdate)
                            {
                                File.WriteAllLines(csvFilePath, csvLines);
                            }
                        }
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Design Tools/Generate Data")]
    public static void GenerateData()
    {
        UpdateCSVs();
        GenerateStatusEffects();
        GeneratePlayerSkills();
        //Logic to generate items
        GenerateEnemies();
        GenerateEncounters();
    }
}