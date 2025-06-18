using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitAutomationCore
{
    public class RevitFileSearcher
    {
        public List<string> FindRevitFiles(string baseDir, List<string> buildingNumbers)
        {
            List<string> revitFiles = new List<string>();

            foreach (var number in buildingNumbers)
            {
                string formattedNumber = number.PadLeft(4, '0');
                string buildingFolder = Path.Combine(baseDir, formattedNumber);

                if (!Directory.Exists(buildingFolder))
                {
                    File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Skipping nonexistent folder: {buildingFolder}\n");
                    continue; // Skip if the building folder does not exist
                }

                // Get subdirectories inside the building folder
                var subDirectories = Directory.GetDirectories(buildingFolder);

                // Filter: Only include directories that explicitly contain "REVIT"
                var filteredDirectories = subDirectories.Where(dir =>
                    dir.ToUpper().Contains("REVIT") && // Must contain "REVIT"
                    !ContainsExcludedKeyword(dir) // Must not contain excluded terms
                ).ToList();

                // Debug: Log excluded and included directories
                foreach (var excludedDir in subDirectories.Except(filteredDirectories))
                {
                    File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Excluded directory: {excludedDir}\n");
                }
                foreach (var validDir in filteredDirectories)
                {
                    File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Searching in directory: {validDir}\n");
                }

                foreach (var subDir in filteredDirectories)
                {
                    string[] files = Directory.GetFiles(subDir, "*MASTER ARCH MODEL.rvt", SearchOption.TopDirectoryOnly);

                    foreach (var file in files)
                    {
                        // Exclude any Revit files with "MEP" in their name
                        if (Path.GetFileName(file).ToUpper().Contains("MEP"))
                        {
                            File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Excluded MEP model: {file}\n");
                            continue; // Skip this file
                        }

                        File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Found valid Revit file: {file}\n");
                        revitFiles.Add(file);
                    }
                }
            }

            return revitFiles;
        }

        public Dictionary<string, string> FindRevitFilesWithBuildingNumbers(string baseDir, List<string> buildingNumbers)
        {
            var result = new Dictionary<string, string>(); // Key: file path, Value: building number

            foreach (var number in buildingNumbers)
            {
                string formattedNumber = number.PadLeft(4, '0');
                string buildingFolder = Path.Combine(baseDir, formattedNumber);

                if (!Directory.Exists(buildingFolder))
                {
                    File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Skipping nonexistent folder: {buildingFolder}\n");
                    continue;
                }

                var subDirectories = Directory.GetDirectories(buildingFolder);
                var filteredDirectories = subDirectories.Where(dir =>
                    dir.ToUpper().Contains("REVIT") &&
                    !ContainsExcludedKeyword(dir)
                ).ToList();

                foreach (var subDir in filteredDirectories)
                {
                    string[] files = Directory.GetFiles(subDir, "*MASTER ARCH MODEL.rvt", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        if (Path.GetFileName(file).ToUpper().Contains("MEP"))
                        {
                            File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Excluded MEP model: {file}\n");
                            continue;
                        }
                        File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Found valid Revit file: {file}\n");
                        result[file] = formattedNumber;
                    }
                }
            }
            return result;
        }

        private bool ContainsExcludedKeyword(string dir)
        {
            string lowerDir = dir.ToLower();
            return lowerDir.Contains("backup") ||
                   lowerDir.Contains("back") ||
                   lowerDir.Contains("archive") ||
                   lowerDir.Contains("support");
        }
    }
}
