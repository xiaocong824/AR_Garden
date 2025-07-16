using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPanel : MonoBehaviour
{
    [SerializeField] private Toggle _toggle;
    [SerializeField] private Sprite _pauseSprite;
    [SerializeField] private Sprite _playSprite;
    [SerializeField] private VideoPlayer _videoPlayer;

    private bool _isPlaying = true; // 手动跟踪播放状态

    void Start()
    {
        // 初始化状态
        _isPlaying = true;
        UpdateToggleVisual();
        
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
        
        UpdateToggleVisual();
    }
    
    // 手动设置为播放状态
    public void PlayVideo()
    {
        _isPlaying = true;
        
        if (_videoPlayer != null)
        {
            _videoPlayer.Play();
        }
        
        UpdateToggleVisual();
    }
    
    // 手动设置为暂停状态
    public void PauseVideo()
    {
        _isPlaying = false;
        
        if (_videoPlayer != null)
        {
            _videoPlayer.Pause();
        }
        
        UpdateToggleVisual();
    }
    
    private void UpdateToggleVisual()
    {
        // 更新Toggle的视觉状态（不触发事件）
        _toggle.SetIsOnWithoutNotify(_isPlaying);
        
        if (_isPlaying)
        {
            _toggle.GetComponent<Image>().sprite = _pauseSprite;
        }
        else
        {
            _toggle.GetComponent<Image>().sprite = _playSprite;
        }
    }

    // 获取当前播放状态
    public bool IsPlaying()
    {
        return _isPlaying;
    }
}
