using UnityEngine;

public static class CoinWallet
{
    private const string Key = "coins";

    public static int Coins => PlayerPrefs.GetInt(Key, 0);

    public static void Add(int amount)
    {
        PlayerPrefs.SetInt(Key, Coins + amount);
        PlayerPrefs.Save();
    }

    public static bool TrySpend(int amount)
    {
        if (Coins < amount)
        {
            return false;
        }

        PlayerPrefs.SetInt(Key, Coins - amount);
        PlayerPrefs.Save();
        return true;
    }
}
