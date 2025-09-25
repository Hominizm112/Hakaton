using UnityEngine;
using TMPro;

public class EmailItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text emailViewNameText;
    [SerializeField] private TMP_Text emailViewDescriptionText;

    private EmailApp _emailApp;
    private EmailConfig _emailConfig;

    private void Awake()
    {
        GetComponent<BaseButtonExtended>().OnButtonClick += HandleClick;
    }

    public void SetEmailInfo(EmailConfig emailConfig, EmailApp emailApp)
    {

        emailViewNameText.text = emailConfig.EmailName;
        emailViewDescriptionText.text = emailConfig.Contents;

        _emailApp = emailApp;
        _emailConfig = emailConfig;
    }

    public void HandleClick()
    {
        _emailApp.OpenEmailView(_emailConfig);
    }
}
