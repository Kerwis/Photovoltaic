using System.Collections;
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
    public static string PanelSaveName => PanelsProducer.name + PanelsCatalog.power + ".pdf";
    public static float ExtraCost => PanelsCatalog.extraCost + FalownikCatalog.extraCost;
    public static string FalownikSaveName => FalownikProducer.name + FalownikCatalog + ".pdf";

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
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            if ((Input.mousePosition - lastMousePosition).x > 0)
            {
                if (panelCatalogIndex < PanelsProducer.catalogs.Capacity - 1 && PanelsProducer.catalogs[panelCatalogIndex + 1].power <= panelPower)
                    panelCatalogIndex++;
            }
            else
            {
                if (panelCatalogIndex > 0 && PanelsProducer.catalogs[panelCatalogIndex - 1].power >= panelPower)
                    panelCatalogIndex--;
            }

            lastMousePosition = Input.mousePosition;
        }
#else
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).deltaPosition.x > 0)
            {
                if (catalogIndex < producer.catalogs.Capacity - 1 &&
                    producer.catalogs[catalogIndex + 1].power <= panelPower)
                    catalogIndex++;
            }
            else
            {
                if (catalogIndex > 0 && producer.catalogs[catalogIndex - 1].power >= panelPower)
                    catalogIndex--;
            }
        }
#endif
        
        
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