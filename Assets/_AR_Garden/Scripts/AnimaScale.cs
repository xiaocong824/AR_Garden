using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Oculus.Interaction;

public class AnimaScale : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float minScale = 0f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private Transform StartAnima;
    [SerializeField] private Transform EndAnima;
    [SerializeField] private GameObject targetObject;
    
    private Coroutine animationCoroutine;
    private bool isAnimating = false;

    /// <summary>
    /// 开始动画 - 供按钮调用的公共方法
    /// </summary>
    /// 
    void Start()
    {
        targetObject.transform.localScale = new Vector3(minScale, minScale, minScale);
    }
    public void StartAnimation()
    {
        if (isAnimating) return; // 防止重复调用
        
        if (targetObject == null || StartAnima == null || EndAnima == null)
        {
            Debug.LogError("AnimaScale: 缺少必要的组件引用！");
            return;
        }
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        animationCoroutine = StartCoroutine(AnimateObjectCoroutine());
    }
    
    /// <summary>
    /// 停止动画
    /// </summary>
    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isAnimating = false;
    }
    
    /// <summary>
    /// 重置对象到起始状态
    /// </summary>
    public void ResetToStart()
    {
        if (targetObject != null && StartAnima != null)
        {
            targetObject.transform.position = StartAnima.position;
            targetObject.transform.rotation = StartAnima.rotation;
            targetObject.transform.localScale = Vector3.one * minScale;
        }
    }
    
    /// <summary>
    /// 设置对象到结束状态
    /// </summary>
    public void SetToEnd()
    {
        if (targetObject != null && EndAnima != null) 
        {
            targetObject.transform.position = EndAnima.position;
            targetObject.transform.rotation = EndAnima.rotation;
            targetObject.transform.localScale = Vector3.one * maxScale;
        }
    }
    
    private IEnumerator AnimateObjectCoroutine()
    {
        isAnimating = true;
        
        // 记录初始状态
        Vector3 startPosition = StartAnima.position;
        Quaternion startRotation = StartAnima.rotation;
        Vector3 startScale = Vector3.one * minScale;
        
        // 记录目标状态
        Vector3 endPosition = EndAnima.position;
        Quaternion endRotation = EndAnima.rotation;
        Vector3 endScale = Vector3.one * maxScale;
        
        // 设置初始状态
        targetObject.transform.position = startPosition;
        targetObject.transform.rotation = startRotation;
        targetObject.transform.localScale = startScale;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // 使用缓动函数让动画更平滑
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            
            // 插值位置
            targetObject.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
            
            // 插值旋转
            targetObject.transform.rotation = Quaternion.Lerp(startRotation, endRotation, smoothT);
            
            // 插值缩放
            targetObject.transform.localScale = Vector3.Lerp(startScale, endScale, smoothT);
            
            yield return null;
        }
        
        // 确保最终状态精确
        targetObject.transform.position = endPosition;
        targetObject.transform.rotation = endRotation;
        targetObject.transform.localScale = endScale;
        
        isAnimating = false;
        animationCoroutine = null;
    }
    
    // 在Inspector中验证组件
    private void OnValidate()
    {
        if (duration <= 0f)
        {
            duration = 1f;
        }
        
        if (minScale < 0f)
        {
            minScale = 0f;
        }
    }
}
