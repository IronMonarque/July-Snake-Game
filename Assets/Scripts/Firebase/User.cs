[System.Serializable]
public class User
{
    public string username;
    public int highscore;

    public User(string username, int highscore)
    {
        this.username = username;
        this.highscore = highscore;
    }
}
