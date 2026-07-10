using System;
using System.IO;

namespace BookManagement.Services.Utils
{
    public static class FileStorageHelper
    {
        private const string FolderName = "Manuscripts";

        // Check if the file is a PDF
        public static bool IsPdfFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            string extension = Path.GetExtension(filePath);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        // Copy selected PDF to local Manuscripts folder and return the relative path
        public static string CopyPdfToStorage(string sourceFilePath, string title)
        {
            if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException("Tệp tin nguồn không tồn tại.");
            }

            if (!IsPdfFile(sourceFilePath))
            {
                throw new ArgumentException("Chỉ cho phép chọn tệp tin định dạng PDF.");
            }

            // Target folder path: AppDomain.CurrentDomain.BaseDirectory/Manuscripts
            string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FolderName);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Create a safe unique file name based on the book title
            string safeTitle = string.IsNullOrWhiteSpace(title) 
                ? "manuscript" 
                : string.Concat(title.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_").ToLower();
            
            string uniqueFileName = $"{safeTitle}_{Guid.NewGuid()}.pdf";
            string destFilePath = Path.Combine(targetFolder, uniqueFileName);

            // Copy file physically
            File.Copy(sourceFilePath, destFilePath, true);

            // Return relative path (e.g. Manuscripts/title_guid.pdf)
            return Path.Combine(FolderName, uniqueFileName).Replace('\\', '/');
        }

        // Delete PDF file physically
        public static void DeletePdfFromStorage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Không thể xóa file bản thảo cũ: {ex.Message}");
                }
            }
        }
    }
}
