using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardItemView : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text speciesText;
    public RawImage rawImage;

    public void SetData(string name, string species)
    {
        if (nameText) nameText.text = name;
        if (speciesText) speciesText.text = species;
    }

    public void SetImage(Texture2D tex)
    {
        if (!rawImage) return;
        rawImage.texture = tex;
        rawImage.color = Color.white;
    }
}