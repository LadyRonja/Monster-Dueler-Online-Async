

public class ActiveUser
{
    private static User activeUser;
    public static User CurrentActiveUser { get => GetActiveUser(); private set => activeUser = value; }

    public static void SetActiveUser(User userToSet)
    {
        activeUser = userToSet;
    }

    private static User GetActiveUser()
    {
        if(activeUser == null) {
            return new User("Guest", "guest@guest.com", "password123");
        }

        return activeUser;
    }
}
