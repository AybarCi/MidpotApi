namespace DatingWeb.Model.Request
{
    public class UpdateProfileSettingsRequest
    {
        public string Description { get; set; }
        public int FromAge { get; set; }
        public int UntilAge { get; set; }
        public string School { get; set; }
        public string Job { get; set; }
        public string DeviceToken { get; set; }
        public bool GhostMode { get; set; }
        public string PersonName { get; set; }
    }
}
