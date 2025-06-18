using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;

namespace RevitAutomationCore
{
    public class ExcelReader
    {
        public List<string> ExtractBuildingNumbers(string excelPath)
        {
            var buildingNumbers = new List<string>();

            try
            {
                // Open the Excel file
                using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read()) // Loop through rows
                        {
                            var cellValue = reader.GetValue(0); // Read raw cell value
                            if (cellValue != null)
                            {
                                string number = cellValue.ToString().Trim();
                                if (!string.IsNullOrWhiteSpace(number))
                                {
                                    buildingNumbers.Add(number.PadLeft(4, '0')); // Ensures 4-digit format
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Extracted {buildingNumbers.Count} building numbers from Excel.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading Excel file: {ex.Message}");
            }

            return buildingNumbers;
        }
    }
}
