using Microsoft.SPOT;
using System.IO;
using Microsoft.SPOT.IO;
using System.Text;
using System;
using NetMf.CommonExtensions;

namespace NetduinoReadWriteFile
{
    public class Program
    {
        const string volumnName = "SD";
        private static readonly VolumeInfo volume = new VolumeInfo(volumnName);

        public static void Main()
        {
            ValidateSDVolumn();

            var path = Path.Combine(volumnName, "log.txt");
            File.Delete(path); //Delete file if exist

            //first write
            var textValue = StringUtility.Format("{0} Hello world 1\n", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            WriteTextToFile(path, textValue);

            //second write to the same file
            textValue = StringUtility.Format("{0} Hello world 2\n", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            WriteTextToFile(path, textValue);

            var readTextValue = ReadTextFromFile(path);
            Debug.Print(readTextValue);
        }

        private static void WriteTextToFile(string path, string textValue)
        {
            //more information about FileStream https://docs.microsoft.com/en-us/previous-versions/windows/embedded/y0bs3w9t%28v%3dvs.102%29
            //More information for FileMode Enumeration https://docs.microsoft.com/en-us/previous-versions/windows/embedded/6b40c5ay(v%3dvs.102)

            using (var fileStream = new FileStream(
                path,
                FileMode.Append, //Opens the file if it exists and seeks to the end of the file, or creates a new file. 
                FileAccess.Write))
            {
                //Write some text to a file
                //Use StringUtility from NetMf.CommonExtensions to format string
                var byteData = Encoding.UTF8.GetBytes(textValue);
                fileStream.Write(byteData, 0, byteData.Length);

                //Must call flush to write immediately. 
                //Otherwise, there's no guarantee as to when the file is written. 
                volume.FlushAll();
            }
        }

        private static string ReadTextFromFile(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                // read all texts from log file
                var stringData = streamReader.ReadToEnd();
                return stringData;
            }
        }

        private static void ValidateSDVolumn()
        {
            // check to see if there's an SD card inserted
            if (volume == null)
            {
                throw new InvalidOperationException("No SD volumn, make sure you have inserted a SD card");
            }
        }
    }
}
