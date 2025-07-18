using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Oculus.Interaction;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Sprite _hilightSprite;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _confirmHilightSprite;
    [SerializeField] private Sprite _confirmNormalSprite;

    private int _selectedButtonIndex = -1; // 记录当前选中的按钮索引

    void Start()
    {
        //_confirmButton.interactable = false;
        _confirmButton.GetComponent<Image>().sprite = _confirmNormalSprite;

        // 为每个按钮添加OnClick事件，调用OnButtonClickManual方法
        for (int i = 0; i < _buttons.Length; i++)
        {
            int buttonIndex = i; // 捕获循环变量
            _buttons[i].onClick.AddListener(() => OnButtonClickManual(buttonIndex));
            
            // 初始化按钮sprite
            _buttons[i].GetComponent<Image>().sprite = _normalSprite;
        }
    }

    private void OnButtonClick(int buttonIndex)
    {

        if (_selectedButtonIndex == buttonIndex)
            return;

        ResetAllButtons();
        _selectedButtonIndex = buttonIndex;
        _buttons[buttonIndex].GetComponent<Image>().sprite = _hilightSprite;

        //_confirmButton.interactable = true;
        _confirmButton.GetComponent<Image>().sprite = _confirmHilightSprite;
        
    }

    private void ResetAllButtons()
    {
        // 将所有按钮重置为普通状态
        foreach (var button in _buttons)
        {
            button.GetComponent<Image>().sprite = _normalSprite;
        }
        _selectedButtonIndex = -1;
    }


    public void OnButtonClickManual(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < _buttons.Length)
        {
            OnButtonClick(buttonIndex);
        }
    }

    // 获取当前选中的按钮索引（可用于其他脚本调用）
    public int GetSelectedButtonIndex()
    {
        return _selectedButtonIndex;
    }

    // 重置选择状态（可用于其他脚本调用）
    public void ResetSelection()
    {
        ResetAllButtons();
        //_confirmButton.interactable = false;
        _confirmButton.GetComponent<Image>().sprite = _confirmNormalSprite;
    }
}
