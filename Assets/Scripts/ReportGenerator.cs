using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using sharpPDF;
using sharpPDF.Enumerators;
using UnityEngine;

public class ReportGenerator : MonoBehaviour
{
	public Slide slide;
	public GameObject waitSending;
	public float scal = 1 / 2;
	private Camera cam;
	private Rect rect;
	private string path;
	private SmtpClient smtpServer;
	private string reportName = "Report";
	private pdfDocument document;

	public void SendReport(string email, string emailTrader)
	{
		rect = slide.canvas.rect;
		cam = Camera.main;
		path = Application.persistentDataPath;
		
		document = new pdfDocument("Report","GFT", false);

		StartCoroutine(CreateReport(email, emailTrader));
	}

	private IEnumerator CreateReport(string email, string emailTrader)
	{
		waitSending.SetActive(true);

		yield return new WaitForEndOfFrame();

		waitSending.SetActive(false);

		var panelsCount = slide.PanelsTransforms.Count;
		for (int i = 0; i < panelsCount; i++)
		{
			slide.SetPage(i - 1);
			CreateReportPage(i);
		}
		
		document.createPDF(Path.Combine(path, reportName + ".pdf"));

		waitSending.SetActive(true);

		yield return new WaitForEndOfFrame();

		smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential("8gftid@gmail.com", "Qwerty2810$$!!");
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback =
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				return true;
			};

		SendEmail("fotowoltaika@gft.pl");
		SendEmail(email);
		if(emailTrader != "")
			SendEmail(emailTrader);

		smtpServer.Dispose();

		waitSending.SetActive(false);

		Debug.Log("Finish create report");
	}

	private void SendEmail(string email)
	{
		//TODO Set cover screan
		MailMessage mail = new MailMessage();

		mail.From = new MailAddress("8gftid@gmail.com");
		try
		{
			mail.To.Add(email);
		}
		catch (Exception e)
		{
			Debug.Log(e.GetBaseException());
			return;
		}
		mail.Subject = "Auto Raport";
		mail.Body = "Ta wiadomość została wygenerowana automatycznie";

		Attachment data = new Attachment(Path.Combine(path, reportName + ".pdf"), System.Net.Mime.MediaTypeNames.Application.Octet);
		mail.Attachments.Add(data);

		try
		{
			smtpServer.Send(mail);
		}
		catch (Exception e)
		{
			Debug.Log(e.GetBaseException());
			return;
		}

		Debug.Log("Sanded Email");

	}

	private void SaveTexture(Texture2D texture, string path)
	{
		var bytes = texture.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
	}
	
	private byte[] GetTextureBytes(Texture2D texture)
	{
		return texture.EncodeToJPG();
	}

	private void CreateReportPage(int pageIndex)
	{
		// Create an empty page
		pdfPage page = document.addPage(3508, 2480);
		
		float textureScale = page.height * 1f / rect.height;
		
		var part = new RenderTexture((int) (rect.width * textureScale), page.height, 24);
		var pathToFile = Path.Combine(path, reportName + pageIndex + ".png");
		cam.targetTexture = part;
		cam.Render();
		cam.targetTexture = null;
		var texture2D = ApplyTexture(part, page, textureScale);

		SaveTexture(texture2D, pathToFile);

		ApplyTextureToPDF(page, texture2D);
	}

	private void ApplyTextureToPDF(pdfPage page, Texture2D image)
	{
		//XGraphics gfx = XGraphics.FromPdfPage(page);
		//XImage image = XImage.FromFile(pathToFile);
		//gfx.DrawImage(image, 0, 0, page.Width, page.Height);
		page.addImage(GetTextureBytes(image), (page.width - image.width) / 2, 0, image.height, image.width);
	}

	private Texture2D ApplyTexture(RenderTexture renderTexture, pdfPage page, float textureScale)
	{
		Texture2D texture2D = new Texture2D((int) (rect.width * textureScale), page.height, TextureFormat.RGB24, false);
		var activeTexture = RenderTexture.active;
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0, 0, rect.width * textureScale, page.height), 0, 0);
		RenderTexture.active = activeTexture;
		texture2D.Apply();
		return texture2D;
	}
}