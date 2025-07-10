using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ruler : MonoBehaviour
{
    [Header("测量点")]
    public GameObject StartA;
    public GameObject EndA;
    public GameObject TextObject;
    
    [Header("显示组件")]
    public LineRenderer lineRenderer;
    
    [Header("文本设置")]
    public float textSize = 0.1f;
    public Color textColor = Color.white;
    
    private TextMeshPro midPointText;
    
    [Header("设置")]
    public string distanceUnit = "cm";
    public int decimalPlaces = 2; 
    public bool showLine = true; 
    public bool verticalText = true; // 文本垂直于地面
    public float textOffset = 0.02f; // 文本偏离连线的距离 
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
            
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
            
        // 如果没有LineRenderer组件，自动创建一个
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        // 配置LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = Color.white;
        }
        
        // 获取用户设置的TextObject上的TextMeshPro组件
        if (TextObject != null)
        {
            midPointText = TextObject.GetComponent<TextMeshPro>();
            if (midPointText == null)
            {
                Debug.LogError("TextObject上没有找到TextMeshPro组件！");
            }
        }
        else
        {
            Debug.LogError("请在Inspector中设置TextObject！");
        }
    }

    void Update()
    {
        // 检查必要的组件是否存在
        if (StartA == null || EndA == null || midPointText == null)
            return;
            
        UpdateRuler();
    }
    
    void OnDestroy()
    {
        // 不需要清理文本对象，因为使用的是用户提供的TextObject
    }
    
    void UpdateRuler()
    {
        // 计算距离
        Vector3 startPos = StartA.transform.position;
        Vector3 endPos = EndA.transform.position;
        float distance = Vector3.Distance(startPos, endPos);
        
        distance = ConvertDistanceUnit(distance);
        
        // 计算中点位置
        Vector3 midPoint = (startPos + endPos) / 2f;
        
        // 计算连线方向和垂直偏移
        Vector3 lineDirection = (endPos - startPos).normalized;
        Vector3 upDirection = Vector3.up;
        Vector3 offsetDirection = Vector3.Cross(lineDirection, upDirection).normalized;
        
        // 将文本位置偏移到连线旁边
        Vector3 textPosition = midPoint + offsetDirection * textOffset;
        
        // 更新文本位置
        if (TextObject != null)
        {
            TextObject.transform.position = textPosition;
        }
        
        // 合并数字和单位显示
        string distanceText = distance.ToString("F" + decimalPlaces) + distanceUnit;
        midPointText.text = distanceText;
        
        // 设置文本垂直于地面
        if (verticalText && TextObject != null)
        {
            Quaternion verticalRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            TextObject.transform.rotation = verticalRotation;
        }
        
        // 更新线条渲染器
        if (lineRenderer != null && showLine)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }
        else if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// 获取当前测量的距离
    /// </summary>
    /// <returns>距离值（根据设置的单位）</returns>
    public float GetDistance()
    {
        if (StartA == null || EndA == null)
            return 0f;
            
        float distance = Vector3.Distance(StartA.transform.position, EndA.transform.position);
        
        // 转换为设置的单位
        distance = ConvertDistanceUnit(distance);
        
        return distance;
    }
    
    /// <summary>
    /// 设置测量的起始点和结束点
    /// </summary>
    /// <param name="startPoint">起始点</param>
    /// <param name="endPoint">结束点</param>
    public void SetMeasurePoints(GameObject startPoint, GameObject endPoint)
    {
        StartA = startPoint;
        EndA = endPoint;
    }
    
    /// <summary>
    /// 设置距离单位
    /// </summary>
    /// <param name="unit">单位字符串</param>
    public void SetDistanceUnit(string unit)
    {
        distanceUnit = unit;
    }
    
    /// <summary>
    /// 设置文本颜色
    /// </summary>
    /// <param name="color">文本颜色</param>
    public void SetTextColor(Color color)
    {
        textColor = color;
        if (midPointText != null)
        {
            midPointText.color = color;
        }
    }
    
    /// <summary>
    /// 设置文本大小
    /// </summary>
    /// <param name="size">文本大小</param>
    public void SetTextSize(float size)
    {
        textSize = size;
        if (TextObject != null)
        {
            TextObject.transform.localScale = Vector3.one * size;
        }
        // 也可以直接调整TextMeshPro的字体大小
        if (midPointText != null)
        {
            midPointText.fontSize = midPointText.fontSize * size;
        }
    }
    
    /// <summary>
    /// 设置小数位数
    /// </summary>
    /// <param name="places">小数位数</param>
    public void SetDecimalPlaces(int places)
    {
        decimalPlaces = Mathf.Clamp(places, 0, 6);
    }
    

    
    /// <summary>
    /// 将米为单位的距离转换为指定单位
    /// </summary>
    /// <param name="distanceInMeters">以米为单位的距离</param>
    /// <returns>转换后的距离</returns>
    private float ConvertDistanceUnit(float distanceInMeters)
    {
        switch (distanceUnit.ToLower())
        {
            case "mm":
                return distanceInMeters * 1000f;
            case "cm":
                return distanceInMeters * 100f;
            case "m":
                return distanceInMeters;
            case "km":
                return distanceInMeters / 1000f;
            case "in":
                return distanceInMeters * 39.3701f; // 英寸
            case "ft":
                return distanceInMeters * 3.28084f; // 英尺
            default:
                return distanceInMeters; // 默认返回米
        }
    }
}
