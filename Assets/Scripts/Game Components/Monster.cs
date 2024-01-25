
public enum Element { Fire, Water, Grass}
public enum Position { Front, BackRight, BackLeft}
public class Monster {

    // Stats
    public int curHealth;
    public int maxHealth;
    public Element myElement;
    public Position myPosition;
    public bool alive;

    // Counters
    public int damageCounters;
    public int slowingCounters;
}
