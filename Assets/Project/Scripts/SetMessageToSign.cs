using UnityEngine;
using UnityEngine.Networking;
using GraphQlClient.Core;
using Defective.JSON;
using WalletConnectSharp.Unity;
using UnityEngine.Events;
public class SetMessageToSign : MonoBehaviour
{
    public GraphApi lensReference;
    public string messageToSign;
    public UnityEvent OnChallenge;
    // Start is called before the first frame update
    public void Start()
    {
        GetChallenge();
    }

    public async void GetChallenge()
    {
        GraphApi.Query query = lensReference.GetQueryByName("challenge", GraphApi.Query.Type.Query);
        var signerAddress = WalletConnect.ActiveSession.Accounts[0];
        query.SetArgs(new { request = new { address = signerAddress } });
        UnityWebRequest request = await lensReference.Post(query);
        string data = request.downloadHandler.text;
        JSONObject json = new JSONObject(data);
        messageToSign = json.GetField("data").GetField("challenge").GetField("text").stringValue;
        MessageSigner signer = GameObject.Find("== Sign Message ==").GetComponent<MessageSigner>();
        signer.messageToSign = messageToSign;
        OnChallenge.Invoke();
        Debug.Log(data);
    }
}