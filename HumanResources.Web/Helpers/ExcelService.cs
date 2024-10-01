using OfficeOpenXml;

namespace HumanResources.Web.Helpers
{
    public class ExcelService
    {
        public void FilterAndCopyExcel(string sourceFilePath, string destinationFilePath, string filterCondition)
        {
            var sourceFileInfo = new FileInfo(sourceFilePath);
            var destinationFileInfo = new FileInfo(destinationFilePath);

            // Check if destination file exists and delete it
            if (destinationFileInfo.Exists)
            {
                destinationFileInfo.Delete();
            }

            using (var package = new ExcelPackage(sourceFileInfo))
            {
                var sourceWorksheet = package.Workbook.Worksheets.FirstOrDefault();

                // Ensure worksheet exists
                if (sourceWorksheet == null)
                {
                    throw new Exception("No worksheet found in the source file.");
                }

                using (var destPackage = new ExcelPackage())
                {
                    var destinationWorksheet = destPackage.Workbook.Worksheets.Add("Filtered Data");

                    // Get the number of rows and columns
                    int rowCount = sourceWorksheet.Dimension.Rows;
                    int colCount = sourceWorksheet.Dimension.Columns;

                    // Check the row and column count for debugging
                    Console.WriteLine($"Source Worksheet: {rowCount} rows, {colCount} columns.");

                    int destRow = 1; // Start writing to the first row of the destination sheet

                    // Loop through rows and filter by Column A (index 1)
                    for (int row = 1; row <= rowCount; row++)
                    {
                        var cellValue = sourceWorksheet.Cells[row, 1].Value?.ToString();

                        // Log the value of Column A for each row (debugging)
                        Console.WriteLine($"Row {row}, Column A value: {cellValue}");

                        if (!string.IsNullOrWhiteSpace(cellValue) && cellValue.Contains(filterCondition))
                        {
                            Console.WriteLine($"Row {row} matches filter condition: {cellValue}");

                            // Copy the entire row to the new sheet if it matches
                            for (int col = 1; col <= colCount; col++)
                            {
                                destinationWorksheet.Cells[destRow, col].Value = sourceWorksheet.Cells[row, col].Value;
                            }

                            destRow++; // Move to the next row in the destination sheet
                        }
                    }

                    // If no rows were copied, log it (debugging)
                    if (destRow == 1)
                    {
                        Console.WriteLine("No rows matched the filter condition.");
                    }

                    // Save the destination file
                    destPackage.SaveAs(destinationFileInfo);
                    Console.WriteLine($"Filtered data saved to {destinationFilePath}");
                }
            }
        }
    }
}