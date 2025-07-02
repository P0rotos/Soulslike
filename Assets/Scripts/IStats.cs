[System.Serializable]
public class Stats
{
    public float mov = 8.0f;
    public float str = 1.0f;
    public float dex = 1.0f;
    public float mind = 1.0f;
    public float def = 1.0f;
    public float mdef = 1.0f;
    public float vit = 10.0f;
    public float res = 10.0f;
}

public interface IStats
{
    Stats stats { get; set; }
}