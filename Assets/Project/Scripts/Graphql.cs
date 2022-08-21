using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Defective.JSON;
using TMPro;
using Oculus.Interaction.HandGrab;

public class Graphql : MonoBehaviour
{
    public GraphApi lensReference;
    public Transform spawnPoint;
    public float distanceBetweenFrames;
    public SetTexture[] posts;
    public Transform parent;
    public GameObject postPrefab;
    public TextMeshProUGUI currentURL;
    private GameObject tmpPost;
    private int index;
    private Texture2D currentTexture;
    public static Graphql Instance;
    public OVRHand leftHand;
    public OVRHand rightHand;
    public bool hoverPost;
    public GameObject grabbable;
    public HandGrabInteractor leftIntereactor;
    public HandGrabInteractor rightInteractor;
    private GameObject grabbaleTmp;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        getProfile();
        explorePublications();

    }

    public void OnSpawn()
    {
        currentURL.color = Color.green;
        //if (hoverPost)
        //{
        //    grabbaleTmp =  Instantiate(grabbable);
        //    rightInteractor.SetInteractableSelected(grabbaleTmp.GetComponentsInChildren<HandGrabInteractable>()[0]);
        //}
    }
    public void OnDesSpawn()
    {
        currentURL.color = Color.white;
    }
    //private void Update()
    //{
    //    if (leftHand.IsSystemGestureInProgress && leftHand.IsDominantHand)
    //    {
    //        if (leftHand.IsPointerPoseValid)
    //            currentURL.color = Color.green;
    //        else
    //            currentURL.color = Color.white;
    //    }
    //    if (rightHand.IsSystemGestureInProgress && rightHand.IsDominantHand)
    //    {
    //        if (rightHand.IsPointerPoseValid)
    //            currentURL.color = Color.green;
    //        else
    //            currentURL.color = Color.white;
    //    }
    //}
    public async void getProfile() {
        GraphApi.Query query = lensReference.GetQueryByName("Profile", GraphApi.Query.Type.Query);
        query.SetArgs(new { request = new { profileId = "0x02" } });
        UnityWebRequest request = await lensReference.Post(query);
        //UnityWebRequest request = await lensReference.Post("getProfile", GraphApi.Query.Type.Query);
        string data = request.downloadHandler.text;
        Debug.Log(data);
    }

    public async void explorePublications() {
        //GraphApi.Query query = lensReference.GetQueryByName("ExplorePublications", GraphApi.Query.Type.Query);
        //query.SetArgs(new{request = new{sortCriteria = "LATEST_CREATED"}});
        #region QUERY
        UnityWebRequest request = await lensReference.Post(@"query ExplorePublications {
  explorePublications(request: {
    sortCriteria: TOP_COMMENTED,
    publicationTypes: [POST, COMMENT, MIRROR],
    limit: 50
  }) {
    items {
      __typename 
      ... on Post {
        ...PostFields
      }
    }
    pageInfo {
      prev
      next
      totalCount
    }
  }
}

fragment MediaFields on Media {
  url
  width
  height
  mimeType
}


fragment MetadataOutputFields on MetadataOutput {
  name
  description
  content
  media {
    original {
      ...MediaFields
    }
    small {
      ...MediaFields
    }
    medium {
      ...MediaFields
    }
  }
  attributes {
    displayType
    traitType
    value
  }
}


fragment PostFields on Post {
  id
  metadata {
    ...MetadataOutputFields
  }
  createdAt
  referenceModule {
    ... on FollowOnlyReferenceModuleSettings {
      type
    }
  }
  appId
  hidden
  reaction(request: null)
  mirrors(by: null)
  hasCollectedByMe
}");
        #endregion
        string data = request.downloadHandler.text;
        JSONObject json = new JSONObject(data);
        JSONObject items = json.GetField("data").GetField("explorePublications").GetField("items");
        index = 0;
        for (int i = 0; i < items.list.Count; i++)
        {
            JSONObject item = items.list[i];
            JSONObject id = item.GetField("id");
            JSONObject pictureUrl;
            JSONObject contentElement;
            try {
                pictureUrl = item.GetField("metadata").GetField("media")[0].GetField("original").GetField("url");
            } catch (System.Exception) {
                //Debug.Log("No picture");
                continue;
            }
            try
            {
                contentElement = item.GetField("metadata").GetField("content");
            }
            catch (System.Exception)
            {
                //Debug.Log("No description");
                continue;
            }

            Debug.Log(id.stringValue);
            Debug.Log(pictureUrl);
            Vector3 newPosition = spawnPoint.localPosition + new Vector3(index * distanceBetweenFrames, 0, 0);

            tmpPost = Instantiate(postPrefab, newPosition, Quaternion.identity);
            tmpPost.transform.parent = parent;
            tmpPost.transform.localPosition = newPosition;
            tmpPost.transform.localScale = Vector3.one;
            tmpPost.transform.localRotation = Quaternion.identity;
            tmpPost.GetComponent<SetTexture>().SetURL(pictureUrl.stringValue);
            tmpPost.GetComponent<SetTexture>().SetPost(contentElement.stringValue);
            index++;
        }
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(index * distanceBetweenFrames, 3404);
        Debug.Log(data);
    }

    public void SetCurrentPost(Texture2D sprite, string url)
    {
        currentURL.text = url;
        currentTexture = sprite;
        hoverPost = true;
    }

    public void OurPointer()
    {
        hoverPost = false;
    }
}
