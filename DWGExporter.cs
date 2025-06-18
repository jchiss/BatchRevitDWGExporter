using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitAutomationCore
{
    public class DWGExporter
    {
        public void ExportDWGs(Document doc, string exportBaseDir, string buildingNumber)
        {
            DWGExportOptions dwgOptions = new DWGExportOptions
            {
                FileVersion = ACADVersion.R2018,
                SharedCoords = true,
                ExportOfSolids = SolidGeometry.ACIS
            };

            // **Step 1: Get All Floor Plans & Apply Filters**
            IEnumerable<ViewPlan> floorPlans = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewPlan))
                .Cast<ViewPlan>()
                .Where(view => view.ViewType == ViewType.FloorPlan)
                .Where(view => view.CanBePrinted) // ✅ Ensures only printable/exportable views are selected
                .Where(view => !ContainsExcludedKeyword(view.Name));


            if (!floorPlans.Any()) return; // No valid views—exit silently

            // **Step 2: Set Correct Export Folder Name**
            string exportDir = Path.Combine(exportBaseDir, $"{buildingNumber}_Floorplans_Revit");
            Directory.CreateDirectory(exportDir);

            // **Step 3: Export Each Floor Plan**
            foreach (ViewPlan floorPlan in floorPlans)
            {
                string floorLevel = GetFloorLevelFromViewName(floorPlan.Name);
                string dwgExportPath = Path.Combine(exportDir, $"{buildingNumber}_{floorLevel}_Revit.dwg");

                bool exportSuccess = doc.Export(exportDir, $"{buildingNumber}_{floorLevel}_Revit", new List<ElementId> { floorPlan.Id }, dwgOptions);

                File.AppendAllText(@"C:\Temp\DWGExportLog.txt",
                    exportSuccess ? $"Exported: {dwgExportPath}\n" : $"Failed: {dwgExportPath}\n");
            }
        }

        private bool ContainsExcludedKeyword(string viewName)
        {
            string lowerView = viewName.ToLower();
            return lowerView.Contains("3d") ||
                   lowerView.Contains("roof") ||
                   lowerView.Contains("demo") ||
                   lowerView.Contains("demolition") ||
                   lowerView.Contains("remo") ||
                   lowerView.Contains("remodel") ||
                   lowerView.Contains("construction") ||
                   lowerView.Contains("site"); // Exclude views with 'site'
        }

        private string GetFloorLevelFromViewName(string viewName)
        {
            string lowerView = viewName.ToLower();

            // Map word-based numbers to digits
            var wordToNumber = new Dictionary<string, string>
            {
                { "first", "1" }, { "second", "2" }, { "third", "3" }, { "fourth", "4" },
                { "fifth", "5" }, { "sixth", "6" }, { "seventh", "7" }, { "eighth", "8" },
                { "ninth", "9" }, { "tenth", "10" }, { "eleventh", "11" }, { "twelfth", "12" }
                // Add more as needed
            };

            if (lowerView.Contains("basement")) return "Basement";

            // Check for word-based floor numbers
            foreach (var kvp in wordToNumber)
            {
                if (lowerView.Contains(kvp.Key))
                    return $"{kvp.Value}_Floor";
            }

            // Check for numeric ordinals (e.g., 5th, 6th)
            var match = System.Text.RegularExpressions.Regex.Match(lowerView, @"\b(\d+)(st|nd|rd|th)\b");
            if (match.Success)
                return $"{match.Groups[1].Value}_Floor";

            // Check for plain numbers
            string[] words = viewName.Split(new[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in words)
            {
                if (int.TryParse(word, out int floorNumber))
                {
                    return $"{floorNumber}_Floor";
                }
            }

            // Fallback: log and use sanitized view name
            string sanitized = string.Join("_", viewName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Replace(' ', '_');
            File.AppendAllText(@"C:\Temp\DWGExportLog.txt", $"Unknown floor level for view: {viewName}, using fallback: {sanitized}\n");
            return sanitized;
        }
    }
}
