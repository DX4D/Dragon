using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMountScript : MonoBehaviour {

    public Text text;
    public float time;
    public Color FadeOut;
    public Color Fadein;

    void Start()
    {
        Fade_Out(0);
    }

    public virtual void ShowMountText(GameObject mount)
    {
        if (mount != null)
        {
            Fade_In(time);
        }
        else
        {
            Fade_Out(time);
        }
    }

    public virtual void Fade_In(float time)
    {
        text.CrossFadeColor(Fadein, time, false, true);
    }

    public virtual void Fade_Out(float time)
    {
        text.CrossFadeColor(FadeOut, time, false, true);
    }

}
