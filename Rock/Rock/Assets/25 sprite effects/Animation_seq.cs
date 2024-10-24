using UnityEngine;
using System.Collections;

public class Animation_seq: MonoBehaviour
{
    public float fps = 24.0f;
    public Texture2D[] frames;

    private int frameIndex;
    private MeshRenderer rendererMy;

    public bool DestroyAfterDone;
    public bool FollowExactly;

    private Transform PlayerTransform;

    public float offsetY;
    public float offsetX;

    void Start()
    {
        PlayerTransform = FindObjectOfType<PlayerMovement>().gameObject.transform;
        rendererMy = GetComponent<MeshRenderer>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }

    private void Update()
    {
        if(PlayerTransform != null && !FollowExactly)
        {
            transform.position = new Vector3(PlayerTransform.position.x, transform.position.y, transform.position.z);
        }else if (FollowExactly)
        {
            transform.position = new Vector3(PlayerTransform.position.x - offsetX, PlayerTransform.position.y - offsetY, PlayerTransform.position.z);
        }
    }

    void NextFrame()
    {
        rendererMy.sharedMaterial.SetTexture("_MainTex", frames[frameIndex]);
        frameIndex = (frameIndex + 0001) % frames.Length;

        if(DestroyAfterDone && frameIndex >= frames.Length - 1)
        {
            Destroy(gameObject);
        }
    }
}