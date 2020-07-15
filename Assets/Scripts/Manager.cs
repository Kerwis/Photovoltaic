using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
	private readonly string phoneNumber = "733443344";
	private readonly string websideAdress = "http://fotowoltaika.gft.pl/";
	private readonly float fiveKDonationLimit = 10;
	private readonly float panelArea = 1.66f;
	
	public PowerCostConfig config;
	public Slider slider;
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
	public InputField kWhPriceLabel;
	public Text kWhProductionPerLabel;
	public Text investmentBackAfterNoDonationLabel;
	public Text investmentBackAfterWithDonationLabel;

	public Toggle ground;
	public Toggle ceramics;
	public InputField panelPowerLabel;
	public Text panelCountLabel;
	public Text panelAreaLabel;
	public Text areaLabel;

    
	private float providerKWhCost = 0.67f;
	private float panelPower = 330;
	private float kWhProductAnnual = 960f;
	private float groundExtraCost = 60;
	private float ceramicsExtraCost = 40;

	private bool farmer;
	private bool activeFiveK = true;
	private bool lowHouseholdTax = true;
	private bool farmerTax = true;
	private bool farmerDonation = true;
	private float annualCost;
	private float installationPower;

	private float costWithDonation;
	private float annualSaving;
	private float costNoDonation;
	private float savingAfterYears;
	private float monthlyKWhProduction;
	private float annualKWhProduction;
	private float kWhPrice;
	private float kWhProductionPer;
	private float investmentBackAfterNoDonation;
	private float investmentBackAfterWithDonation;

	private float panelCount;
	private float area;


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
		Debug.Log("Set slider to " + value);
		Slide.ShowHintIfNeed = true;
		value = RoundValue(value, 50);
		slider.value = value;
		price.text = value + "zł";

		SetAllValues(value);
	}

	private void SetAllValues(float value)
	{
		SetAnnualCost(value);
		SetProductionPower();
		SetKWhProduction(value);
		SetInfo();
		SetInstallationCost(installationPower);
		SetSaving(annualKWhProduction);
		SetProduction();
		SetInvestmentBack();
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

	public void SetPanelsPower(string value)
	{
		if (float.TryParse(value, out panelPower))
		{
			panelPower = Mathf.Clamp(panelPower, 280, 500);
			SetAllValues(slider.value);
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
		panelPowerLabel.text = panelPower.ToString();

		panelCount = installationPower * 1000 / panelPower;
		panelCount = Mathf.Ceil(panelCount);
		panelCountLabel.text = panelCount.ToString();

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
		costNoDonation = config.powerCosts[(int) (production * 2)] * production;
		if (ground.isOn)
			costNoDonation += groundExtraCost * panelCount;
		if (ceramics.isOn)
			costNoDonation += ceramicsExtraCost * panelCount;
		
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
				costWithDonation -= costWithDonation * 0.18f;
			else
				costWithDonation -= costWithDonation * 0.32f;
		}

		costWithDonationLabel.text = costWithDonation.ToString();
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

	private float RoundValue(float value, int round)
	{
		return value - value % round;
	}

	private void Awake()
	{
		SetGroundToggle();
		SlideValueHandler(150);
		ChangeCustomerBase(false);
		StartCoroutine(DownloadConfig());
		StartCoroutine(DownloadFromRemote(ConfigDownloadHandler));
	}

	private void SetGroundToggle()
	{
		ground.onValueChanged.AddListener((x) => SetAllValues(slider.value));
		ceramics.onValueChanged.AddListener((x) => SetAllValues(slider.value));
	}

	private IEnumerator DownloadConfig()
	{
		config.DownloadFromRemote(ConfigDownloadHandler);
		yield return null;
	}

	private void ConfigDownloadHandler(bool success)
	{
		Debug.Log("Download finish " + (success ? "Success" : "Failed"));
		if (success) SlideValueHandler(150);
	}

	private IEnumerator DownloadFromRemote(Action<bool> callback)
	{
		var client = new WebClient();

		var data = client.OpenRead(@"https://www.gft.pl/GroundPrice.htm");

		if (data == null)
		{
			callback(false);
			yield break;
		}

		var reader = new StreamReader(data);
		string s;
		float newValue;

		s = reader.ReadLine();

		if (s == null)
		{
			callback(false);
			yield break;
		}

		if (float.TryParse(s, out newValue))
		{
			groundExtraCost = newValue;
		}
		else
		{
			callback(false);
			yield break;
		}

		s = reader.ReadLine();

		if (s == null)
		{
			callback(false);
			yield break;
		}

		if (float.TryParse(s, out newValue))
		{
			ceramicsExtraCost = newValue;
		}
		else
		{
			callback(false);
			yield break;
		}

		data.Close();
		reader.Close();

		callback(true);
		yield return null;
	}
}