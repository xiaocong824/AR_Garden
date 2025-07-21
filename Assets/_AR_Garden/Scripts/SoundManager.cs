using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Oculus.Interaction;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("按钮点击音效")]
    public AudioClip buttonClickClip;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 播放按钮点击音效（可在UI按钮OnClick事件中直接调用）
    /// </summary>
    public void PlayButtonClick()
    {
        if (buttonClickClip != null)
            audioSource.PlayOneShot(buttonClickClip);
    }

    /// <summary>
    /// 静态方法，方便UI直接调用
    /// </summary>
    public static void PlayButtonClickStatic()
    {
        if (Instance != null)
            Instance.PlayButtonClick();
    }
}
