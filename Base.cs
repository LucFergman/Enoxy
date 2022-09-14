using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SocialPlatforms.Impl;
using BreakInfinity;
using static BreakInfinity.BigDouble;
using System.Reflection;
using FactoryUnits;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour
{
    public PlayerData data;
    //public EventManager events;
    public DarkMaterrManager materr;
    public RebithManager rebith;
    public AchievementsManager achievements;
    //public OfflineManager offline;
    public FactoryManager factoryM;
    public Factory factory;
    public PlanetManager planetM;
    public Planet planets;
    public ShipManager shipM;

    #region Texts
    public Text[] fuelRfOxText = new Text[3];
    public Text[] fuelRfOxsecText = new Text[3];
    public GameObject[] fuelRfOxInfo = new GameObject[3];
    public BigDouble passivePlanetOxy;

    public Text donatiumText;//Колво Донатий(текст)

    public Text[] resoursesText = new Text[6];
    public Text[] resoursesSecText = new Text[6];

    //Завод,Factory

    //Корабль,Ship
    public Text FuelRefineryUpgradeText;//Улучшение FuelRefinery (текст кнопки)
    public Text CrewModuleUpgradeText;//Улучшение CrewModule (текст кнопки)
    public Text FuelTankModuleUpgradeText;//Улучшение FuelTankModule (текст кнопки)
    public Text CargoModuleUpgradeText;//Улучшение CargoModule (текст кнопки)
    public Text EngineUpgradeText;//Улучшение Engine (текст кнопки)
    public Text ScanerUpgradeText;//Улучшение Scaner (текст кнопки)
    public Text PlanetaryStorageUpgradeText;//Улучшение PlanetaryStorage (текст кнопки)
    public Text GasGiantEaterUpgradeText;//Улучшение GasGiantEater (текст кнопки)
    public Text ShipUpgradeText;//Улучшение Ship (текст кнопки)

    #endregion
    #region GameObjects
    public GameObject GetMaterr;//Кнопка получения DarkMaterr
    #endregion
    #region resourses
    //public GameObject fuelProduction;//Скрытое окно параметров топлива
    //public GameObject rawFuelProduction;//Скрытое окно параметров rawFuel, открывается после улучшения BaseMiner
    /*public GameObject carbonProduction;//Скрытое окно параметров углерода(добыча,склад), открывается после улучшения BaseMiner
    public GameObject titaniumProduction;//скрытое окно титана открывается если AutoMinerUpgradeLevel>=1
    public GameObject he3Production;//скрытое окно гелий3
    public GameObject platinumProduction;//скрытое окно платины
    public GameObject palladiumProduction;//скрытое окно палладия
    public GameObject sulphurProduction;//скрытое окно сера
    public GameObject tungstenProduction;//скрытое окно вольфрам
    public GameObject osmiridiumProduction;//скрытое окно осмиридий*/
    public GameObject[] baseResourses = new GameObject[6];//Базовые - 0Алюминий, 1Титан, 2Железо, 3Серебро, 4Магний, 5Ниобий
    #endregion

    #region Planets
    #region Solar System
    public GameObject[] solarPlanets = new GameObject[26];
    /* sun number = 0 
     * mercury number = 1
     * venus number = 2
     * earth number = 3
     * mars number = 4
     * jupiter number = 5
     * saturn number = 6
     * uran number = 7
     * neptun number = 8
     * pluto number = 9
     * haron number = 10
     * moon number = 11
     * deimos number = 12
     * phobos number = 13
     * io number = 14
     * europa number = 15
     * ganimed number = 16
     * callisto number = 17
     * atlas number = 18
     * enceladus number = 19
     * diona number = 20
     * titan number = 21
     * titania number = 22
     * oberon number = 23
     * protheus number = 24
     * triton number = 25

     */
    public BigDouble totalResOnPlanet;
    #endregion
    #endregion
    public FuelRefinary fuelRefinary;
    public GameObject mainButton;
    public Canvas mainMenuGroup;
    public GameObject factoryButton;
    public Canvas factoryMenuGroup;
    public GameObject factoryScreen;
    public GameObject shipButton;
    public Canvas shipMenuGroup;   
    public Canvas settingsGroup;
    public Canvas achievementsGroup;
    public Canvas eventsGroup;
    public Canvas sectorMapGroup;
    

    public Image[] fuelRfOxBar = new Image[3];
    public BigDouble[] fuelRfOxTemp = new BigDouble[3];

    //public Canvas rebithGroup;//10 этап - не входит в 1.0 версию.




    public void Start()
    {
        Application.targetFrameRate = 60;
        data = SaveSystem.SaveExists("PlayerData") ? SaveSystem.LoadPlayer<PlayerData>("PlayerData") : new PlayerData();
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            Debug.Log(data.firstPlanetStart[data.pNumberInfo]);

            DisableCanvas();
            mainMenuGroup.gameObject.SetActive(true);
            foreach (GameObject elemet in solarPlanets) elemet.SetActive(false);
            solarPlanets[data.pNumberInfo].gameObject.SetActive(true);

            if (data.DarkMaterr >= 1) { GetMaterr.SetActive(true); }
            else GetMaterr.SetActive(false);

            DevTest();

            planetM.FirstFillPlanet();

            //events.StartEvents();
            materr.StartMaterr();
            TotalResoursesPerSec();
            DisableAllObjects();
            LoadMainGameobjects();

            shipM.StartShip();

            factoryM.StartFactory();

            if (data.travelTimer > 0) planetM.DisablePlanets();
            else planetM.SwitchPlanets(data.tPlanetInfo);

            

        }        
        //offline.LoadOfflineProduction();

    }
    public void DevTest()
    {
        Debug.Log("Удалить");
        //shipMenuGroup.gameObject.SetActive(true);
        data.clickCount = 5;
        data.unitsT1Level[0] = 5;
        //data.unitsT1onPlanetCount[1, 3] = 1;
        data.unitsT1Level[1] = 1;
        data.unitsT1Level[2] = 1;
        //data.unitsT1onPlanetCount[2, 3] = 1;
    }
    public void OpenMenus(GameObject panel)
    {
        GameObject[] disableCanvas = GameObject.FindGameObjectsWithTag("BaseCanvas");
        foreach (GameObject o in disableCanvas)
        {
            o.SetActive(false);
        }
               
        //DisableCanvas();
        panel.SetActive(!panel.activeSelf);
    }
    public void DisableCanvas()
    {
        mainMenuGroup.gameObject.SetActive(false);
        shipMenuGroup.gameObject.SetActive(false);
        factoryMenuGroup.gameObject.SetActive(false);
        materr.prestige.gameObject.SetActive(false);
    }

    public void Update()
    {

        //rebith.Run();//10 этап, не входит в версию 1.0
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            achievements.RunAchievements();
            materr.Run();
            if (data.getDarkMaterr >= 1) GetMaterr.SetActive(true);
            //отключение планеты если на ней нет ресурсов
            if (totalResOnPlanet <= 0) solarPlanets[data.pNumberInfo].gameObject.SetActive(false);

            //заполнение баров(fuel, Rf, Oxy)
            for (int x = 0; x < data.fuelRfOx.Length; x++)
            {
                Methods.BigDoubleFill(data.fuelRfOx[x], data.maxFluids, ref fuelRfOxBar[x]);
            }

            ResoursesTexts();
        }
            //Будет считать по максимальному значению руд
            #region Rework
            BigDouble summForPrestige = 0;
        for (int x = 0; x < data.baseRes.Length; x++)
        {
            
            if(summForPrestige >= data.TotalForPrestige)
            {
                summForPrestige += data.baseRes[x];
                data.TotalForPrestige = summForPrestige;
            }
        }
        data.getDarkMaterr = 150 * Sqrt(data.TotalForPrestige / 1e1);//Рассчет получения темной материи
        if (data.getDarkMaterr > data.maxDM) { data.maxDM = data.getDarkMaterr; }


        //Отключение добычи ресурсов при отсутствии топлива
        if (data.fuelRfOx[0] <= 0) data.resZeroFuel = 0;
        else data.resZeroFuel = 1;

        data.fuelRfOx[0] += FuelProduction() * Time.deltaTime;
        data.fuelRfOx[1] += RawFuelProduction() * Time.deltaTime;
        data.fuelRfOx[2] += OxyProduction() * Time.deltaTime;


        //Вычитание всех собранных ресурсов планеты из общего запаса.
        for (int x = 0; x < data.baseResSolarPlanets.GetLength(0); x++)//[ресурс,планета] 
        {
            if (data.baseResSec[x, data.pNumberInfo] > 0 && data.baseResSolarPlanets[x, data.pNumberInfo] > 0)
            {
                data.baseResSolarPlanets[x, data.pNumberInfo] -= TotalResSec(x) * Time.deltaTime;
                data.baseRes[x] += TotalResSec(x) * Time.deltaTime;
                data.baseResTotalCollected[x] += TotalResSec(x) * Time.deltaTime;
                totalResOnPlanet -= TotalResSec(x) * Time.deltaTime;

                if (data.baseResSolarPlanets[x, data.pNumberInfo] <= 0) data.baseResPlanetOver[x] = 0;//отключить добычу ресурсе если его нет на планете
                
            }
            Debug.Log(data.baseRes[0]);
            if (data.baseRes[x] >= data.maxCargo) data.baseRes[x] = data.maxCargo;
        }
        for (int x = 0; x < data.fuelRfOx.Length; x++)
        {
            if (data.fuelRfOx[x] >= data.maxFluids) data.fuelRfOx[x] = data.maxFluids;
            if (data.fuelRfOx[x] <= 0) data.fuelRfOx[x] = 0;
        }
        #endregion



        saveTimer += Time.deltaTime;
        if (!(saveTimer >= 5)) return;
        Debug.Log("SaveTimer 999. Изменить на 15");
        //SaveSystem.SavePlayer(data, "PlayerData");
        data.offlineCheck = true;
        saveTimer = 0;

    }
    public float saveTimer;
    public BigDouble TotalResSec(int x)
    {
        BigDouble production = data.baseResSec[x, data.pNumberInfo] * data.resZeroFuel * data.travelMod * data.baseResPlanetOver[x];
        return production;
    }
    public BigDouble FuelProduction()
    {
        //Отключение производстав топлива, при отсутствии rawFuel
        if (data.fuelRfOx[1] <= 0) data.resZeroRF = 0;
        else data.resZeroRF = 1;
        BigDouble fuelProduction = (data.fuelConsume[data.pNumberInfo] * data.travelMod + data.fuelRfOxsec[0]) * data.resZeroRF;
        
        return fuelProduction;
    }
    public BigDouble RawFuelProduction()
    {
        BigDouble rfProduction = data.fuelRfOxsec[1] * data.resZeroFuel * data.travelMod;
        return rfProduction;
    }
    public BigDouble OxyProduction()
    {
        BigDouble oxyProduction = (data.fuelRfOxsec[2] + passivePlanetOxy) * data.travelMod;
        return oxyProduction;
    }
    public void TotalResOnPlanet()
    {
        for (int x = 0; x < data.baseResSolarPlanets.GetLength(0); x++)
        {
            totalResOnPlanet += data.baseResSolarPlanets[x, data.pNumberInfo];
        }       
    }
    public void ResoursesTexts()
    {
        for (var x = 0; x < resoursesText.Length; x++)
        {
            resoursesText[x].text = $"{Methods.ConvertNumToExp(data.baseRes[x])}";
            if (planetM.travelScreen.gameObject.activeSelf) { resoursesSecText[x].text = "In Travel";}//применяет только первое полученное значение____________
            else if (data.fuelRfOx[0] <= 0) { resoursesSecText[x].text = "Out Of Fuel";}
            else 
            {
                resoursesSecText[x].text = $"{Methods.ConvertNumToExp(TotalResSec(x))} Sec";
                if (data.baseResSolarPlanets[x,data.pNumberInfo] <= 0) resoursesSecText[x].text = "All Collected";
            }
        }
        fuelRfOxText[0].text = "Fuel: " + Methods.ConvertNumToExp(data.fuelRfOx[0]);
        fuelRfOxsecText[0].text = "Fuel/s:" + Methods.ConvertNumToExp(FuelProduction());

        fuelRfOxText[1].text = "RawFuel: " + Methods.ConvertNumToExp(data.fuelRfOx[1]);
        fuelRfOxsecText[1].text = "RawFuel/s: " + Methods.ConvertNumToExp(RawFuelProduction());

        fuelRfOxText[2].text = "Oxy: " + Methods.ConvertNumToExp(data.fuelRfOx[2]);
        fuelRfOxsecText[2].text = "Oxy/s: " + Methods.ConvertNumToExp(OxyProduction());
    }


    public BigDouble TotalBoost()
    {
        var temp = materr.TotalMaterrBoost();
        //temp *= events.eventTokensBoost;
        return temp;
    }

    public BigDouble TotalResoursesPerSec()
    {
        //data.ironPerSec;
        BigDouble temp = 0;
        temp *= TotalBoost();
        temp *= Pow(1.1, materr.levels[1]);
        return temp;
    }
    public BigDouble TotalClickValue()
    {
        var temp = data.oreClickValue;
        temp *= TotalBoost();
        temp *= BigDouble.Pow(1.5, materr.levels[0]);
        //temp *= ironClickModifier;
        return temp;
    }

    public void DisableAllObjects()
    {       
        foreach (GameObject element in fuelRfOxInfo) element.SetActive(false);
        foreach (GameObject element in baseResourses) element.SetActive(false);
        factoryButton.gameObject.SetActive(false);
        shipButton.gameObject.SetActive(false);
        fuelRfOxInfo[2].SetActive(true);
    }
    public void LoadMainGameobjects()//
    {
        //fuelRfOxInfo[2].gameObject.SetActive(true);
        if (data.clickCount >= 1) baseResourses[0].gameObject.SetActive(true);
        if (data.clickCount >= 5) factoryButton.gameObject.SetActive(true);

    }
    public void SaveScene()
    {
        SaveSystem.SavePlayer(data, "PlayerData");
    }
    public void FullReset()
    {
        data = new PlayerData();
        planetM.FirstFillPlanet();
        planetM.earthSM.transform.SetParent(planetM.systemMap);
        planetM.moonSM.transform.SetParent(planetM.systemMap);
        planetM.ship.transform.SetParent(planetM.systemMap);
        planetM.ship.transform.localPosition = new Vector3(planetM.earthSM.gameObject.transform.localPosition.x, planetM.earthSM.gameObject.transform.localPosition.y +100, 0);
        //OpenMenus("main");
        DisableAllObjects();
        factoryM.StartFactory();
        //factoryM.LoadUnlocks();
        planetM.SwitchPlanets(data.tPlanetInfo);
    }
}

    

