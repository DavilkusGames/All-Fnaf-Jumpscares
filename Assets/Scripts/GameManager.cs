using System.IO;
using UnityEngine;
using UnityEngine.Video;
using Plugins.Audio.Core;
using System;

[System.Serializable]
public class Jumpscare
{
    public string videoPath = string.Empty;
    public string clipName;
    public float showTime = 0f;
    public bool loop = true;
}

public class GameManager : MonoBehaviour
{
    public string promoStartStr = string.Empty;
    public string promoEndStr = string.Empty;

    public GameObject jumpscarePanel;
    public GameObject jumpscareRenderPanel;
    public GameObject[] gamePanels;
    public GameObject[] gameBtnsSelections;

    public VideoPlayer videoPlayer;
    public SourceAudio audioPlayer;

    public static GameManager Instance { get; private set; }
    public static bool IsPromoAction { get; private set; }

    public Jumpscare[] jumpscares;

    private int selectedGameId = 0;
    private int playingJumpscareId = 0;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        videoPlayer.prepareCompleted += VideoReady;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        DateTime dateTime = DateTime.Today;
        Debug.Log("DateTime now (UTC): " + dateTime.ToString());
        IsPromoAction = (dateTime.Date >= DateTime.ParseExact(promoStartStr, "dd/MM/yyyy", null).Date && 
            dateTime.Date <= DateTime.ParseExact(promoEndStr, "dd/MM/yyyy", null).Date);
        Debug.Log("Is Promo: " + IsPromoAction.ToString());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SelectGame(int id)
    {
        if (id == selectedGameId) return;
        gamePanels[selectedGameId].SetActive(false);
        gameBtnsSelections[selectedGameId].SetActive(false);
        selectedGameId = id;
        gamePanels[selectedGameId].SetActive(true);
        gameBtnsSelections[selectedGameId].SetActive(true);
    }

    public void PlayJumpscare(int id)
    {
        string videoPath = Path.Combine(Application.streamingAssetsPath, jumpscares[id].videoPath);
        videoPlayer.url = videoPath;
        videoPlayer.isLooping = jumpscares[id].loop;
        videoPlayer.Play();

        jumpscareRenderPanel.SetActive(false);
        jumpscarePanel.SetActive(true);
        playingJumpscareId = id;
    }

    public void VideoReady(VideoPlayer source)
    {
        if (jumpscares[playingJumpscareId].clipName != string.Empty)
        {
            audioPlayer.Play(jumpscares[playingJumpscareId].clipName);
        }
        jumpscareRenderPanel.SetActive(true);
        Invoke(nameof(CloseVideoPanel), jumpscares[playingJumpscareId].showTime);
    }

    private void CloseVideoPanel()
    {
        audioPlayer.Stop();
        jumpscarePanel.SetActive(false);
    }
}
