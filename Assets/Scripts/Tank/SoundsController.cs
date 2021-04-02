using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;

public class SoundsController : MonoBehaviour
{
    private AudioService audioService;

    [Inject]
    public void Init(AudioService audioService)
    {
        this.audioService = audioService;
    }

    private void Awake()
    {
        var audioSource = GetComponent<AudioSource>();
        audioService.Audio = audioSource;
    }
}
