using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Header("Laser Value")]
    public float LaserWidth = 1.0f;
    public float MaxLength = 50.0f;
    public Color LaserColor = Color.red;

    [Header("Mirror Value")]
    public Color MirrorNormalColor;
    public Color MirrorHitColor;

    [Header("Mirror Object")]
    public Mirror[] LevelMirror;
    public List<Mirror> ConnectedMirror = new List<Mirror>();

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        LevelMirror = FindObjectsOfType<Mirror>();
    }

    public void AddConnectedMirror(Mirror _mirror)
    {
        bool NotHaveTheConnetedMirror = true;
        for (int i = 0; i < ConnectedMirror.Count; i++)
        {
            if(ConnectedMirror[i].id == _mirror.id)
            {
                NotHaveTheConnetedMirror = false;
            }
        }

        if (NotHaveTheConnetedMirror)
        {
            ConnectedMirror.Add(_mirror);
        }
    }

    public void RemoveConnectedMirror(Mirror _mirror)
    {
        ConnectedMirror.Remove(_mirror);
    }

    public bool AllMirrorHasConnected()
    {
        bool _connected = false;

        if (ConnectedMirror.Count == LevelMirror.Length)
            _connected = true;

        return _connected;
    }
}
