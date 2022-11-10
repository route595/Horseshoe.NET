using System;
using System.IO;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Excel.Interop
{
    public static class ConvertExcelFile
    {
        public static ConversionSource From(string filePath)
        {
            return new ConversionSource(filePath);
        }

        public class ConversionSource
        {
            public string FilePath { get; }
            internal ConversionSource(string filePath) { FilePath = filePath; }

            public ConversionDest To(ExcelFileTypeByExtension fileType)
            {
                return new ConversionDest(this, null, fileType);
            }

            public ConversionDest To(string filePath, ExcelFileTypeByExtension fileType)
            {
                return new ConversionDest(this, filePath, fileType);
            }
        }

        public class ConversionDest
        {
            public ConversionSource Source { get; }
            public string DestFilePath { get; private set; }
            public ExcelFileTypeByExtension DestFileType { get; }

            internal ConversionDest(ConversionSource source, string filePath, ExcelFileTypeByExtension fileType) 
            {
                Source = source;
                DestFilePath = filePath;
                DestFileType = fileType;
            }

            public string Execute(bool overwriteExisting = false, bool allowAnyExtension = false, bool ignoreSourceFormat = false)
            {
                var sourceFile = new FileInfo(Source.FilePath);
                if (!sourceFile.Exists)
                {
                    throw new ValidationException("Source file does not exist or access is denied: " + sourceFile.FullName);
                }

                if (sourceFile.Extension.Length == 0)
                {
                    if (!allowAnyExtension)
                    {
                        throw new ValidationException("Source file is missing extension (to bypass this set allowAnyExtension = true and try again).");
                    }
                }
                else
                {
                    var acceptedExcelFileTypes = Enum.GetNames(typeof(ExcelFileTypeByExtension));
                    if (!sourceFile.Extension.Substring(1).InIgnoreCase(acceptedExcelFileTypes) && !allowAnyExtension)
                    {
                        throw new ValidationException("\"" + sourceFile.Extension + "\" is not an accepted file extension, try one of [" + string.Join(", ", acceptedExcelFileTypes) + "] (to bypass this set allowAnyExtension = true and try again).");
                    }

                    if (string.Equals(sourceFile.Extension.Substring(1), DestFileType.ToString(), StringComparison.OrdinalIgnoreCase) && !ignoreSourceFormat)
                    {
                        throw new ValidationException("Source file appears to already be in the requested format (to bypass this set ignoreSourceFormat = true and try again).");
                    }
                }

                DestFilePath = DestFilePath 
                    ?? Path.Combine(sourceFile.DirectoryName, sourceFile.Name.Replace(sourceFile.Extension, "") + "." + DestFileType.ToString().ToLower());

                if (File.Exists(this.DestFilePath) && !overwriteExisting)
                {
                    throw new ValidationException("Destination file already exists (to bypass this set overwriteExisting = true and try again).");
                }

                var app = new Microsoft.Office.Interop.Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                var workbook = app.Workbooks.Open(Source.FilePath, Microsoft.Office.Interop.Excel.XlUpdateLinks.xlUpdateLinksNever, true);
                workbook.SaveAs(DestFilePath, DestFileType);
                workbook.Close(false);
                app.Quit();
                return DestFilePath;
            }
        }
    }
}
