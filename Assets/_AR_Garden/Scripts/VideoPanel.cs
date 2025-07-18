using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Oculus.Interaction;

public class VideoPanel : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;
    [SerializeField] private VideoPlayer _videoPlayer;

    private bool _isPlaying = true; // 手动跟踪播放状态

    void Start()
    {
        // 初始化状态
        _isPlaying = true;
        UpdateButtonVisual();
        
        // 确保VideoPlayer初始状态
        if (_videoPlayer != null)
        {
            _videoPlayer.Play();
        }
    }

    // 手动切换toggle状态（供Unity Event Wrapper调用）
    public void ToggleVideo()
    {
        _isPlaying = !_isPlaying;
        
        if (_videoPlayer == null) return;
        
        if (_isPlaying)
        {
            _videoPlayer.Play();
        }
        else
        {
            _videoPlayer.Pause();
        }
        
        UpdateButtonVisual();
    }
    
    // 手动设置为播放状态
    public void PlayVideo()
    {
        _isPlaying = true;
        
        if (_videoPlayer != null)
        {
            _videoPlayer.Play();
        }
        
        UpdateButtonVisual();
    }
    
    // 手动设置为暂停状态
    public void PauseVideo()
    {
        _isPlaying = false;
        
        if (_videoPlayer != null)
        {
            _videoPlayer.Pause();
        }
        
        UpdateButtonVisual();
    }
    
    private void UpdateButtonVisual()
    {
        if (_isPlaying)
        {
            _button.GetComponent<Image>().sprite = _pauseSprite;
        }
        else
        {
            _button.GetComponent<Image>().sprite = _playSprite;
        }
    }

    // 获取当前播放状态
    public bool IsPlaying()
    {
        return _isPlaying;
    }
}
