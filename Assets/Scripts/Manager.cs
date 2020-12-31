using System;
using System.Collections;
using Config;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
	private readonly string phoneNumber = "733443344";
	private readonly string websideAdress = "http://fotowoltaika.gft.pl/";
	private readonly float fiveKDonationLimit = 10;
	private readonly float panelSmallArea = 1.66f;
	private readonly float panelBigArea = 2.22f;
	private readonly float optimizerCost = 280;
	private readonly float co2ReductionFactor = 0.000812f;
	
	public PowerCostConfig powerCostConfig;
	public GroundPriceConfig groundPriceConfig;
	public Slider slider;
	public CatalogCard catalogCard;
	public Text price;
	public Text annualCostLabel;
	public Text installationPowerLabel;
	public GameObject donationHousehold;
	public Toggle fiveKDonation;
	public GameObject donationFarmer;

	public Text costWithDonationLabel;
	public Text annualSavingLabel;
	public Text costNoDonationLabel;
	public Text savingAfterYearsLabel;
	public Text monthlyKWhProductionLabel;
	public Text annualKWhProductionLabel;
	public Text co2SavingKGLabel;
	public InputField kWhPriceLabel;
	public Text kWhProductionPerLabel;
	public Text investmentBackAfterNoDonationLabel;
	public Text investmentBackAfterWithDonationLabel;

	public Toggle ground;
	public Toggle ceramics;
	public Toggle optimizer;
	//public InputField panelPowerLabel;
	public Text panelCountLabel;
	public Text panelAreaLabel;
	public Text areaLabel;

	public Text costWithDonationLastPageLabel;
	public InputField finalCost;

	public GameObject sliderHint;
    
	private float providerKWhCost = 0.67f;
	private float panelPower = 330;
	private float kWhProductAnnual = 960f;
	private float GroundExtraCost => groundPriceConfig.groundExtraCost;
	private float CeramicsExtraCost => groundPriceConfig.ceramicsExtraCost;

	private bool farmer;
	private bool activeFiveK = true;
	private bool lowHouseholdTax = true;
	private bool farmerTax = true;
	private bool farmerDonation = true;
	private float annualCost;
	private static float installationPower = 3;
	
	public static float InstallationPower => installationPower;

	private float costWithDonation;
	private float annualSaving;
	private float costNoDonation;
	private float savingAfterYears;
	private float monthlyKWhProduction;
	private float annualKWhProduction;
	private float co2SavingKG;
	private float kWhPrice;
	private float kWhProductionPer;
	private float investmentBackAfterNoDonation;
	private float investmentBackAfterWithDonation;

	private float panelCount;
	private float area;
	private float panelArea;

	public void ChangeCustomerBase(bool isFarmer)
	{
		farmer = isFarmer;
		donationFarmer.SetActive(farmer);
		donationHousehold.SetActive(!farmer);
		SetAllValues(slider.value);
	}

	public void ActiveFiveK(bool active)
	{
		activeFiveK = active;
		SetAllValues(slider.value);
	}

	public void SetHighHouseholdTax(bool high)
	{
		lowHouseholdTax = !high;
		SetAllValues(slider.value);
	}

	public void SetFarmerTax(bool tax)
	{
		farmerTax = tax;
		SetAllValues(slider.value);
	}

	public void SetFarmerDotation(bool dotation)
	{
		farmerDonation = dotation;
		SetAllValues(slider.value);
	}

	public void SlideValueHandler(float value)
	{
		sliderHint.SetActive(false);
		SetSliderValue(value);
	}

	public void SetSliderValue(float value)
	{
		Debug.Log("Set slider to " + value);
		
		value = RoundValue(value, 50);
		slider.value = value;
		price.text = value + "zł";

		SetAllValues(value);	
	}

	public void Recalculate()
	{
		SetAllValues(slider.value);
	}

	private void SetAllValues(float value)
	{
		SetAnnualCost(value);
		SetProductionPower();
		SetKWhProduction(value);
		SetCO2Saving();
		SetInfo();
		SetInstallationCost(installationPower);
		SetSaving(annualKWhProduction);
		SetProduction();
		SetInvestmentBack();
		SetFinalCost();
	}

	public void OpenPhone()
	{
		Application.OpenURL("tel://" + phoneNumber);
	}

	public void OpenWebPage()
	{
		Application.OpenURL(websideAdress);
	}

	public void SetkWhPrice(string value)
	{
		if (float.TryParse(value, out providerKWhCost))
		{
			providerKWhCost = Mathf.Clamp(providerKWhCost, 0.01f, 50);
			SetAllValues(slider.value);
		}
	}

	public void SetPanelsPower(float power)
	{
		panelPower = catalogCard.SetPanelPower(power);
		SetAllValues(slider.value);
	}
	public void SetPanelsPower(string value)
	{
		if (float.TryParse(value, out panelPower))
		{
			panelPower = Mathf.Clamp(panelPower, 280, 700);
			SetAllValues(slider.value);
			catalogCard.SetPanelPower(panelPower);
		}
	}

	public void SetAnnualProduction(string value)
	{
		if (float.TryParse(value, out kWhProductAnnual))
		{
			kWhProductAnnual = Mathf.Clamp(kWhProductAnnual, 900, 1200);
			SetAllValues(slider.value);
		}
	}

	private void SetInfo()
	{
		//panelPowerLabel.text = panelPower.ToString();

		panelCount = installationPower * 1000 / panelPower;
		panelCount = Mathf.Ceil(panelCount);
		panelCountLabel.text = panelCount.ToString();

		if (panelPower > 370)
		{
			panelArea = panelBigArea;
		}
		else
		{
			panelArea = panelSmallArea;
		}
		
		panelAreaLabel.text = panelArea.ToString();

		area = panelCount * panelArea;
		areaLabel.text = area.ToString();
	}

	private void SetInvestmentBack()
	{
		investmentBackAfterNoDonation = costNoDonation / annualSaving;
		investmentBackAfterNoDonationLabel.text = investmentBackAfterNoDonation.ToString("n2");

		investmentBackAfterWithDonation = costWithDonation / annualSaving;
		investmentBackAfterWithDonationLabel.text = investmentBackAfterWithDonation.ToString("n2");
	}

	private void SetProduction()
	{
		kWhPrice = providerKWhCost;
		kWhPriceLabel.text = kWhPrice.ToString();

		kWhProductionPer = kWhProductAnnual;
		kWhProductionPerLabel.text = kWhProductionPer + "kWh";
	}

	private void SetInstallationCost(float production)
	{
		costNoDonation =
			powerCostConfig.powerCosts
				[Mathf.Clamp((int) (production * 2), 0, powerCostConfig.powerCosts.Count - 1)] *
			production;
		if (ground.isOn)
			costNoDonation += GroundExtraCost * panelCount;
		if (ceramics.isOn)
			costNoDonation += CeramicsExtraCost * panelCount;
		if (optimizer.isOn)
			costNoDonation += optimizerCost * panelCount;

		costNoDonation += CatalogCard.ExtraCost;
		
		costNoDonationLabel.text = costNoDonation.ToString();
		if (farmer)
		{
			//For farmer
			costWithDonation = costNoDonation;
			if (farmerDonation) costWithDonation -= costNoDonation * 0.4f;

			if (farmerTax) costWithDonation -= costNoDonation * 0.6f * 0.25f;
		}
		else
		{
			costWithDonation = costNoDonation;
			//For household
			if (activeFiveK) costWithDonation -= 5000;

			if (lowHouseholdTax)
				costWithDonation -= costWithDonation * 0.17f;
			else
				costWithDonation -= costWithDonation * 0.32f;
		}

		costWithDonationLabel.text = costWithDonation.ToString();
		costWithDonationLastPageLabel.text = costWithDonation.ToString();
	}

	private void SetSaving(float production)
	{
		annualSaving = Mathf.Ceil(production * providerKWhCost * 0.8f);
		annualSavingLabel.text = annualSaving.ToString();
		savingAfterYears = annualSaving * 25;
		savingAfterYearsLabel.text = Mathf.Ceil(savingAfterYears / 1000) + "tyś";
	}

	private void SetProductionPower()
	{
		installationPower = RoundToHalf(annualCost / providerKWhCost / 1000);
		installationPowerLabel.text = installationPower.ToString();
		CheckAvailabilityFiveKDonation();
		catalogCard.SetInstallationPower(installationPower);
	}

	private float RoundToHalf(float value)
	{
		int wholePart = (int) value;
		float rest = value - wholePart;
		return (float) (wholePart + (rest > 0.5 ? 1 : 0.5));
	}

	private void SetKWhProduction(float value)
	{
		annualKWhProduction = kWhProductAnnual * installationPower * 0.913f;
		annualKWhProductionLabel.text = annualKWhProduction.ToString();

		monthlyKWhProduction = annualKWhProduction / 12;
		monthlyKWhProductionLabel.text = monthlyKWhProduction.ToString();
	}

	private void SetCO2Saving()
	{
		co2SavingKG = RoundValue(annualKWhProduction * co2ReductionFactor, 1);
		co2SavingKGLabel.text = co2SavingKG + "t";
	}

	private void CheckAvailabilityFiveKDonation()
	{
		if (fiveKDonation.interactable == installationPower <= fiveKDonationLimit)
			return;
		fiveKDonation.interactable = installationPower <= fiveKDonationLimit;
		fiveKDonation.isOn = installationPower <= fiveKDonationLimit;
	}

	private void SetAnnualCost(float value)
	{
		annualCost = value * 12;
		annualCostLabel.text = annualCost.ToString();
	}

	public static float RoundValue(float value, int round)
	{
		return value - value % round;
	}

	private void Awake()
	{
		SetSliderValue(150);
		ChangeCustomerBase(false);
		StartCoroutine(DownloadConfig());
	}

	private void SetFinalCost()
	{
		finalCost.text = costWithDonation + " zł";
	}

	private IEnumerator DownloadConfig()
	{
		powerCostConfig.DownloadFromRemote(ConfigDownloadHandler);
		groundPriceConfig.DownloadFromRemote(ConfigDownloadHandler);
		catalogCard.DownloadFromRemote();
		yield return null;
	}

	private void ConfigDownloadHandler(bool success)
	{
		Debug.Log("Download finish " + (success ? "Success" : "Failed"));
		if (success) SetSliderValue(150);
	}
}