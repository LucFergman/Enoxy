using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using BreakInfinity;
using static BreakInfinity.BigDouble;


public class Ship : MonoBehaviour
{
    public Base game;
    public ShipManager shipM;
    public PlanetManager planetM;
    
    // Стоимость апгрейда
    public BigDouble basePrice;
    public float increacePrice;
    public float bonusModifier;

    // Если true, значит этот ресурс требуется для покупки и на складе достаточно, чтобы его купить

    private bool[] payResourse = new bool[6];
    public GameObject pricePrefab;
    public Text nameAndLevelButtonOnShipText;

    public GameObject moduleInfoBG;
    public GameObject moduleNameText;
    public GameObject moduleInfoText;

    public GameObject moduleIconBG;
    public GameObject moduleIcon;
    public GameObject moduleUpgradePriceBG;
    public GameObject priceBGText;
    public GameObject upgradeButton;

    // Достаточно ли запасов ресурса для оплаты
    public Dictionary<string, bool> itCostOre = new Dictionary<string, bool>();
    public string[] itCostOreString = new string[] { "itCostAl", "itCostTi", "itCostFe", "itCostAg", "itCostMg", "itCostNb" };
    public string[] moduleName = new string[] {"Refinery", "Cargo", "Fluids", "Scaner", "Engine", "FactoryModule" };
    public int moduleNumber;

    public int[] moduleLevelUnlocks = new int[5]; 
    public int level;
    public bool loaded;
    

