namespace PokemonCRUD.Core.Common
{
    public static class Constants
    {
        public const string SecretKey = "thisIsASecureKeyOfAtLeast12Characters";
    }
    public static class ResultMessage
    {
        public const string Ok = "Ok";
        public const string Exists = "Exists";
        public const string Updated = "Updated";
        public const string Deleted = "Deleted";
        public const string Error = "Error";
        public const string NotFound = "NotFound";
        public const string Empty = "Empty";      
    }
    public static class CustomMessage
    {

        public const string OkCsvPath = "CSV File Path Configured correctly";
        public const string NotExistsFile = "The file specified doesn't exists";
        public const string ErrorFilePath = "Error when configuring the file path";
    }
}
