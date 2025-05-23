﻿using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private float m_CurrentHealth;
    private bool m_Dead;
    private GameManager m_GameManager;

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        m_ExplosionParticles.gameObject.SetActive(false);
        m_GameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }
    
    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        SetHealthUI();

        if(m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);
    }

    private void OnDeath()
    {
        m_Dead = true;
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // If this is an AI tank, notify the GameManager
        if (GetComponent<TankAI>() != null && m_GameManager != null)
        {
            m_GameManager.OnAITankDestroyed();
        }

        gameObject.SetActive(false);
    }
}