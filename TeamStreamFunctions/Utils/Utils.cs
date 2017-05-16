using System;

namespace TeamStreamFunctions
{
    public class Utils
    {
        public static string CreateBlobWithImageExtension(string filename, int totalFrames, int frame)
        {
            return filename.Replace(".mp4", $"_{ (object)totalFrames}_{ (object)frame}.jpg");
        }

        public static string GetGuidFromBlobName(string filename)
        {
            return filename.Substring(filename.IndexOf('_') + 1, 36);
        }

        public static int GetThumbnailIndex(string filename)
        {
            int lastunderscore = filename.LastIndexOf('_') + 1; //58 or 59
            int period = filename.IndexOf('.'); //61
            int diff = period - lastunderscore;

            return Convert.ToInt32(filename.Substring(lastunderscore, diff));
        }

        public static int GetThumbnailCount(string filename)
        {
            int middleunderscore = filename.IndexOf('_') + 38;
            int lastnunderscore = filename.LastIndexOf('_');
            int diff = lastnunderscore - middleunderscore;

            return Convert.ToInt32(filename.Substring(middleunderscore, diff));
        }
    }
}