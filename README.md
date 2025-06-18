This project creates a custom Revit Addin used to batch export DWG Floorplans from many Revit files located in a directory. 

Specifics:
- Reads an Excel file with a list of building names to target
- Creates saved local .rvt copies of each targeted building from the collaborative central models located on network drives, without modifying the central models
- exports DWGs from each building, saving them into RevitExports\Subfolder(with building name)\Whatever_you_want_to_name_them.dwgs
- Paths and naming conventions should be changed for the user's application, and are labeled in the code
- Only certain DWGs are exported: Right now the logic only exports Floorplans, and no plans with "3D", "Construction", "Remodel", "Demolition", or anything related to those names
After download, you can find the add-in in Revit:
- Navigate to Add-ins on the ribbon --> External Tools --> BatchExportDWGs
