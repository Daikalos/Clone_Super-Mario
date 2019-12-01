using System.IO;

namespace Super_Mario
{
    static class FileReader
    {
        /// <summary>
        /// Returns all instances of specified information inside a text document
        /// </summary>
        public static string[] FindInfo(string aPath, string aName, char aSeperator)
        {
            if (File.Exists(aPath))
            {
                if (new FileInfo(aPath).Length > 0)
                {
                    string[] tempFoundInfo;
                    int tempInfoSize = 0;

                    string[] tempReadFile = File.ReadAllLines(aPath);
                    string[] tempFoundValues = new string[tempReadFile.Length];
                    for (int i = 0; i < tempReadFile.Length; i++)
                    {
                        string[] tempSplitText = tempReadFile[i].Split(aSeperator);
                        if (tempSplitText[0] == aName)
                        {
                            tempInfoSize++;
                            tempFoundValues[i] = tempSplitText[1];
                        }
                    }

                    tempFoundInfo = new string[tempInfoSize];
                    for (int i = 0; i < tempFoundValues.Length; i++)
                    {
                        if (tempFoundValues[i] != null)
                        {
                            tempFoundInfo[i] = tempFoundValues[i];
                        }
                    }
                    return tempFoundInfo;
                }
            }
            return new string[0];
        }

        /// <summary>
        /// Returns all file names within a specified folder
        /// <returns></returns>
        public static string[] FindFileNames(string aPath)
        {
            DirectoryInfo tempInfo = new DirectoryInfo(aPath);
            FileInfo[] tempFiles = tempInfo.GetFiles("*.txt");

            if (tempFiles.Length > 0)
            {
                string[] tempFileNames = new string[tempFiles.Length];
                for (int i = 0; i < tempFiles.Length; i++)
                {
                    tempFileNames[i] = tempFiles[i].Name;
                    tempFileNames[i] = tempFileNames[i].Replace(".txt", "");
                }

                return tempFileNames;
            }
            return new string[0];
        }
    }
}
