using UnityEngine;
using TMPro;

public class EmailContentsView : MonoBehaviour
{
    [SerializeField] TMP_Text emailNameText;
    [SerializeField] TMP_Text emailContentsText;

    public void SetEmail(EmailConfig email)
    {
        emailNameText.text = email.EmailName;
        emailContentsText.text = email.Contents;
    }
}
