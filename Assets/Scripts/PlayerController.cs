using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public Image youDiedBG;  // 死亡时显示的背景图片

    [Header("交互对象")]
    public Transform dropTrigger; // 放下物体的触发区域
    public GameObject showAfterCats; // 所有猫放置完成后显示的物体
    private GameObject carriedObject; // 当前举起的物体
    private bool hasDroppedCat; // 是否已经放下过猫
    private Vector3 carryOffset = new Vector3(0, 1f, 0); // 举起物体时的偏移量

    [Header("猫猫设置")]
    public int totalCatsRequired = 1; // 需要放置的猫的总数
    public float swayAngle = 10f; // 猫晃动的角度范围
    public float swaySpeed = 2f; // 猫晃动的速度
    private HashSet<GameObject> placedCats = new HashSet<GameObject>(); // 已放置的猫的集合

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDash = true;
    private bool isDashing;
    private float dashTimeLeft;
    private float dashCooldownTimer;
    private int facingDirection = 1;
    private Vector2 respawnPoint;
    private CheckpointTextManager checkpointTextManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 3f; // 设置合适的重力缩放
        respawnPoint = transform.position;
        // 获取CheckpointTextManager组件
        checkpointTextManager = FindObjectOfType<CheckpointTextManager>();
        // 确保死亡背景一开始是隐藏的
        if (youDiedBG != null)
        {
            youDiedBG.gameObject.SetActive(false);
            youDiedBG.canvasRenderer.SetAlpha(0f);
        }
    }

    private IEnumerator FadeInDeathScreen()
    {
        youDiedBG.gameObject.SetActive(true);
        youDiedBG.CrossFadeAlpha(1f, 1f, true);
        yield return new WaitForSeconds(1f);
    }

    private void HideDeathScreen()
    {
        if (youDiedBG != null)
        {
            youDiedBG.CrossFadeAlpha(0f, 0.5f, true);
            youDiedBG.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDashing)
        {
            HandleDashing();
            return;
        }

        // 更新猫的晃动
        UpdateCatSway();

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Update facing direction
        if (moveInput != 0)
        {
            facingDirection = (int)Mathf.Sign(moveInput);

            // 更新举起物体的朝向
            if (carriedObject != null)
            {
                Vector3 localPos = carriedObject.transform.localPosition;
                localPos.x = 0; // 保持在玩家正上方
                carriedObject.transform.localPosition = localPos;
            }
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartDash();
        }

        // Handle dash cooldown
        if (!canDash && !isDashing)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
            }
        }
    }

    private void HandleDashing()
    {
        dashTimeLeft -= Time.deltaTime;
        rb.velocity = new Vector2(dashSpeed * facingDirection, 0);

        if (dashTimeLeft <= 0)
        {
            isDashing = false;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 3f; // 恢复重力并设置合适的重力缩放
        }
    }

    private void UpdateCatSway()
    {
        if (placedCats.Count >= totalCatsRequired)
        {
            // 使用Sin函数来创建摆动效果
            float swayValue = Mathf.Sin(Time.time * swaySpeed) * swayAngle;

            // 对所有已放置的猫进行摆动
            foreach (GameObject cat in placedCats)
            {
                Vector3 rotation = cat.transform.eulerAngles;
                rotation.z = swayValue;
                cat.transform.eulerAngles = rotation;
            }
        }
    }

    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;
        rb.gravityScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            Die();
        }
        else if (other.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform.position;
            // 触发检查点文本显示
            if (checkpointTextManager != null)
            {
                checkpointTextManager.ShowCheckpointText(other.gameObject.name);
            }
        }
        else if (other.CompareTag("Cat") && carriedObject == null && !placedCats.Contains(other.gameObject))
        {
            // 举起猫
            carriedObject = other.gameObject;
            carriedObject.transform.parent = transform;
            carriedObject.transform.localPosition = carryOffset;
            // 让猫横躺
            Vector3 rotation = carriedObject.transform.eulerAngles;
            rotation.z = 90f;
            carriedObject.transform.eulerAngles = rotation;
        }
        else if (other.transform == dropTrigger && carriedObject != null)
        {
            // 放下猫
            carriedObject.transform.parent = null;
            float randomX = dropTrigger.position.x + Random.Range(-4f, 4f);
            Vector3 dropPosition = new Vector3(randomX, dropTrigger.position.y - 1.4f, dropTrigger.position.z);
            carriedObject.transform.position = dropPosition;
            // 让猫恢复竖直状态
            Vector3 rotation = carriedObject.transform.eulerAngles;
            rotation.z = 0f;
            carriedObject.transform.eulerAngles = rotation;
            if (carriedObject.CompareTag("Cat"))
            {
                // 添加到已放置的猫集合中
                placedCats.Add(carriedObject);

                // 检查是否所有猫都已放置
                if (placedCats.Count >= totalCatsRequired)
                {
                    // 所有猫都放置完成后，设置hasDroppedCat为true
                    hasDroppedCat = true;
                    // 隐藏放置触发器
                    dropTrigger.gameObject.SetActive(false);
                    // 显示完成后的物体
                    if (showAfterCats != null)
                    {
                        showAfterCats.SetActive(true);
                    }
                }
            }
            carriedObject = null;
        }
    }

    private void Die()
    {
        // 禁用输入和物理效果
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        enabled = false;

        // 显示死亡画面
        StartCoroutine(FadeInDeathScreen());

        // 通知GameManager处理死亡
        GameManager.Instance.OnPlayerDeath();
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.isKinematic = false;
        enabled = true;
        isDashing = false;
        canDash = true;

        // 隐藏死亡画面
        HideDeathScreen();
    }
}