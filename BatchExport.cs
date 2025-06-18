using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;

namespace RevitAutomationCore
{
    public class BatchExport
    {
        public void OpenAsLocalCopyAndExportDWG(string centralFilePath, UIApplication uiApp, string buildingNumber)
        {
            // Ensure the central model path is valid
            if (!File.Exists(centralFilePath))
            {
                TaskDialog.Show("Error", $"Central model not found: {centralFilePath}");
                return;
            }

            // Convert file path to Revit ModelPath
            ModelPath centralModelPath = ModelPathUtils.ConvertUserVisiblePathToModelPath(centralFilePath);

            // **Detach Central Model**
            OpenOptions openOptions = new OpenOptions
            {
                DetachFromCentralOption = DetachFromCentralOption.DetachAndDiscardWorksets // Fully detach from worksharing
            };

            Document doc = uiApp.Application.OpenDocumentFile(centralModelPath, openOptions);

            // **Define Local Copy Path**
            string localDir = @"C:\Temp\RevitLocals\"; //CHANGE THIS PATH TO WHERE YOU WANT THE REVIT LOCAL COPIES TO BE STORED
            Directory.CreateDirectory(localDir);
            string localCopyPath = Path.Combine(localDir, $"{buildingNumber}_LocalCopy.rvt");

            SaveAsOptions saveOptions = new SaveAsOptions();
            doc.SaveAs(localCopyPath, saveOptions);

            // **Trigger DWG Export**
            DWGExporter dwgExporter = new DWGExporter();
            dwgExporter.ExportDWGs(doc, @"C:\RevitExports", buildingNumber);

            // **Close the document after processing**
            doc.Close(false);
        }
    }
}
