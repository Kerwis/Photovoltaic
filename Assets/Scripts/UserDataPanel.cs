using UnityEngine;
using UnityEngine.UI;

public class UserDataPanel : MonoBehaviour
{
	private readonly string linkToTerms = "www.google.pl";

	public ReportGenerator reportGenerator;
	
	public Text userName;
	public Text location;
	public Text email;
	public Text phone;
	public Text emailTrader;
	
	public Text userNamePlaceholder;
	public Text locationPlaceholder;
	public Text emailPlaceholder;
	public Text phonePlaceholder;

	private bool missing = false;

	public void OpenTerms()
	{
		Application.OpenURL(linkToTerms);
	}

	public void Show()
	{
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(true);
	}

	public void TrySendReport()
	{
		missing = false;
		//CheckFields
		if (userName.text == "")
		{
			userNamePlaceholder.color = Color.red;
			missing = true;
		}
		
		if (location.text == "")
		{
			locationPlaceholder.color = Color.red;
			missing = true;
		}
		
		if (email.text == "")
		{
			emailPlaceholder.color = Color.red;
			missing = true;
		}
		
		if (phone.text == "")
		{
			phonePlaceholder.color = Color.red;
			missing = true;
		}
		
		if(missing)
			return;
		
		Hide();
		
		reportGenerator.SendReport(email.text, emailTrader.text);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}