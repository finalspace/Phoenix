using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class FadeInOut : MonoBehaviour
{
    [Header("Config Data")]
    public bool includeChildren = false;
    public float time = 1.5f;
    public float delay;
    public Ease easeType = Ease.Linear;
    public bool pingPong = false;

    private bool fadeIn = false;
    private float valueFrom;
    private float valueTo;
    private Action onComplete;

    public List<SpriteRenderer> spritesToFade = new List<SpriteRenderer>();
    private List<MeshRenderer> meshRenderersToFade = new List<MeshRenderer>();
    private List<TMP_Text> TMPToFade = new List<TMP_Text>();
    private bool playing = false;
    private float progress;

    private void Update()
    {
        if (!playing)
            return;

        progress = Mathf.Clamp01(progress);

        foreach (SpriteRenderer sprite in spritesToFade)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, progress);

        foreach (MeshRenderer rend in meshRenderersToFade)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.GetColor("_Color");
                    mat.SetColor("_Color", new Color(color.r, color.g, color.b, progress));
                }
            }

        }

        foreach (TMP_Text t in TMPToFade)
            t.color = new Color(t.color.r, t.color.g, t.color.b, progress);
    }

    public void Init(bool fadeIn, float time, bool includeChildren, Action onComplete, bool autoPlay = true)
    {
        this.fadeIn = fadeIn;
        if (fadeIn)
        {
            valueFrom = 0;
            valueTo = 1.15f;
        }
        else
        {
            valueFrom = 1;
            valueTo = -0.15f;
        }
        this.time = time;
        this.includeChildren = includeChildren;
        this.onComplete = onComplete;

        //sprites
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            spritesToFade.Add(sprite);
        if (includeChildren)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sp in sprites)
                spritesToFade.Add(sp);
        }

        //spine materials
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderersToFade.Add(meshRenderer);
        }
        if (includeChildren)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer rend in renderers)
                meshRenderersToFade.Add(rend);
        }

        //textmesh pro
        TMP_Text tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
            TMPToFade.Add(tmpText);
        if (includeChildren)
        {
            TMP_Text[] tmpTextx = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text t in tmpTextx)
                TMPToFade.Add(t);
        }

        if (autoPlay)
            Play();
    }

    public void InstantChage(bool fadeIn, bool includeChildren, Action onComplete)
    {
        this.fadeIn = fadeIn;
        this.includeChildren = includeChildren;
        this.onComplete = onComplete;

        //sprites
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            spritesToFade.Add(sprite);
        if (includeChildren)
        {
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sp in sprites)
                spritesToFade.Add(sp);
        }

        //spine materials
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderersToFade.Add(meshRenderer);
        }
        if (includeChildren)
        {
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer rend in renderers)
                meshRenderersToFade.Add(rend);
        }

        //textmesh pro
        TMP_Text tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
            TMPToFade.Add(tmpText);
        if (includeChildren)
        {
            TMP_Text[] tmpTextx = GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text t in tmpTextx)
                TMPToFade.Add(t);
        }

        //apply instant change
        progress = fadeIn ? 1 : 0;

        foreach (SpriteRenderer sp in spritesToFade)
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, progress);

        foreach (MeshRenderer rend in meshRenderersToFade)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color color = mat.GetColor("_Color");
                    mat.SetColor("_Color", new Color(color.r, color.g, color.b, progress));
                }
            }

        }

        foreach (TMP_Text t in TMPToFade)
            t.color = new Color(t.color.r, t.color.g, t.color.b, progress);

        if (onComplete != null)
            onComplete();
    }

    public void Play()
    {
        progress = valueFrom;
        if (pingPong)
            DOTween.To(() => progress, x => progress = x, valueTo, time).SetDelay(delay).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
        else DOTween.To(() => progress, x => progress = x, valueTo, time).SetDelay(delay).SetEase(easeType).OnComplete(OnFinish);

        playing = true;
    }

    public void OnFinish()
    {
        playing = false;
        if (onComplete != null)
            onComplete();
    }

    public void DestroySelf()
    {
        Destroy(this);
    }
}