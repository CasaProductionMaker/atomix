using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerElectronController : NetworkBehaviour
{

    // Electrons
    [SerializeField] float electronAngle = 0f;
    [SerializeField] float electronRotationSpeed = 2.5f;
    public List<SpawnedElectron>[] electronsPerShell = new List<SpawnedElectron>[3];
    [SerializeField] int[] maxElectronsPerShell;
    [SerializeField] UISlot[] uiSlots;

    // Inventory
    public Dictionary<GameObject, int> electronInventory = new Dictionary<GameObject, int>();
    public Transform inventoryContainer;
    public GameObject inventoryObject;
    public GameObject inventoryElectronPrefab;

    // References
    public GameObject startingElectronPrefab;
    public Player player;
    public GameObject quantumTunnelEffect;

    // Input
    public InputActionReference attack;
    public InputActionReference defend;
    public InputActionReference toggleInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!IsOwner) return;

        player = GetComponent<Player>();
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            electronsPerShell[i] = new List<SpawnedElectron>();
            for (int j = 0; j < maxElectronsPerShell[i]; j++)
            {
                int slotID = i * 8 + j;
                GameObject slot = GameObject.Find("Slot (" + slotID + ")");
                if (slot != null) uiSlots[slotID] = slot.GetComponent<UISlot>();
                slot.GetComponent<UISlot>().SetPEC(this);
                electronsPerShell[i].Add(new SpawnedElectron(true));
            }
        }
        Debug.Log(GameObject.Find("InventoryContent"));
        inventoryContainer = GameObject.Find("InventoryContent").transform;
        inventoryObject = GameObject.Find("Inventory");
        inventoryObject.SetActive(false);
        AddElectronToBuild(startingElectronPrefab.GetComponent<Electron>().electronID);

        toggleInventory.action.performed += ctx =>
        {
            if (ChatManager.Singleton.isChatSelected()) return;
            inventoryObject.SetActive(!inventoryObject.activeSelf);
        };

        defend.action.performed += ctx =>
        {
            QuantumParticle particle = GetActiveQuantumParticle();
            if (particle == null) return;
            Vector3 myPos = transform.position;
            transform.position = particle.transform.position;
            particle.transform.position = myPos;
            Vector3 direction = particle.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion playerAngle = Quaternion.Euler(0f, 0f, angle);
            Instantiate(quantumTunnelEffect, transform.position, playerAngle);
            playerAngle = Quaternion.Euler(0f, 0f, angle + 180f);
            Instantiate(quantumTunnelEffect, particle.transform.position, playerAngle);
            player.TakeDamageOwnerRpc(particle.selfDamage);
            player.ApplyVelocityOwnerRpc(direction.normalized * -particle.push);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        if (player.getHealth() <= 0f) {
            KillAllElectrons();
            return;
        }

        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;

                if (electronReference.isDead)
                {
                    electronReference.transform.position = transform.position;
                    continue;
                }

                if (electronReference is HeavyElectron heavy)
                {
                    // Shoot it
                    if (Time.time - heavy.timeDied >= heavy.reload + heavy.secondaryReload) {
                        heavy.isDetached = attack.action.inProgress;
                    }
                }

                if (electronReference.isDetached)
                {
                    continue;
                }

                // Specific electron behavior
                if (electronReference is MissileElectron missileElectron)
                {
                    // Rotate it
                    missileElectron.transform.rotation = Quaternion.Euler(0, 0, -(electronAngle + electron.angle) * Mathf.Rad2Deg + 90f);
                    if (attack.action.inProgress && Time.time - missileElectron.timeDied >= missileElectron.reload + missileElectron.secondaryReload) missileElectron.isDetached = true;
                }
                if (electronReference is Taser taser)
                {
                    // Rotate it
                    taser.transform.rotation = Quaternion.Euler(0, 0, -(electronAngle + electron.angle) * Mathf.Rad2Deg + 90f);
                }
                if (electronReference is Blade blade)
                {
                    // Rotate it
                    blade.transform.rotation = Quaternion.Euler(0, 0, -(electronAngle + electron.angle) * Mathf.Rad2Deg);
                }
                if (electronReference is PiercingElectron piercing)
                {
                    // Rotate it
                    piercing.transform.rotation = Quaternion.Euler(0, 0, -(electronAngle + electron.angle + 90) * Mathf.Rad2Deg);
                }
                if (electronReference is Saw saw)
                {
                    // Shoot it
                    if (attack.action.inProgress && Time.time - saw.timeDied >= saw.reload + saw.secondaryReload) {
                        saw.isDetached = true;
                        Vector2 fireDirection = new Vector2((float)Math.Sin(electronAngle + electron.angle), (float)Math.Cos(electronAngle + electron.angle));
                        saw.Fire(fireDirection);
                    }
                }
                if (electronReference is Nucleus nucleus)
                {
                    // Shoot it
                    if (attack.action.inProgress && Time.time - nucleus.timeDied >= nucleus.reload + nucleus.secondaryReload) {
                        nucleus.isDetached = true;
                    }
                }
                if (electronReference is Neutralizer neutralizer)
                {
                    // Shoot it
                    if (attack.action.inProgress && Time.time - neutralizer.timeDied >= neutralizer.reload + neutralizer.secondaryReload) {
                        neutralizer.isDetached = true;
                    }
                }
                if (electronReference is QuantumParticle particle)
                {
                    // Shoot it
                    if (attack.action.inProgress && Time.time - particle.timeDied >= particle.reload + particle.secondaryReload) {
                        particle.isDetached = true;
                    }
                }
                if (electronReference is Mine mine)
                {
                    // Shoot it
                    if (attack.action.inProgress && Time.time - mine.timeDied >= mine.reload + mine.secondaryReload) {
                        mine.isDetached = true;
                    }
                }

                float targetAngle = electronAngle + electron.angle;
                float targetDistance = electron.distance * (attack.action.inProgress && electron.electronReference.doesExpand ? 1.5f : 1) * (defend.action.inProgress ? 0.7f : 1);
                Vector2 targetPosition = new Vector2((float)Math.Sin(targetAngle) * targetDistance, (float)Math.Cos(targetAngle) * targetDistance) + (Vector2)transform.position;
                float easing = 20f;
                if (electronReference is HeavyElectron) easing = 10f;
                electronReference.transform.position = Vector2.Lerp(electronReference.transform.position, targetPosition, easing * Time.deltaTime);

                // Electric storm movement
                if (electronReference is ChargeGenerator chargeGenerator)
                {
                    if (chargeGenerator.hasSpawnedStorm())
                    {
                        chargeGenerator.MoveSpawned(player.GetVelocity());
                    }
                }
            }
        }

        // Rotate electrons
        electronAngle += electronRotationSpeed * Time.deltaTime;
    }

    public void KillAllElectrons()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;

                electronReference.isPlayerDead = true;
                electronReference.transform.position = transform.position;
            }
        }
    }

    public void ReviveAllElectrons()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;

                electronReference.isPlayerDead = false;
            }
        }
    }

    public void PickUpElectron(GameObject electronPrefab)
    {
        if(!AddElectronToBuild(electronPrefab.GetComponent<Electron>().electronID))
        {
            AddElectronToInventory(electronPrefab);
        }
    }

    public Nucleus GetActiveNucleus()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;
                
                if (electronReference is Nucleus nucleus && nucleus.isActive())
                {
                    return nucleus;
                }
            }
        }

        return null;
    }

    public Neutralizer GetActiveNeutralizer()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;
                
                if (electronReference is Neutralizer neutralizer && neutralizer.isActive())
                {
                    return neutralizer;
                }
            }
        }

        return null;
    }

    public QuantumParticle GetActiveQuantumParticle()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;
                
                if (electronReference is QuantumParticle particle && particle.isActive())
                {
                    return particle;
                }
            }
        }

        return null;
    }

    public float GetHealthBonus()
    {
        float bonus = 0f;
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;

                if (electronReference is Neutron neutron)
                {
                    bonus += neutron.maxHealthBonus;
                }
            }
        }

        return bonus;
    }

    public bool AddElectronToBuild(int electronID)
    {
        // Finds the avaiable position to put a new electron in
        int shell = -1;
        int position = -1;
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            if (shell != -1) break;
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot)
                {
                    shell = i;
                    position = electrons.IndexOf(electron);
                    break;
                }
            }
        }

        if (shell == -1) return false;

        SpawnElectronOnNetworkServerRpc(electronID, shell, position, NetworkManager.LocalClientId);

        return true;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SpawnElectronOnNetworkServerRpc(int objectID, int shell, int position, ulong clientID)
    {
        LangObject langObject = GameObject.Find("LangObject").GetComponent<LangObject>();
        GameObject electronPrefab = langObject.electronsInGame[objectID];
        GameObject newElectronObject = Instantiate(electronPrefab, transform.position, Quaternion.identity);
        newElectronObject.GetComponent<NetworkObject>().Spawn(true);
        newElectronObject.GetComponent<NetworkObject>().ChangeOwnership(clientID);

        SpawnedElectronClientRpc(newElectronObject.GetComponent<NetworkObject>().NetworkObjectId, objectID, shell, position);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnedElectronClientRpc(ulong networkObjectId, int electronID, int shell, int position)
    {
        if (!IsOwner) return;
        NetworkObject newElectronObject = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId];
        LangObject langObject = GameObject.Find("LangObject").GetComponent<LangObject>();
        GameObject electronPrefab = langObject.electronsInGame[electronID];

        electronsPerShell[shell][position] = new SpawnedElectron(shell, newElectronObject.GetComponent<Electron>(), electronPrefab);
        electronsPerShell[shell][position].electronReference.player = this;
        UpdateElectronAngles();
        UpdateHotbarUI();
    }

    public bool InsertElectronToBuild(int shell, int position, GameObject electronPrefab)
    {
        if (!electronsPerShell[shell][position].isEmptySlot) return false;

        SpawnElectronOnNetworkServerRpc(electronPrefab.GetComponent<Electron>().electronID, shell, position, NetworkManager.LocalClientId);

        return true;
    }

    public void UpdateElectronAngles()
    {
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            int filledSlots = 0;
            foreach (SpawnedElectron electron in electrons)
            {
                if (!electron.isEmptySlot) filledSlots++;
            }

            if (filledSlots == 0) continue;

            float angleStep = 360f / filledSlots;
            int currentIndex = 0;
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                electron.angle = angleStep * currentIndex * Mathf.Deg2Rad;
                currentIndex++;
            }
        }
    }

    public void UpdateHotbarUI()
    {
        int slotID = 0;
        for (int i = 0; i < maxElectronsPerShell.Length; i++)
        {
            List<SpawnedElectron> electrons = electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                uiSlots[slotID].electronInSlot = electron.isEmptySlot ? null : electron.electronPrefab;
                uiSlots[slotID].amount = electron.isEmptySlot ? 0 : 1;
                uiSlots[slotID].UpdateSlotImage();

                slotID++;
            }
        }
    }

    public void AddElectronToInventory(GameObject electronPrefab)
    {
        if (electronInventory.ContainsKey(electronPrefab) && electronInventory[electronPrefab] > 0)
        {
            Debug.Log("Increased amount of electron in inventory: " + electronInventory[electronPrefab]);
            electronInventory[electronPrefab]++;
            foreach(Transform slot in inventoryContainer)
            {
                UISlot uiSlot = slot.GetComponent<UISlot>();
                if(uiSlot.electronInSlot == electronPrefab)
                {
                    uiSlot.amount = electronInventory[electronPrefab];
                    uiSlot.UpdateSlotImage();
                    break;
                }
            }
        } else {
            Debug.Log("Added new electron to inventory");
            electronInventory[electronPrefab] = 1;
            GameObject inventoryElectron = Instantiate(inventoryElectronPrefab, inventoryContainer);
            inventoryElectron.transform.SetParent(inventoryContainer, false);
            UISlot uiSlot = inventoryElectron.GetComponent<UISlot>();
            uiSlot.SetPEC(this);
            uiSlot.electronInSlot = electronPrefab;
            uiSlot.amount = 1;
            uiSlot.UpdateSlotImage();
        }
    }

    public void RemoveElectronFromInventory(GameObject electronPrefab)
    {
        if (!electronInventory.ContainsKey(electronPrefab)) return;

        electronInventory[electronPrefab]--;
        foreach (Transform slot in inventoryContainer)
        {
            UISlot uiSlot = slot.GetComponent<UISlot>();
            if (uiSlot.electronInSlot == electronPrefab)
            {
                uiSlot.amount = electronInventory[electronPrefab];
                uiSlot.UpdateSlotImage();
                break;
            }
        }
    }

    public void RemoveElectronFromBuild(int shell, int position)
    {
        SpawnedElectron electron = electronsPerShell[shell][position];
        if (electron.isEmptySlot) return;

        DestroyElectronServerRpc(electron.electronReference.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        electronsPerShell[shell][position] = new SpawnedElectron(true);
        UpdateElectronAngles();
        UpdateHotbarUI();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void DestroyElectronServerRpc(ulong objectID)
    {
        Destroy(NetworkManager.SpawnManager.SpawnedObjects[objectID].gameObject);
        foreach(Transform child in NetworkManager.SpawnManager.SpawnedObjects[objectID].transform)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("REMOVED " + objectID + " Object: " + NetworkManager.SpawnManager.SpawnedObjects[objectID].name);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        if (collision.gameObject.TryGetComponent(out ElectronDrop drop))
        {
            LangObject langObject = GameObject.Find("LangObject").GetComponent<LangObject>();
            PickUpElectron(langObject.electronsInGame[drop.electronDropID.Value]);
            drop.PickedUpServerRpc();
        }
    }

    public int[] getMaxElectronsPerShell()
    {
        return maxElectronsPerShell;
    }
}

[System.Serializable]
public class SpawnedElectron
{
    public float angle;
    public float direction;
    public float distance;
    public int shell;
    public Electron electronReference;
    public GameObject electronPrefab;
    public bool isEmptySlot;

    public SpawnedElectron(int shell, Electron electronReference, GameObject electronPrefab)
    {
        this.shell = shell;
        this.electronReference = electronReference;
        this.electronPrefab = electronPrefab;
        this.angle = 0;
        direction = 1;
        distance = 1f + (0.8f * shell);
        isEmptySlot = false;
        electronReference.spawnedElectronInfo = this;
    }

    public SpawnedElectron(bool isEmpty)
    {
        shell = 0;
        electronReference = null;
        angle = 0;
        direction = 0;
        distance = 0;
        isEmptySlot = isEmpty;
    }
}