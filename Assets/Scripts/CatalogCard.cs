﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Config;
using UnityEngine;
using UnityEngine.UI;

public class CatalogCard : MonoBehaviour
{
    public PanelsPDFConfig panelsPdfConfig;
    public FalownikPDFConfig falownikPdfConfig;
    public Slider slider;
    public Text powerText;
    private int power;
    public static PanelsPDFConfig.Producer PanelsProducer = new PanelsPDFConfig.Producer();
    public static PanelsPDFConfig.Catalog PanelsCatalog = new PanelsPDFConfig.Catalog();
    public static FalownikPDFConfig.Producer FalownikProducer = new FalownikPDFConfig.Producer();
    public static FalownikPDFConfig.Catalog FalownikCatalog = new FalownikPDFConfig.Catalog();
    private int panelCatalogIndex = 0;
    private Vector3 lastMousePosition = Vector3.zero;

    public static string PanelSaveName =>
        Path.Combine(Application.persistentDataPath, PanelsProducer.name + PanelsCatalog.power + ".pdf");

    public static string FalownikSaveName => 
        Path.Combine(Application.persistentDataPath, FalownikProducer.name + FalownikCatalog.powerMax + ".pdf");

    public static float ExtraCost(int panelsCount) => PanelsCatalog.extraCost * panelsCount + FalownikCatalog.extraCost;

    public void SetPanelNo(int index)
    {
        PanelsProducer = panelsPdfConfig.producers[index];
        slider.maxValue = PanelsProducer.catalogs.Last().power;
        slider.minValue = PanelsProducer.catalogs[0].power;
        slider.value = PanelsProducer.catalogs[0].power;
    }

    public void SetFalownikNo(int index)
    {
        FalownikProducer = falownikPdfConfig.producers[index];
        SetInstallationPower(Manager.InstallationPower);
        slider.onValueChanged.Invoke(slider.value);
    }

    public void SetInstallationPower(float installationPower)
    {
        foreach (var catalog in FalownikProducer.catalogs)
        {
            if (catalog.powerMin <= installationPower && catalog.powerMax >= installationPower)
            {
                FalownikCatalog = catalog;
                return;
            }
        }
    }
    
    public int SetPanelPower(float panelPower)
    {
        int direction = 0;
        panelPower = Manager.RoundValue(panelPower, 5);
        panelCatalogIndex = Mathf.Clamp(panelCatalogIndex, 0, PanelsProducer.catalogs.Count - 1);
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) && PanelsProducer.catalogs.Count > 1)
        {
            if ((Input.mousePosition - lastMousePosition).x > 0)
            {
#else
        if (Input.touchCount > 0 && PanelsProducer.catalogs.Count > 1)
        {
            if (Input.GetTouch(0).deltaPosition.x > 0)
            {
#endif
                if (panelCatalogIndex < PanelsProducer.catalogs.Count - 1 &&
                    PanelsProducer.catalogs[panelCatalogIndex + 1].power <= panelPower)
                    panelCatalogIndex++;
            }
            else
            {
                if (panelCatalogIndex > 0 && PanelsProducer.catalogs[panelCatalogIndex - 1].power >= panelPower)
                    panelCatalogIndex--;
            }

            lastMousePosition = Input.mousePosition;
        }

        panelCatalogIndex = Mathf.Clamp(panelCatalogIndex, 0, PanelsProducer.catalogs.Count - 1);
        
        PanelsCatalog = PanelsProducer.catalogs[panelCatalogIndex];
        panelPower = PanelsCatalog.power;
        
        slider.SetValueWithoutNotify(panelPower);
        power = (int) panelPower;
        powerText.text = power + " Wp";
        return power;
    }

    public void DownloadFromRemote()
    {
        panelsPdfConfig.DownloadFromRemote((x) =>
        {
            if (x) SetPanelNo(0);
        });
        falownikPdfConfig.DownloadFromRemote((x)=>{});
    }
}