using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CheckpointTextManager : MonoBehaviour
{
    public GameObject textPanel; // 包含文本的Panel引用
    public Text checkpointText; // UI Text组件引用
    private float displayDuration = 2f; // 文本显示持续时间
    private HashSet<string> displayedCheckpoints = new HashSet<string>(); // 记录已显示过的检查点

    private void Start()
    {
        // 确保Panel一开始是隐藏的
        // if (textPanel != null)
        // {
        //     textPanel.SetActive(false);
        // }
    }

    public void ShowCheckpointText(string checkpointName)
    {
        // 如果这个检查点已经显示过文本，则直接返回
        if (displayedCheckpoints.Contains(checkpointName) || checkpointText == null) return;

        // 记录这个检查点已经显示过文本
        displayedCheckpoints.Add(checkpointName);

        // 根据检查点名字设置不同的文本
        string displayText = GetTextForCheckpoint(checkpointName);

        // 设置文本内容
        checkpointText.text = displayText;

        // 显示Panel
        textPanel.SetActive(true);

        // 启动协程来处理定时隐藏
        StartCoroutine(HideTextAfterDelay());
    }

    private string GetTextForCheckpoint(string checkpointName)
    {
        // 根据检查点名字返回对应的文本
        // 您可以根据需要添加更多的检查点判断
        switch (checkpointName)
        {
            case "Checkpoint0":
                return "使用A D左右移动，空格跳跃，shift冲刺";
            case "Checkpoint1":
                return "跳到墙上可以再次跳跃";
            case "Checkpoint2":
                return "一共5只猫猫！";
            default:
                return "已到达检查点：" + checkpointName;
        }
    }

    private IEnumerator HideTextAfterDelay()
    {
        // 等待指定时间
        yield return new WaitForSeconds(displayDuration);

        // 隐藏Panel
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }
    }
}