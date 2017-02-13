using UnityEngine;
using System.Collections;
using System;

public class EndScreen : MonoBehaviour
{
    public float pauseTimeScale = 0.0000000001f;
    public ColorInOut blackout;
    public ColorInOut creditsTexture, shareTexture;
    public TextMesh playTimeText;
    public TextColorInOut playTimeAnimator;
    public float startTime;

    public int shareCounter = 10;
    private int shareCurrentCount;

    public GameObject warpObj, centerStructure;

    public bool test;

    public string shareText = "https://play.google.com/store/apps/details?id=com.byhook.exo";
    public string shareMsgSubject = "Check out this Daydream VR game!";
    public string sharePopupMessage = "Share with...";
    public TextMesh shareCounterText;
    public TextColorInOut shareCounterAnimator;
    private bool sharingTriggerAllowed;


    void Start()
    {
        startTime = Time.time;

        playTimeAnimator.gameObject.SetActive(true);
        playTimeAnimator.Deactivated();
        playTimeAnimator.gameObject.SetActive(false);

        shareCounterAnimator.gameObject.SetActive(true);
        shareCounterAnimator.Deactivated();
        shareCounterAnimator.gameObject.SetActive(false);
    }

    public void End()
    {
        blackout.gameObject.SetActive(true);
        playTimeText.text = GetPlayTime();

        StartCoroutine(EndCoroutine());
    }
    IEnumerator EndCoroutine()
    {
        bool finished = false;

        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(blackout, () =>
        {
            warpObj.SetActive(false);
            centerStructure.SetActive(false);
            blackout.gameObject.SetActive(false);
            playTimeAnimator.gameObject.SetActive(true);
            creditsTexture.gameObject.SetActive(true);
            creditsTexture.Deactivated();
            Application.targetFrameRate = 60;
            StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(playTimeAnimator));
            StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(creditsTexture, () =>
            {
                finished = true;
            }));
        }));

        while(!finished)
        {
            yield return null;
        }

        sharingTriggerAllowed = true;
    }
    private string GetPlayTime()
    {
        float diff = Time.time - startTime;

        int hours = Mathf.FloorToInt(diff / 3600);
        int min = Mathf.FloorToInt((diff % 3600) / 60);
        int sec = Mathf.FloorToInt((diff % 60));

        string res = "Play Time: ";

        if (hours < 10)
            res += "0" + hours;
        else
            res += hours;
        res += ":";
        if (min < 10)
            res += "0" + min;
        else
            res += min;
        res += ":";
        if (sec < 10)
            res += "0" + sec;
        else
            res += sec;

        return res;
    }

    void Update()
    {
        if (test)
        {
            End();
            test = false;
        }

        if(sharingTriggerAllowed && GvrController.AppButtonUp)
        {
            StartCoroutine(ShareCoroutine());
        }
    }

    IEnumerator ShareCoroutine()
    {
        sharingTriggerAllowed = false;

        bool finished = false;
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(playTimeAnimator));
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(creditsTexture, () =>
        {
            finished = true;
        }));
        while (!finished)
        {
            yield return null;
        }
        playTimeAnimator.gameObject.SetActive(false);
        creditsTexture.gameObject.SetActive(false);

        shareCurrentCount = shareCounter;

        finished = false;
        shareCounterText.text = shareCurrentCount.ToString();
        shareCounterAnimator.gameObject.SetActive(true);
        shareTexture.gameObject.SetActive(true);
        shareTexture.Deactivated();
        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(shareCounterAnimator));
        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(shareTexture, () =>
        {
            finished = true;
        }));
        while (!finished)
        {
            yield return null;
        }

        while (shareCurrentCount > 1)
        {
            shareCurrentCount = Math.Max(0, shareCurrentCount - 1);
            shareCounterText.text = shareCurrentCount.ToString();
            yield return new WaitForSeconds(1);
        }

        finished = false;
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(shareCounterAnimator));
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(shareTexture, () =>
        {
            finished = true;
        }));
        while (!finished)
        {
            yield return null;
        }
        shareCounterAnimator.gameObject.SetActive(false);
        shareTexture.gameObject.SetActive(false);

        string res = shareText.Replace("$time$", GetPlayTime());
        ShareUrl(res, shareMsgSubject, sharePopupMessage);
    }

    private void ShareUrl(string text, string subject=null, string sharePopupMessage = "Share with...")
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
            {
                using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
                {
                    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                    intentObject.Call<AndroidJavaObject>("setType", "text/plain");

                    if (!string.IsNullOrEmpty(subject))
                        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);

                    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
                    // force a choice every-time
                    using (AndroidJavaObject chooserIntent = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, sharePopupMessage))
                    {
                        using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        {
                            using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                            {
                                currentActivity.Call("startActivity", chooserIntent);
                                //StartCoroutine(EndCoroutine());
                                Application.Quit();
                            }
                        }
                    }
                }
            }
        }
    }
}
