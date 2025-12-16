using TMPro;
using UnityEngine;

public class StatsWindow : MonoBehaviour
{
    public TextMeshProUGUI electronName;
    public TextMeshProUGUI electronFullDescription;
    public TextMeshProUGUI electronReload;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
    }

    // Update is called once per frame
    public void updateStats(Electron electron)
    {
        electronName.text = electron.electronName;
        string electronDescription = electron.description + "\n\n" + electron.actualDescription + "\n\nDamage: " + electron.damage + "\nHealth: " + electron.maxHealth + "\n" + electron.stats;
        electronFullDescription.text = electronDescription;
        electronReload.text = electron.reload.ToString();
        if(electron.secondaryReload > 0f){
            electronReload.text += " + " + electron.secondaryReload + "s";
        } else {
            electronReload.text += "s";
        }
    }
}
