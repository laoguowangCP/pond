using System;
using System.IO;
using System.Text;

public static class Filey
{
    public static bool TryAddTimestampToFileNameIfCollide(ref StringBuilder fileNameBuilder)
    {
        string fileName = fileNameBuilder.ToString();
        if (File.Exists(fileName))
        {
            string fileExt = Path.GetExtension(fileName);
            fileNameBuilder.Remove(fileNameBuilder.Length - fileExt.Length, fileExt.Length);
            fileNameBuilder.Append(DateTime.Now.ToString("_yyyyMMdd_HHmmssff"));
            // fileNameBuilder.Append('.');
            fileNameBuilder.Append(fileExt);
            if (File.Exists(fileNameBuilder.ToString()))
            {
                // Too short interval between 2 drops.
                return false;
            }
        }

        return true;
    }
}
