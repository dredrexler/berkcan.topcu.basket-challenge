using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 10f;
    private Image image;
    private int frameIndex;
    private float timer;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer > 1f / fps)
        {
            frameIndex = (frameIndex + 1) % frames.Length;
            image.sprite = frames[frameIndex];
            timer = 0f;
        }
    }
}
