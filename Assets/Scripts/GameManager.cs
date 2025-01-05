using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Jumpscare
{
    public string videoPath = string.Empty;
    public AudioClip clip = null;
    public float showTime = 0f;
    public bool loop = true;
}

public class GameManager : MonoBehaviour
{
    public GameObject loadingPanel;
    public Button moreGamesBtn;
    public TMP_Text versionTxt;
    public GameObject canvas;

    public GameObject[] gamePanels;
    public GameObject[] gameBtnsSelections;
    public GameObject[] gameBtnsAdUnlockers;

    public VideoPlayer videoPlayer;
    public AudioSource audioPlayer;

    public float loadTimeout = 3f;

    public static GameManager Instance { get; private set; }

    public Jumpscare[] jumpscares;

    private int selectedGameId = 0;
    private int playingJumpscareId = -1;
    private int unlockingGameId = -1;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        videoPlayer.prepareCompleted += VideoReady;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        versionTxt.text = "v." + Application.version;

        if (moreGamesBtn != null)
            moreGamesBtn.interactable = !string.IsNullOrEmpty(YandexGames.DevLink);

        if (GameData.dataLoaded) DataLoaded(false);
        else if (Application.isEditor) GameData.LoadData();
        else loadingPanel.SetActive(true);
    }

    public void DataLoaded(bool firstTime)
    {
        loadingPanel.SetActive(false);
        if (firstTime) YandexGames.Instance.GameInitialized();

        for (int i = 0; i < gameBtnsAdUnlockers.Length; i++)
        {
            gameBtnsAdUnlockers[i].SetActive(!GameData.data.gamesUnlocked[i] && !YandexGames.IsPromoActive);
        }
    }

    public void SelectGame(int id)
    {
        if (id == selectedGameId) return;
        if (!GameData.data.gamesUnlocked[id] && !YandexGames.IsPromoActive)
        {
            unlockingGameId = id;
            YandexGames.Instance.ShowRewarded(UnlockGame);
        }
        else
        {
            gamePanels[selectedGameId].SetActive(false);
            gameBtnsSelections[selectedGameId].SetActive(false);
            selectedGameId = id;
            gamePanels[selectedGameId].SetActive(true);
            gameBtnsSelections[selectedGameId].SetActive(true);
        }
    }

    public void UnlockGame(bool success)
    {
        if (!success || unlockingGameId == -1) return;
        gameBtnsAdUnlockers[unlockingGameId].SetActive(false);
        GameData.data.gamesUnlocked[unlockingGameId] = true;
        GameData.SaveData();
        SelectGame(unlockingGameId);
    }

    public void PlayJumpscare(int id)
    {
        if (playingJumpscareId != -1) return;
        playingJumpscareId = id;

        string videoPath = Path.Combine(Application.streamingAssetsPath, jumpscares[id].videoPath);
        videoPlayer.url = videoPath;
        videoPlayer.isLooping = jumpscares[id].loop;
        videoPlayer.Play();

        loadingPanel.SetActive(true);
        Invoke(nameof(JumpscareLoadTimeout), loadTimeout);
    }

    public void VideoReady(VideoPlayer source)
    {
        if (jumpscares[playingJumpscareId].clip != null)
        {
            audioPlayer.clip = jumpscares[playingJumpscareId].clip;
            audioPlayer.Play();
        }

        loadingPanel.SetActive(false);
        canvas.SetActive(false);
        Invoke(nameof(JumpscareWatched), jumpscares[playingJumpscareId].showTime);
        CancelInvoke(nameof(JumpscareLoadTimeout));
    }

    private void JumpscareLoadTimeout()
    {
        playingJumpscareId = -1;
        
        videoPlayer.Stop();
        videoPlayer.url = string.Empty;

        loadingPanel.SetActive(false);
    }

    private void JumpscareWatched()
    {
        canvas.SetActive(true);
        playingJumpscareId = -1;

        videoPlayer.Stop();
        videoPlayer.url = string.Empty;
        audioPlayer.Stop();

        GameData.data.jumpscaresWatched++;
        GameData.SaveData();
        YandexGames.Instance.SaveToLeaderboard(GameData.data.jumpscaresWatched);
    }

    public void MoreGames()
    {
        if (!string.IsNullOrEmpty(YandexGames.DevLink))
            Application.OpenURL(YandexGames.DevLink);
    }

    public void DevLinkLoaded()
    {
        moreGamesBtn.interactable = true;
    }

    public void PromoIsActive()
    {
        // NOTHING
    }
}
