using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;
using UnityEngine.Analytics;




#if UNITY_ANDROID || UNITY_IOS || UNITY_WP_8_1
using UnityEngine.Advertisements;
#endif
using UnityEngine.SocialPlatforms;

public class GameController : MonoBehaviour {
    //Color of water
    public Color waterColor = Color.blue;
    //UI Animator
    public Animator uiAnimator;
    //UI Elements
    public TextMeshProUGUI bestText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;
    public Text gemReviveText;
    public Image tiltControlIndicator;
    public Image tapControlIndicator;
    public Image soundBTN;
    public Sprite soundON;
    public Sprite soundOFF;
    public Button gemReviveButton;
    public GameObject revivePanel;
    public GameObject overPanel;
    //Current boat in use
    [HideInInspector]
    public BoatControllers currentBoat;
    //Camera Control
    public SmoothFollow cameraScript;
    //Scroll Script
    public ScrollController scrollController;
    //Coins collected
    int coins = 0;
    float lastTime = 0.0f;
    //times saved counter
    int timesSaved = 0;
    //All the boats
    public GameObject[] allBoats;
    public string clickid;
    private StarkAdManager starkAdManager;


    // Use this for initialization
    //Enable selected boat at start

    void Start () {
        int selectedBoat = PlayerPrefs.GetInt("SelectedBoat", 0);
        if (allBoats[selectedBoat]) {
            allBoats[selectedBoat].SetActive(true);
        }

        SetControlVisual();
        SwitchAudioVisual();
    }
	
    //Setup end panel values
    public void EnableDisableEndPanel(string state , float score , int coins){

        if (!scoreText || !bestText || !coinText || !gemText || !revivePanel || !overPanel)
        {
            Debug.LogWarning("Please assign all the variables");
            return;
        }

        if (!uiAnimator)
        {
            Debug.LogWarning("Some variables are not assigned");
            return;
        }

        if (gemReviveText && timesSaved < 3) {
            timesSaved++;
            if (PlayerPrefs.GetInt("Gems", 0) >= (timesSaved * 3)) {
                gemReviveText.text = "Required Gems : " + (timesSaved * 3).ToString() + "\n You have : " + PlayerPrefs.GetInt("Gems", 0).ToString() + " Gems";
                state = "Revive";
            }

            if (timesSaved > 1) {
                if (gemReviveButton) {
                    gemReviveButton.interactable = false;
                }
            }

            
        }

        if (!uiAnimator.enabled)
        {
            uiAnimator.enabled = true;
            return;
        }
        else {
            //switch (state) {
            //    case "Over": uiAnimator.SetTrigger("Over");
            //        overPanel.SetActive(true);
            //        revivePanel.SetActive(false);
            //        AddCoins();
            //        break;
            //    case "Revive": uiAnimator.SetTrigger("Over");
            //        overPanel.SetActive(false);
            //        revivePanel.SetActive(true);
            //        break;
            //}
            uiAnimator.SetTrigger("Over");
            overPanel.SetActive(true);
            ShowInterstitialAd("65btqfsm3gi2bcorhg",
            () => {

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
            revivePanel.SetActive(false);
            AddCoins();
        }



        if (score > PlayerPrefs.GetFloat("Best", 0))
        {
            PlayerPrefs.SetFloat("Best", score);
        }


        bestText.text = "最高分 : " + PlayerPrefs.GetFloat("Best", 0).ToString("F0");
        scoreText.text = "得分 : " + score.ToString("F0");
        coinText.text = "X " + PlayerPrefs.GetInt("Coins", 0);
        gemText.text = PlayerPrefs.GetInt("Gems", 0) + " X";

    }
    //Set coin value
    public void SetCoinValue(int currentCoins) {
        coins = currentCoins;
    }
    //Add coins
    public void AddCoins() {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + coins);
    }
    //Pause
    public void Pause() {
        lastTime = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    //UnPause
    public void UnPause() {
        Time.timeScale = lastTime;
    }
    //Revive with gems
    public void ReviveWithGems() {

        ShowVideoAd("2pn71h3p4omc8khe40",
            (bol) => {
                if (bol)
                {

                    uiAnimator.SetTrigger("Reset");
                    currentBoat.Revive();


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }
    public void AddCoinsONE()
    {
        ShowVideoAd("2pn71h3p4omc8khe40",
            (bol) => {
                if (bol)
                {


                    PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);

                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
    }
    //Revive with video
    public void ReviveWithVideo() {
        Debug.Log("Video Ad code is here");
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
#endif
    }

#if UNITY_ADS
    //Call back for reward video
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                if (uiAnimator)
                {
                    uiAnimator.SetTrigger("Reset");
                }

                if (currentBoat)
                {
                    currentBoat.Revive();
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
#endif

    //Restart game
    public void Restart()
    {
        Initiate.Fade(SceneManager.GetActiveScene().name,waterColor,2.0f);
    }
    //Set Control type button visual
    void SetControlVisual() {
        if (!tiltControlIndicator || !tapControlIndicator) {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        switch (PlayerPrefs.GetInt("ControlType", 0))
        {

            case 0:
                tiltControlIndicator.color = Color.green;
                tapControlIndicator.color = Color.black;

                break;
            case 1:
                tiltControlIndicator.color = Color.black;
                tapControlIndicator.color = Color.green;
                break;
        }
    }
    //Get control type
    public void ChangeControlType(int type) {
        switch (type)
        {
            case 0:
                PlayerPrefs.SetInt("ControlType", 1);
                break;
            case 1:
                PlayerPrefs.SetInt("ControlType", 0);
                break;
        }
        SetControlVisual();
    }
    public void Update()
    {
        
        coinText.text = "X " + PlayerPrefs.GetInt("Coins", 0);
        
    }
    //Switch audio
    public void SwitchAudio() {
        if (PlayerPrefs.GetInt("SFX", 1) == 1) {
            PlayerPrefs.SetInt("SFX", 0);
        } else if (PlayerPrefs.GetInt("SFX", 1) == 0) {
            PlayerPrefs.SetInt("SFX", 1);
        }

        SwitchAudioVisual();
    }

    //Audio toggle button
    public void SwitchAudioVisual() {
        if (!soundBTN || !soundOFF || !soundON)
        {
            Debug.LogWarning("Please Assign all the variables");
            return;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 1)
        {
            soundBTN.sprite = soundON;
            AudioListener.volume = 1.0f;
        }

        if (PlayerPrefs.GetInt("SFX", 1) == 0)
        {
            soundBTN.sprite = soundOFF;
            AudioListener.volume = 0.0f;
        }
    }

    //start the game
    public void StartTheGame() {
        if (currentBoat) {
            currentBoat.StartTheGame();
            if (cameraScript) {
                cameraScript.target = currentBoat.transform;
            }
        }
        if (scrollController)
        {
            scrollController.StartSetup();
        }
        
    }

    //Go to buying scene
    public void Buy() {
        Initiate.Fade("Buy", Color.white, 2.0f);
    }

    public void Leaderboard() {
        //Call "(int)PlayerPrefs.GetFloat("Best", 0)" to get the best score
        Debug.Log("Leaderboard : Put your code here , Double click to open in IDE");
    }
    //rate button
    public void Rate()
    {
        //Application.OpenURL("Put your URL here then uncomment it");

        Debug.Log("Rate : Put your code here , Double click to open in IDE");
    }
    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
