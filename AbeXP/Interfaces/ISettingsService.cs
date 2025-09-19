namespace AbeXP.Interfaces
{
    public interface ISettingsService
    {
        public string FireBaseRef { get; set; }
        public string Culture { get; set; }
        public string AuthAccessToken { get; set; }
    }
}
