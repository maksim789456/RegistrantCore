namespace Registrant
{
    public partial class App
    {
        public static string ActiveUser;
        public static string LevelAccess;

        public static void SetLevelAccess(string type)
        {
            LevelAccess = type;
        }
        public static string GetLevelAccess()
        {
            return LevelAccess;
        }
        
        public static string GetActiveUser()
        {
            return ActiveUser;
        }

        public static void SetActiveUser(string type)
        {
            ActiveUser = type;
        }

    }
}
