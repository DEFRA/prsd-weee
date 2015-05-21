namespace EA.Weee.DocumentGeneration
{
    using System;
    using System.IO;

    internal static class DocumentHelper
    {
        /// <summary>
        /// File.ReadAllBytes(path) will throw an exception if multiple process attempt to access the same file.
        /// This method will allow shared access to the file.
        /// </summary>
        /// <param name="fileNameWithPath">Fully qualified file name to read.</param>
        /// <returns>The full byte array of the file.</returns>
        public static byte[] ReadDocumentShared(string fileNameWithPath)
        {
            byte[] fileBytes;

            using (var fileStream = new FileStream(fileNameWithPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int bytesToRead = (int)fileStream.Length;

                fileBytes = new byte[bytesToRead];

                fileStream.Read(fileBytes, 0, bytesToRead);
            }

            return fileBytes;
        }

        /// <summary>
        /// Gets the location of the document templates assuming they have been compiled to the bin.
        /// </summary>
        /// <returns>The full path name of the document directory ending in "\"</returns>
        public static string GetDocumentDirectory()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;

            return root + "\\Documents\\";
        }
    }
}