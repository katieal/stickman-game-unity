using Combat;
using Items;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Player;
using EventChannel;
using System.Collections;
using System.Threading.Tasks;

public class TestScript : MonoBehaviour
{
    private InputActionMap map;
    private InputAction action1, action2, action3, action4;

    [Title("Temp Testing")]
    public AudioSource Audio;

    [Button]
    public void PlayOneshotAudio(int id)
    {
        AudioSource.PlayClipAtPoint(Managers.AudioData.GetSoundEffect(id), transform.position);
    }

    [Button]
    public void PlayFootsteps()
    {
        
        Audio.Play();
    }

    [Title("Spawn Dummy")]
    public GameObject _prefab;
    public Scenes.SpawnPoint _spawnPoint;
    public Transform _spawn;

    [Button]
    public void SpawnMob()
    {
        GameObject mob = Instantiate(_prefab, _spawnPoint.Spawn.position, Quaternion.identity);
    }

    [Button]
    public void SpawnMobOnTransform()
    {
        GameObject mob = Instantiate(_prefab, _spawn.position, Quaternion.identity);
    }


}
