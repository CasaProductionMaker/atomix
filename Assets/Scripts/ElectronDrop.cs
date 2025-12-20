using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ElectronDrop : NetworkBehaviour
{
    public NetworkVariable<int> electronDropID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector2 spreadDirection;
    public SpriteRenderer electronImage;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * 2f;
        UpdateImage();
    }

    public void UpdateImage()
    {
        LangObject langObject = GameObject.Find("LangObject").GetComponent<LangObject>();
        electronImage.sprite = langObject.electronsInGame[electronDropID.Value].GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        StayInBounds();
        UpdateImage();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PickedUpServerRpc()
    {
        Destroy(gameObject);
    }

    public void StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize;
        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);
        transform.position = pos;
    }
}
