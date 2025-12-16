using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UISlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{

    public Image slotImage;
    public GameObject electronInSlot;
    public int amount = 0;
    public TextMeshProUGUI quantityText;
    public int slotID;
    public int shellID;
    public bool isHotbarSlot = true;

    public GameObject draggingElectronPrefab;
    public StatsWindow statsWindow;
    protected Transform canvas;
    GameObject draggingElectron;
    PlayerElectronController playerElectronController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        UpdateSlotImage();
        canvas = GameObject.Find("Canvas").transform;
        playerElectronController = GameObject.FindWithTag("Player").GetComponent<PlayerElectronController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (electronInSlot == null || !gameObject.activeSelf)
        {
            statsWindow.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log(electronInSlot);
        if (electronInSlot == null) return;
        Electron electronScript = electronInSlot.GetComponent<Electron>();
        statsWindow.updateStats(electronScript);
        statsWindow.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Debug.Log("EXIT");
        if (electronInSlot == null) return;
        Electron electronScript = electronInSlot.GetComponent<Electron>();
        statsWindow.updateStats(electronScript);
        statsWindow.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(draggingElectron == null) return;
        draggingElectron.GetComponent<RectTransform>().anchoredPosition = eventData.position / canvas.GetComponent<Canvas>().scaleFactor;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if(electronInSlot == null || amount <= 0) return;

        draggingElectron = Instantiate(draggingElectronPrefab, transform.position, Quaternion.identity);
        draggingElectron.GetComponent<UISlot>().electronInSlot = electronInSlot;
        Debug.Log(draggingElectron.GetComponent<UISlot>().electronInSlot);
        draggingElectron.GetComponent<UISlot>().amount = 1;
        draggingElectron.GetComponent<UISlot>().UpdateSlotImage();

        if (isHotbarSlot)
        {
            playerElectronController.RemoveElectronFromBuild(shellID, slotID);
            Debug.Log(draggingElectron.GetComponent<UISlot>().electronInSlot);
        } else {
            playerElectronController.RemoveElectronFromInventory(electronInSlot);
        }

        draggingElectron.transform.SetParent(canvas, false);
        draggingElectron.GetComponent<RectTransform>().anchoredPosition = eventData.position / canvas.GetComponent<Canvas>().scaleFactor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isHotbarSlot) return;

        playerElectronController.RemoveElectronFromInventory(electronInSlot);
        playerElectronController.PickUpElectron(electronInSlot);
        if (amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var hit in results)
        {
            UISlot slot = hit.gameObject.GetComponent<UISlot>(); // The slot we are dropping into
            if (slot != null && slot.isHotbarSlot) // ALWAYS dropping into hotbar slots
            {
                if (playerElectronController.InsertElectronToBuild(slot.shellID, slot.slotID, draggingElectron.GetComponent<UISlot>().electronInSlot)) { // If there is nothing there
                    Destroy(draggingElectron);
                    if (!isHotbarSlot && amount <= 0)
                    {
                        Destroy(gameObject);
                    }
                    Debug.Log("Inserted into empty hotbar slot");
                    return;
                } else { // There is something in there
                    if (isHotbarSlot) // Coming from hotbar?
                    {
                        // Swap them
                        playerElectronController.InsertElectronToBuild(shellID, slotID, slot.electronInSlot);
                        playerElectronController.RemoveElectronFromBuild(slot.shellID, slot.slotID);
                        playerElectronController.InsertElectronToBuild(slot.shellID, slot.slotID, draggingElectron.GetComponent<UISlot>().electronInSlot);

                        Destroy(draggingElectron);
                        Debug.Log("Swapped hotbar slots");
                        return;
                    } else { // Coming from inventory then
                        playerElectronController.AddElectronToInventory(slot.electronInSlot);
                        playerElectronController.RemoveElectronFromBuild(slot.shellID, slot.slotID);
                        playerElectronController.InsertElectronToBuild(slot.shellID, slot.slotID, draggingElectron.GetComponent<UISlot>().electronInSlot);
                        Destroy(draggingElectron);
                        if (amount <= 0)
                        {
                            Destroy(gameObject);
                        }
                        Debug.Log("Swapped inventory and hotbar slots");
                        return;
                    }
                }
            }
        }

        // We did not find anywhere to put it
        Debug.Log("Returned electron to inventory");
        playerElectronController.AddElectronToInventory(draggingElectron.GetComponent<UISlot>().electronInSlot);
        Destroy(draggingElectron);
        if (!isHotbarSlot && amount <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateSlotImage()
    {
        if(electronInSlot == null || amount <= 0)
        {
            slotImage.gameObject.SetActive(false);
        } else {
            slotImage.gameObject.SetActive(true);
            slotImage.sprite = electronInSlot.GetComponent<SpriteRenderer>().sprite;

            if (quantityText == null) return;

            if(amount <= 1)
            {
                quantityText.text = "";
                return;
            }
            quantityText.text = "x" + amount.ToString();
        }
    }
}
