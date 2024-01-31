
using System;

public enum Element { Fire, Water, Grass}
public enum Position { Front, BackRight, BackLeft}
[Serializable]
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

    public Monster(int maxHealth, Element element, Position position)
    {
        this.maxHealth = maxHealth;
        this.curHealth = maxHealth;
        this.myElement = element;
        this.myPosition = position;

        alive = true;
        damageCounters = 0;
        slowingCounters = 0;
    }
}
