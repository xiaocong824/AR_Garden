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
    [SerializeField] private GameObject Knife;
    [SerializeField] private GameObject Clip;

    [Header("Working Zone")]
    [SerializeField] private GameObject WorkingZone;
    [SerializeField] private Image WorkingZoneImage; // WorkingZone的Image组件

    [Header("UI Sprite")]
    [SerializeField] private Sprite green_workingzone;
    [SerializeField] private Sprite red_workingzone;
    [SerializeField] private Sprite Process_list_ICON_normal;
    [SerializeField] private Sprite Process_list_ICON_yellow;
    [SerializeField] private Sprite Process_list_normal;
    [SerializeField] private Sprite Process_list_yellow;

    [Header("Process UI")]
    [SerializeField] private GameObject[] Process_list;
    [SerializeField] private GameObject[] Process_list_ICON;

    [Header("Audio")]
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    private AudioSource audioSource;
    
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
                conditionMet = IsObjectInWorkingZone(PlantA);
                break;
            case 1: // 第二步：可以添加其他条件，比如使用工具等
                // 示例：检查是否使用了刀具
                conditionMet = IsObjectInWorkingZone(Knife);
                break;
            case 2: // 第三步
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

        // 将当前步骤设置为完成状态（黄色）
        SetProcessUIState(currentStep, true);

        // 延迟显示下一步
        StartCoroutine(ShowNextStepAfterDelay(1f));
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
