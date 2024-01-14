
public static class Login
{
    public static bool AttemptLogin(string username, string password, out string errorMessaged)
    {
        errorMessaged = "";
        
        if (username == "admin" && password == "admin")
        {
            LogIn(username, password);
            return true;
        }

        errorMessaged = "Username or password not found";
        return false;
    }

    private static void LogIn(string username, string password)
    {
        // TODO:
        // Get user data from database

        //temp
        User signInAs = new User(username, "admin@admin.com", password);

        ActiveUser.SetActiveUser(signInAs);
    }

    public static void Logout()
    {
        ActiveUser.SetActiveUser(null);
    }
}
