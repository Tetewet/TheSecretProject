using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BlackSmith : MonoBehaviour {

    public Button[] slot; // pour les slots des items dans le menu 
    public Image[] iconItem; // pour les images des items qui seront chargés dans les slots. 
    public TextMeshProUGUI quantItem, quantGold, price; // pour afficher les quantités de chaque item

    public List<GameObject> loadItemInventory; // liste temporaire des items

    private string rarity;
    private float valeurBaseItem = 10; // valeur de base pour tous les items
    private float facteurCalculRarity; // facteur utilisé pour calculer le prix selon la rarité utilisé pour l'achat et vente
    //commentaires


    // Use this for initialization
    void Start () {
        clearLoadedItems();

        foreach (Button b in slot)
        {
            b.interactable = false;
        }

        foreach(Image i in iconItem)
        {
            i.sprite = null;
            i.gameObject.SetActive(false);
        }

        quantItem.text = "x 0";
        quantGold.text = "x 0";
        price.text = "0";

        
        switch (rarity)
        {
            case "common":
                facteurCalculRarity = 1;
                break;
            case "Uncommon":
                facteurCalculRarity = 2;
                break;
            case "Rare":
                facteurCalculRarity = 3;
                break;
            case "Unique":
                facteurCalculRarity = 4;
                break;
            case "Legendary":
                facteurCalculRarity = 6;
                break;
            case "WorldClass":
                facteurCalculRarity = 8;
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private float calculValeurItem() // le calcul du valeur de vente des itens vendus par le personnage
    {
        valeurBaseItem = valeurBaseItem*facteurCalculRarity;
        return valeurBaseItem;
    }
        
    public void loadInventory() // méthode pour faire le load de l'Inventory 
    {
        foreach (Button b in slot)
        {
            b.interactable = false;
        }

        foreach (Image i in iconItem)
        {
            i.sprite = null;
            i.gameObject.SetActive(false);
        }
        quantItem.text = "x 0";
        quantGold.text = "x 0";
        price.text = "0";

        int s = 0;
        foreach(GameObject i in loadItemInventory)
        {
            GameObject temp = Instantiate(i); // pour instantier les items de la liste temporaire
            loadItemInventory.Add(temp);

            slot[s].GetComponent<InventorySlot>().objectSlot = i;
            slot[s].interactable = true;       

            // il faut créer le script des images des items dans la classe inventory, pour charger les images dans l'inventory du blackSmith

            s++;
        }
    }
    
    public void clearLoadedItems() // méthode pour effacer les items de la liste temporaire
    {
        foreach(GameObject ic in loadItemInventory)
        {
            Destroy(ic);
        }
        loadItemInventory.Clear();
    }

    // on va créer l'option d'améliorer les items dans le black smith?
    // quoi il faut ajouter de méthodes et fonctionalités?

}

