# Batch Revit DWG Exporter Add-In

This project creates a custom Revit add-in used to batch export DWG floorplans from many Revit files located in a directory.

## Features

- Reads an Excel file containing a list of building names to target  
- Creates saved local `.rvt` copies of each targeted building from collaborative central models located on network drives, without modifying the central models  
- Exports DWGs from each building to:  
  `RevitExports/Subfolder(with building name)/your_custom_file_name.dwg`  
- Paths and naming conventions should be changed for the user's application and are clearly labeled in the code
  - The path to store .rvt local copies needs to be changed in BatchExport.cs
  - The path for the folder "RevitExports" containing subfolders with dwgs needs to be changed in BatchExport.cs
  - The paths for the excel file and directory searching logic need to be changed in BatchExportCommand.cs
- Only certain DWGs are exported: currently, only floorplans are included  
  - Plans with "3D", "Construction", "Remodel", "Demolition", "Site", "Roof", or similar terms are excluded  

## How to Use the Add-In in Revit

After installation, open Revit and navigate to:  
**Add-Ins → External Tools → BatchExportDWGs**
