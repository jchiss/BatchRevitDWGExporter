using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;

namespace RevitAutomationCore
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class BatchExportCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application; // Correctly get UIApplication

            // Debug: Confirm command execution starts
            File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", "BatchExportCommand started.\n");
            TaskDialog.Show("Debug", "BatchExportCommand started.");


            string baseDir = @"L:\ACAD\PPARCENG\";                                         //CHANGE THIS PATH TO MATCH YOUR DIRECTORY STRUCTURE
            string excelPath = @"C:\Users\jchiss\Documents\TargetBuildings.xlsx";           //CHANGE THIS PATH TO FIND YOUR EXCEL FILE

            // Extract building numbers from Excel
            var excelReader = new ExcelReader();
            var buildingNumbers = excelReader.ExtractBuildingNumbers(excelPath);
            foreach (var number in buildingNumbers)
            {
                File.AppendAllText(@"C:\Temp\RevitDebugLog.txt", $"Formatted building number: {number}\n");
            }


            // Search for matching Revit files and their building numbers
            var fileSearcher = new RevitFileSearcher();
            var revitFilesWithNumbers = fileSearcher.FindRevitFilesWithBuildingNumbers(baseDir, buildingNumbers);

            TaskDialog.Show("Batch Export", $"Total Revit files found: {revitFilesWithNumbers.Count}");

            if (revitFilesWithNumbers.Count == 0)
            {
                TaskDialog.Show("Batch Export", "No matching Revit files found. Verify building numbers and directory paths.");
                return Result.Failed;
            }

            var batchExport = new BatchExport();
            foreach (var kvp in revitFilesWithNumbers)
            {
                string revitFile = kvp.Key;
                string buildingNumber = kvp.Value;
                Console.WriteLine($"Processing file: {revitFile}");
                batchExport.OpenAsLocalCopyAndExportDWG(revitFile, uiApp, buildingNumber);
            }

            return Result.Succeeded;
        }
    }
}
