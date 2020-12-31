using UnityEngine;
using UnityEngine.UI;

public class UserDataPanel : MonoBehaviour
{
	private readonly string linkToTerms = "www.google.pl";

	public ReportGenerator reportGenerator;
	
	public InputField userName;
	public InputField location;
	public InputField email;
	public InputField phone;
	public InputField emailTrader;
	
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
		gameObject.SetActive(true);
		transform.localPosition = Vector3.zero;
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