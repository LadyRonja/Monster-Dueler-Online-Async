

using System.Linq;

public static class Register
{
    public static string[] insecurePasswords = { "123456789", "987654321", "password", "password123", "admin", "drowssap", "abcdef", "abc123", "123abc" };

    public static bool AttemptRegister(RegisterData newUserData, out string errorMsg)
    {
        errorMsg = "";

        // TODO:
        // Check if username is vialbe
        // Check if username already in the database
        // Check if password is secure, properly
        // Check if email is viable
        // Check if email is already in use

        if (newUserData.username == ""
            || newUserData.password == ""
            || newUserData.passwordRepeat == ""
            || newUserData.email == "")
        {
            errorMsg = "Please awnser all fields";
            return false;
        }

        if(newUserData.password != newUserData.passwordRepeat)
        {
            errorMsg = "Passwords do not match";
            return false;
        }

        if (newUserData.password.ToCharArray().Length < 5)
        {
            errorMsg = "Insecure Password";
            return false;
        }

        if (insecurePasswords.Contains(newUserData.password))
        {
            errorMsg = "Insecure Password";
            return false;
        }

        RegisterNewUser(newUserData);
        return true;
    }

    private static void RegisterNewUser(RegisterData newUserData)
    {
        // Upload new user to database here
    }
}

public struct RegisterData
{
    public RegisterData(string _username, string _pw, string _pwRepeat, string _email)
    {
        username = _username;
        password = _pw;
        passwordRepeat = _pwRepeat;
        email = _email;
    }

    public string username;
    public string password;
    public string passwordRepeat;
    public string email;
}