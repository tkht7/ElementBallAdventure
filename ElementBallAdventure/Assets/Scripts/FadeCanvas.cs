using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : MonoBehaviour
{

    [System.NonSerialized]
    public bool fadeIn = false;
    [System.NonSerialized]
    public bool fadeOut = false;

    [SerializeField]
    private Image panelImage;

    float red, green, blue, alpha;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        //元の色を取得
        red = panelImage.color.r;
        green = panelImage.color.g;
        blue = panelImage.color.b;
        alpha = panelImage.color.a;
    }
    
    void Update()
    {
        if (fadeIn)
            FadeIn();
        else if (fadeOut)
            FadeOut();
    }

    //フェードイン
    void FadeIn()
    {
        alpha += Time.deltaTime;
        SetAlpha();
        if (alpha >= 1)
        {
            fadeIn = false;
        }
    }

    //フェードアウト
    void FadeOut()
    {
        alpha -= Time.deltaTime;
        SetAlpha();
        if (alpha <= 0)
        {
            fadeOut = false;
            Destroy(gameObject);
        }
    }

    //透明度を変更
    void SetAlpha()
    {
        panelImage.color = new Color(red, green, blue, alpha);
    }
}