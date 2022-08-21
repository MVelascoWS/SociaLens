using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class SetTexture : MonoBehaviour
{
    public string thumbnailUrl;
    public Image imageSprite;
    public TextMeshProUGUI postText;
    public Button pointerButton;
    public MeshRenderer meshRenderer;
    public Transform grabbale;
    private Texture2D postTexture;
    public void SetURL(string thumbnail)
    {
        thumbnailUrl = thumbnail;
        StartCoroutine("setThumbnailTexture");
    }

    public void SetPost(string textPost)
    {
        postText.text = textPost;
    }
    IEnumerator setThumbnailTexture()
    {
        while (string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("setThumbnailTexture" + thumbnailUrl);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(thumbnailUrl);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        Debug.Log(www.error + " " + www.downloadHandler.text + thumbnailUrl);
        postTexture = DownloadHandlerTexture.GetContent(www);
        Rect spriteRect = new Rect(0,0, postTexture.width, postTexture.height);        
        imageSprite.sprite = Sprite.Create(postTexture, spriteRect, Vector2.one * 0.5f, 100);
        meshRenderer.material.mainTexture = postTexture;
    }

    public void SetCurrentData()
    {
        Graphql.Instance.SetCurrentPost(postTexture, thumbnailUrl);
    }

    public void DetachGrabbable()
    {
        grabbale.transform.parent = null;
    }
}
