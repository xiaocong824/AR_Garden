using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GirdingGuideManager : MonoBehaviour
{
    [Header("Plants")]
    [SerializeField] private GameObject PlantA;
    [SerializeField] private GameObject PlantB;
    [SerializeField] private GameObject PlantA_child;
    [SerializeField] private GameObject PlantB_child;

    [Header("Tools")]
    [SerializeField] private GameObject Pipe;
    [SerializeField] private GameObject Pipe_Beacon;
    [SerializeField] private GameObject Knife;
    [SerializeField] private GameObject Knife_Beacon;
    [SerializeField] private GameObject Clip;

    [Header("Working Zone")]
    [SerializeField] private GameObject WorkingZone;
    [SerializeField] private Image WorkingZoneImage; // WorkingZone的Image组件
    [SerializeField] private SpriteRenderer Tag_zhenmu;
    [SerializeField] private SpriteRenderer Tag_jiesumu;

    [Header("UI Sprite")]
    [SerializeField] private Sprite green_workingzone;
    [SerializeField] private Sprite red_workingzone;
    [SerializeField] private Sprite Process_list_ICON_normal;
    [SerializeField] private Sprite Process_list_ICON_yellow;
    [SerializeField] private Sprite Process_list_normal;
    [SerializeField] private Sprite Process_list_yellow;
    [SerializeField] private Sprite Tag_normal;
    [SerializeField] private Sprite Tag_yellow;

    [Header("Process UI")]
    [SerializeField] private GameObject[] Process_list;
    [SerializeField] private GameObject[] Process_list_ICON;

    [Header("Audio")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    private AudioSource audioSource;

    [Header("Angle Check Settings")]
    [SerializeField] private float positionTolerance = 0.1f; // 位置容差（米）
    [SerializeField] private float angleTolerance = 5f; // 角度容差（度）
    
    // 当前步骤
    private int currentStep = 0;
    private bool stepCompleted = false;

    // 碰撞检测相关
    private HashSet<GameObject> objectsInWorkingZone = new HashSet<GameObject>();
    private WorkingZoneDetector workingZoneDetector;

    void Start()
    {
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 设置WorkingZone检测器
        SetupWorkingZoneDetector();

        // 初始化UI状态
        InitializeUI();
    }

    void Update()
    {
        // 检查当前步骤的完成条件
        CheckCurrentStepCompletion();

        // 更新Beacon的视觉反馈
        if (!stepCompleted)
        {
            switch (currentStep)
            {
                case 1: // 第二步：刀具匹配反馈
                    if (Knife != null && Knife_Beacon != null)
                    {
                        UpdateBeaconVisualFeedback(Knife);
                    }
                    break;
                case 2: // 第三步：导管位置反馈
                    if (Pipe != null && Pipe_Beacon != null)
                    {
                        UpdatePipeBeaconFeedback(Pipe);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 设置WorkingZone检测器
    /// </summary>
    private void SetupWorkingZoneDetector()
    {
        if (WorkingZone == null)
        {
            Debug.LogError("WorkingZone未设置！");
            return;
        }

        // 确保WorkingZone有Collider并设置为Trigger
        Collider workingZoneCollider = WorkingZone.GetComponent<Collider>();
        if (workingZoneCollider == null)
        {
            // 如果没有Collider，添加一个BoxCollider
            workingZoneCollider = WorkingZone.AddComponent<BoxCollider>();
            Debug.Log("为WorkingZone添加了BoxCollider");
        }
        workingZoneCollider.isTrigger = true;

        // 添加或获取WorkingZoneDetector组件
        workingZoneDetector = WorkingZone.GetComponent<WorkingZoneDetector>();
        if (workingZoneDetector == null)
        {
            workingZoneDetector = WorkingZone.AddComponent<WorkingZoneDetector>();
        }

        // 设置回调
        workingZoneDetector.Initialize(this);
    }

    /// <summary>
    /// 物体进入WorkingZone时调用
    /// </summary>
    public void OnObjectEnterWorkingZone(GameObject obj)
    {
        objectsInWorkingZone.Add(obj);
        Debug.Log($"{obj.name} 进入了WorkingZone");
    }

    /// <summary>
    /// 物体离开WorkingZone时调用
    /// </summary>
    public void OnObjectExitWorkingZone(GameObject obj)
    {
        objectsInWorkingZone.Remove(obj);
        Debug.Log($"{obj.name} 离开了WorkingZone");
    }

    /// <summary>
    /// 初始化UI状态
    /// </summary>
    private void InitializeUI()
    {
        // 隐藏所有步骤，只显示第一步
        for (int i = 0; i < Process_list.Length; i++)
        {
            if (i == 0)
            {
                Process_list[i].SetActive(true);
                Process_list_ICON[i].SetActive(true);
                // 设置为正常状态
                SetProcessUIState(i, false);
            }
            else
            {
                Process_list[i].SetActive(false);
                Process_list_ICON[i].SetActive(false);
            }
        }

        // 初始化标签状态
        if(Tag_zhenmu != null)
        {
            Tag_zhenmu.sprite = Tag_normal;
        }
        if(Tag_jiesumu != null)
        {
            Tag_jiesumu.sprite = Tag_normal;
        }

        // 设置WorkingZone为初始状态
        if (WorkingZoneImage != null)
            WorkingZoneImage.sprite = red_workingzone;
    }

    /// <summary>
    /// 检查当前步骤的完成条件
    /// </summary>
    private void CheckCurrentStepCompletion()
    {
        if (stepCompleted) return;

        bool conditionMet = false;

        switch (currentStep)
        {
            case 0: // 第一步：将PlantA移动到WorkingZone
                // 高亮当前步骤的标签
                if(Tag_zhenmu != null)
                {
                    Tag_zhenmu.sprite = Tag_yellow;
                }
                conditionMet = IsObjectInWorkingZone(PlantA);
                if(conditionMet)
                {
                    Tag_zhenmu.sprite = Tag_normal;
                }
                break;
            case 1: // 第二步：使用刀具进行切割
                // 显示刀具信标
                if(Knife_Beacon != null)
                {
                    Knife_Beacon.SetActive(true);
                }
                conditionMet = IsObjectInWorkingZone(Knife) && AngleCheck(Knife);
                if(conditionMet)
                {
                    // 完成后隐藏信标
                    Knife_Beacon.SetActive(false);
                }
                break;
            case 2: // 第三步：使用导管
                // 显示导管信标
                if(Pipe_Beacon != null)
                {
                    Pipe_Beacon.SetActive(true);
                }
                // 高亮接穗木标签
                if(Tag_jiesumu != null)
                {
                    Tag_jiesumu.sprite = Tag_yellow;
                }
                conditionMet = IsObjectInWorkingZone(Pipe) && PositionCheck(Pipe);
                if(conditionMet)
                {
                    // 完成后隐藏信标
                    Pipe_Beacon.SetActive(false);
                }
                break;
            case 3: // 第四步：将PlantB移动到WorkingZone
                conditionMet = IsObjectInWorkingZone(PlantB);
                break;
            // 可以继续添加更多步骤
        }

        if (conditionMet)
        {
            CompleteCurrentStep();
        }
    }

    /// <summary>
    /// 检查物体是否在WorkingZone区域内（通过碰撞检测）
    /// </summary>
    private bool IsObjectInWorkingZone(GameObject obj)
    {
        if (obj == null) return false;
        return objectsInWorkingZone.Contains(obj);
    }

    /// <summary>
    /// 检查物体与Beacon的角度和位置是否匹配
    /// </summary>
    private bool AngleCheck(GameObject obj)
    {
        if (obj == null || Knife_Beacon == null) return false;

        // 确保Beacon是激活状态
        if (!Knife_Beacon.activeInHierarchy)
        {
            Knife_Beacon.SetActive(true);
        }

        // 计算位置差距
        Vector3 positionDifference = obj.transform.position - Knife_Beacon.transform.position;
        float distance = positionDifference.magnitude;

        // 计算角度差距
        float angleDifference = Quaternion.Angle(obj.transform.rotation, Knife_Beacon.transform.rotation);

        // 检查是否在容差范围内
        bool positionMatch = distance <= positionTolerance;
        bool angleMatch = angleDifference <= angleTolerance;

        // 调试信息
        Debug.Log($"位置差距: {distance:F3}m (容差: {positionTolerance}m) - {(positionMatch ? "通过" : "未通过")}");
        Debug.Log($"角度差距: {angleDifference:F1}° (容差: {angleTolerance}°) - {(angleMatch ? "通过" : "未通过")}");

        return positionMatch && angleMatch;
    }

    private bool PositionCheck(GameObject obj)
    {
        if (obj == null || Pipe_Beacon == null) return false;

        Vector3 positionDifference = obj.transform.position - Pipe_Beacon.transform.position;

        float distance = positionDifference.magnitude;
        bool positionMatch = distance <= positionTolerance;

        Debug.Log($"pipe位置差距: {distance:F3}m (容差: {positionTolerance}m) - {(positionMatch ? "通过" : "未通过")}");

        return positionMatch;
    }

    /// <summary>
    /// 更新导管信标的视觉反馈
    /// </summary>
    private void UpdatePipeBeaconFeedback(GameObject obj)
    {
        if (obj == null || Pipe_Beacon == null) return;

        // 计算位置匹配程度
        Vector3 positionDifference = obj.transform.position - Pipe_Beacon.transform.position;
        float distance = positionDifference.magnitude;

        // 如果Beacon有Renderer组件，根据距离改变颜色
        Renderer beaconRenderer = Pipe_Beacon.GetComponent<Renderer>();
        if (beaconRenderer != null)
        {
            // 计算匹配度（0-1）
            float positionMatch = Mathf.Clamp01(1f - (distance / positionTolerance));

            // 根据匹配度设置颜色（红色->绿色）
            Color feedbackColor = Color.Lerp(Color.red, Color.green, positionMatch);
            beaconRenderer.material.color = feedbackColor;
        }
    }

    /// <summary>
    /// 更新Beacon的视觉状态（可选，用于给用户提供视觉反馈）
    /// </summary>
    private void UpdateBeaconVisualFeedback(GameObject obj)
    {
        if (obj == null || Knife_Beacon == null) return;

        // 计算匹配程度
        Vector3 positionDifference = obj.transform.position - Knife_Beacon.transform.position;
        float distance = positionDifference.magnitude;
        float angleDifference = Quaternion.Angle(obj.transform.rotation, Knife_Beacon.transform.rotation);

        // 如果Beacon有Renderer组件，可以根据匹配程度改变颜色
        Renderer beaconRenderer = Knife_Beacon.GetComponent<Renderer>();
        if (beaconRenderer != null)
        {
            // 计算匹配度（0-1）
            float positionMatch = Mathf.Clamp01(1f - (distance / positionTolerance));
            float angleMatch = Mathf.Clamp01(1f - (angleDifference / angleTolerance));
            float overallMatch = (positionMatch + angleMatch) / 2f;

            // 根据匹配度设置颜色（红色->黄色->绿色）
            Color feedbackColor = Color.Lerp(Color.red, Color.green, overallMatch);
            beaconRenderer.material.color = feedbackColor;
        }
    }

    /// <summary>
    /// 完成当前步骤
    /// </summary>
    private void CompleteCurrentStep()
    {
        stepCompleted = true;

        // 播放正确音效
        PlaySound(correctSound);

        // 更新WorkingZone UI为绿色
        if (WorkingZoneImage != null)
            WorkingZoneImage.sprite = green_workingzone;

        // 处理当前步骤完成时的特殊逻辑
        HandleStepCompletion(currentStep);

        // 将当前步骤设置为完成状态（黄色）
        SetProcessUIState(currentStep, true);

        // 延迟显示下一步
        StartCoroutine(ShowNextStepAfterDelay(1f));
    }

    /// <summary>
    /// 处理步骤完成时的特殊逻辑
    /// </summary>
    private void HandleStepCompletion(int step)
    {
        switch (step)
        {
            case 0: // 第一步完成：将砧木标签设为正常状态
                if (Tag_zhenmu != null)
                {
                    Tag_zhenmu.sprite = Tag_normal;
                }
                break;
            case 1: // 第二步完成：隐藏刀具信标
                if (Knife_Beacon != null)
                {
                    Knife_Beacon.SetActive(false);
                }
                break;
            case 2: // 第三步完成：隐藏导管信标，将接穗木标签设为正常状态
                if (Pipe_Beacon != null)
                {
                    Pipe_Beacon.SetActive(false);
                }
                if (Tag_jiesumu != null)
                {
                    Tag_jiesumu.sprite = Tag_normal;
                }
                break;
        }
    }

    /// <summary>
    /// 设置流程UI状态
    /// </summary>
    private void SetProcessUIState(int stepIndex, bool isCompleted)
    {
        if (stepIndex >= Process_list.Length || stepIndex >= Process_list_ICON.Length) return;

        Image listImage = Process_list[stepIndex].GetComponent<Image>();
        Image iconImage = Process_list_ICON[stepIndex].GetComponent<Image>();

        if (isCompleted)
        {
            // 完成状态 - 黄色
            if (listImage != null) listImage.sprite = Process_list_yellow;
            if (iconImage != null) iconImage.sprite = Process_list_ICON_yellow;
        }
        else
        {
            // 正常状态
            if (listImage != null) listImage.sprite = Process_list_normal;
            if (iconImage != null) iconImage.sprite = Process_list_ICON_normal;
        }
    }

    /// <summary>
    /// 延迟显示下一步
    /// </summary>
    private IEnumerator ShowNextStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 进入下一步
        currentStep++;
        stepCompleted = false;

        // 重置WorkingZone为红色状态
        if (WorkingZoneImage != null)
            WorkingZoneImage.sprite = red_workingzone;

        // 显示下一步UI（如果存在）
        if (currentStep < Process_list.Length)
        {
            Process_list[currentStep].SetActive(true);
            Process_list_ICON[currentStep].SetActive(true);
            SetProcessUIState(currentStep, false);
        }
        else
        {
            // 所有步骤完成
            OnAllStepsCompleted();
        }
    }

    /// <summary>
    /// 所有步骤完成时调用
    /// </summary>
    private void OnAllStepsCompleted()
    {
        Debug.Log("嫁接过程完成！");
        // 可以在这里添加完成后的逻辑，比如显示完成UI、播放特殊音效等
        PlaySound(correctSound);
        
        // 可以将WorkingZone设置为最终状态
        if (WorkingZoneImage != null)
            WorkingZoneImage.sprite = green_workingzone;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 重置所有步骤（用于重新开始）
    /// </summary>
    public void ResetAllSteps()
    {
        currentStep = 0;
        stepCompleted = false;
        objectsInWorkingZone.Clear();

        // 隐藏所有信标
        if (Knife_Beacon != null) Knife_Beacon.SetActive(false);
        if (Pipe_Beacon != null) Pipe_Beacon.SetActive(false);

        // 重置所有标签状态
        if (Tag_zhenmu != null) Tag_zhenmu.sprite = Tag_normal;
        if (Tag_jiesumu != null) Tag_jiesumu.sprite = Tag_normal;

        InitializeUI();
    }

    /// <summary>
    /// 手动触发步骤完成（用于调试）
    /// </summary>
    [ContextMenu("Complete Current Step")]
    public void DebugCompleteCurrentStep()
    {
        if (!stepCompleted)
            CompleteCurrentStep();
    }
}

/// <summary>
/// WorkingZone检测器，负责检测物体进入和离开
/// </summary>
public class WorkingZoneDetector : MonoBehaviour
{
    private GirdingGuideManager manager;

    public void Initialize(GirdingGuideManager girdingManager)
    {
        manager = girdingManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (manager != null)
        {
            manager.OnObjectEnterWorkingZone(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (manager != null)
        {
            manager.OnObjectExitWorkingZone(other.gameObject);
        }
    }
}