    // Start is called before the first frame update
    public void StartModule()
    {
        game = FindObjectOfType<Base>();
        shipM = FindObjectOfType<ShipManager>();
        planetM = FindObjectOfType<PlanetManager>();
        moduleName = new string[] { "Refinery", "Cargo", "Fluids", "Scaner", "Engine", "FactoryModule" };
        itCostOre.Clear();
        itCostOre.Add("itCostAl", true);
        itCostOre.Add("itCostTi", false);
        itCostOre.Add("itCostFe", false);
        itCostOre.Add("itCostAg", false);
        itCostOre.Add("itCostMg", false);
        itCostOre.Add("itCostNb", false);
        FillUpgrade();
        LoadModuleLevels();

    }
    public virtual void FillUpgrade()
    {
        var data = game.data;
        GameObject moduleCanvas;
        GameObject shipButton;
        float mainSizeY;
        float mainSizeX;
        shipButton = shipM.buttonsList.Find(x => x.name == moduleName[moduleNumber]);
        nameAndLevelButtonOnShipText = shipButton.transform.Find("Text").GetComponent<Text>();
        nameAndLevelButtonOnShipText.text = moduleName[moduleNumber] + "\n" + data.shipModulesT1Level[moduleNumber] + " Lvl";

        moduleCanvas = shipM.canvasList.Find(x => x.name == moduleName[moduleNumber]);

        moduleInfoBG = moduleCanvas.transform.Find("ModuleInfoBG").gameObject;
        moduleNameText = moduleInfoBG.transform.Find("Name").gameObject;
        moduleInfoText = moduleInfoBG.transform.Find("Info").gameObject;

        upgradeButton = moduleCanvas.transform.Find("UpgradeButton").gameObject;
        moduleUpgradePriceBG = moduleCanvas.transform.Find("PriceBG").gameObject;
        priceBGText = moduleUpgradePriceBG.transform.Find("PriceText").gameObject;
        pricePrefab = (GameObject)Resources.Load("Prefabs/Units/Production");
        moduleIconBG = moduleCanvas.transform.Find("ModuleIconBG").gameObject;
        //moduleIcon = (GameObject)Resources.Load("Prefabs/Module/" + moduleName[moduleNumber]);
        //moduleIcon.transform.SetParent(moduleIconBG.transform);
        moduleIcon = moduleIconBG.transform.Find("Image").gameObject;


        mainSizeY = shipM.canvasList.Find(x => x.name == moduleName[moduleNumber]).GetComponent<RectTransform>().sizeDelta.y;
        mainSizeX = shipM.canvasList.Find(x => x.name == moduleName[moduleNumber]).GetComponent<RectTransform>().sizeDelta.x;

        //Иконка модуля
        moduleIconBG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mainSizeX, mainSizeY - moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta.y);
        moduleIcon.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta.y, moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta.y);
        //Стоимть Апгрейда тексты
        moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mainSizeX - mainSizeY * 0.25f, mainSizeY * 0.25f);
        priceBGText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta.x, moduleUpgradePriceBG.transform.GetComponent<RectTransform>().sizeDelta.y/5);
        //Кнопка апгрейда
        upgradeButton.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mainSizeY * 0.25f, mainSizeY * 0.25f);
        upgradeButton.transform.GetComponent<Button>().onClick.AddListener(delegate { BuyUpgrade(); });
        //Иконка модуля
        moduleIconBG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mainSizeX, upgradeButton.transform.GetComponent<RectTransform>().sizeDelta.y*2);
        moduleIconBG.transform.GetComponent<RectTransform>().localPosition = new Vector2(0, upgradeButton.transform.GetComponent<RectTransform>().sizeDelta.y);
        moduleIcon.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(moduleIconBG.transform.GetComponent<RectTransform>().sizeDelta.y, moduleIconBG.transform.GetComponent<RectTransform>().sizeDelta.y);

        moduleInfoBG.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(mainSizeX, upgradeButton.transform.GetComponent<RectTransform>().sizeDelta.y);
        moduleInfoBG.transform.GetComponent<RectTransform>().localPosition = new Vector2(0, mainSizeY - upgradeButton.transform.GetComponent<RectTransform>().sizeDelta.y);
        moduleNameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(moduleInfoBG.transform.GetComponent<RectTransform>().sizeDelta.x, moduleInfoBG.transform.GetComponent<RectTransform>().sizeDelta.y*0.33f); 
        moduleInfoText.transform.GetComponent<Text>();
        moduleInfoText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(moduleInfoBG.transform.GetComponent<RectTransform>().sizeDelta.x, moduleInfoBG.transform.GetComponent<RectTransform>().sizeDelta.y - moduleNameText.transform.GetComponent<RectTransform>().sizeDelta.y);
        //иконка юнита
        PriceSpawnIcons(moduleUpgradePriceBG, itCostOre);
    }
    //спаун иконок ресурса апгрейда
    public void PriceSpawnIcons(GameObject canvas, Dictionary<string, bool> costOre)
    {
        int b = 0;
        foreach (KeyValuePair<string, bool> Ore in costOre)
        {

            if (Ore.Value == true)
            {
                if (Ore.Key == itCostOreString[b] && canvas.transform.Find(itCostOreString[b]) == null)
                {

                    Text cost;
                    GameObject icon;
                    cost = Instantiate(pricePrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Text>();
                    cost.transform.SetParent(canvas.transform);
                    cost.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
                    cost.name = itCostOreString[b];
                    icon = Instantiate(planetM.resIconPrefab[b], new Vector2(0, 0), Quaternion.identity);
                    var h = 45;
                    cost.GetComponent<HorizontalLayoutGroup>().padding.left = -(h);
                    icon.transform.SetParent(cost.transform);
                    icon.GetComponent<RectTransform>().sizeDelta = new Vector2(h, h);

                    icon.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                    icon.transform.localScale = new Vector2(1f, 1f);

                    cost.GetComponent<Text>().fontSize = 30;
                    
                }
            }
            b++;
        }
    }
    public virtual void CostAndProductionTexts()
    {
        var data = game.data;
        Text costUpgrade;

        for (int x = 0; x < itCostOreString.Length; x++)
        {

            if (moduleUpgradePriceBG.transform.Find(itCostOreString[x]) != null)
            {
                costUpgrade = moduleUpgradePriceBG.transform.Find(itCostOreString[x]).GetComponent<Text>();
                costUpgrade.text = Methods.ConvertNumToExp(data.shipModuleUpgradeCosts[moduleNumber]);
            }
        }
    }
        void Update()
    {
        
    }
    public bool BaseBuyUpgrade(string id) // Покупка улучшения
    {
        if (CheckOresPrices())
        {
            var data = game.data;
            payOres(); //Оплачиваем апгрейд юнита
            switch (id)
            {
                case "Refinery":
                    Debug.Log("try buy ref " + moduleName[moduleNumber]);
                    Buy(0);
                    break;
                case "Cargo":
                    Buy(1);
                    break;
                case "Fluids":
                    Buy(2);
                    break;
                case "Scaner":
                    Buy(3);
                    break;
                case "Engine":
                    Buy(4);
                    break;
                case "FactoryModule":
                    Buy(5);
                    break;

            }
            void Buy(int moduleNumber)
            {
                
                data.shipModulesT1Level[moduleNumber]++;
                level++;
                
            }
            nameAndLevelButtonOnShipText.text = moduleName[moduleNumber] + "\n" + data.shipModulesT1Level[moduleNumber] + " Lvl";
            ModuleUnlocks();
            PriceSpawnIcons(moduleUpgradePriceBG, itCostOre);
            CalculatePriceAndBonuses();
            CostAndProductionTexts();
            
            return true;
        }
        return false;
    }
    public virtual void BuyUpgrade()
    {

    }

    // Функция проверки основных условий для покупки (какие типы ресурсов требуются и хватает ли их на складе
    public bool CheckOresPrices()
    {
        var data = game.data;
        int x = 0;
        foreach (KeyValuePair<string, bool> Ore in itCostOre)
        {
            if (Ore.Value == true)
            {
                if (Ore.Key == itCostOreString[x] && data.baseRes[x] >= data.shipModuleUpgradeCosts[moduleNumber])
                {
                    payResourse[x] = true;
                    x++;
                    continue;
                }
                return false;
            }
            
        }
        return true;
    }

    // Вычитает те ресурсы из хранилища, которые требуются
    private void payOres()
    {
        var data = game.data;
        for (var x = 0; x < payResourse.Length; x++)
        {
            if (payResourse[x]) data.baseRes[x] -= data.shipModuleUpgradeCosts[moduleNumber];
        }
        //Возвращаем переменные проверки оплаты в исходное состояние
        for (var x = 0; x < payResourse.Length; x++) payResourse[x] = false;
    }
    public void LoadModuleLevels()
    {
        var data = game.data;
        if (!loaded)
        {
            for(level = -1; level < data.shipModulesT1Level[moduleNumber];)
            {
                level++;
                ModuleUnlocks();
                
            }
            PriceSpawnIcons(moduleUpgradePriceBG, itCostOre);
            CalculatePriceAndBonuses();
            CostAndProductionTexts();
            
        }

        loaded = true;
    }
    public virtual void ModuleUnlocks()
    {
        
    }
    public virtual void CalculatePriceAndBonuses()
    {
        var data = game.data;
        data.shipModuleUpgradeCosts[moduleNumber] = basePrice * Math.Pow(increacePrice, data.shipModulesT1Level[moduleNumber]);

    }
}
