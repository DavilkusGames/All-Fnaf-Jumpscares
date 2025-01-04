using UnityEngine;
using UnityEngine.UI;

public class RewardedAdUnlocker : MonoBehaviour
{
    public Button lockedBtn;
    private bool alreadyUnlocked = false;

    private void Start()
    {
        lockedBtn.interactable = false;
        if (GameManager.IsPromoAction || PlayerPrefs.HasKey(gameObject.name))
        {
            alreadyUnlocked = true;
            Unlock();
        }
    }

    public void Click()
    {
        YandexGames.Instance.ShowRewardedAd(Unlock);
    }

    public void Unlock()
    {
        if (!alreadyUnlocked) 
        {
            PlayerPrefs.SetInt(gameObject.name, 1);
            PlayerPrefs.Save();
        }
        lockedBtn.interactable = true;
        Destroy(gameObject);
    }
}
