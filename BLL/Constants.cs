using System;

namespace DatingWeb.BLL
{
    public static class Constants
    {
        public static string BlobStorage = "https://midpotstorage.blob.core.windows.net";
        public static string PhotoStorage(string value) => String.Format("{0}{1}{2}", "https://midpotstorage.blob.core.windows.net/firstcontainer/", value, ".jpeg");
        public static string PhotoStorageId(string value) => value.Replace("https://midpotstorage.blob.core.windows.net/firstcontainer/", "").Replace(".jpeg", "");
        public static string StoryStorage(string value) => String.Format("{0}{1}{2}", "https://midpotstorage.blob.core.windows.net/secondcontainer/", value, ".jpeg");
        public static string StoryStorageId(string value) => value.Replace("https://midpotstorage.blob.core.windows.net/secondcontainer/", "").Replace(".jpeg", "");
    }
}
