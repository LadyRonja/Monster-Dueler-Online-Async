

public class User
{
    string username; 
    string email; // Primary Key
    string password; // TODO: Currently stored in plain text
    bool admin = false;

    public User(string username, string email, string password)
    {
        this.username = username;
        this.email = email;
        this.password = password;
    }
}
