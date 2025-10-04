using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmailApp : BaseApp
{
    [SerializeField] public Transform emailHolder;
    [SerializeField] private GameObject emailItemViewPrefab;
    [SerializeField] private EmailContentsView emailContentsView;

    private List<EmailItemView> _emailItemViews = new();

    private List<EmailConfig> _unlockedEmails = new();

    public List<EmailConfig> emailConfigs;

    private void Start()
    {
        emailConfigs = Email.LoadEmailConfigs();
        UnlockEmailFromConsole("Foo", 0);
    }


    public void UnlockEmailFromConsole(string npcName, int id)
    {
        EmailConfig email = emailConfigs.Find(r => r.Sender == npcName && r.friendLevelToUnlock == id);
        UnlockEmail(email);
    }

    public void UnlockEmail(EmailConfig email)
    {

        if (!_unlockedEmails.Contains(email))
        {
            _unlockedEmails.Add(email);
            var newEmailItem = Instantiate(emailItemViewPrefab);
            newEmailItem.transform.SetParent(emailHolder);
            newEmailItem.transform.localScale = Vector3.one;
            _emailItemViews.Add(newEmailItem.GetComponent<EmailItemView>());
            _emailItemViews.Last().SetEmailInfo(email, this);

        }


    }

    public void OpenEmailView(EmailConfig emailConfig)
    {
        emailContentsView.gameObject.SetActive(true);
        emailContentsView.SetEmail(emailConfig);
    }

    protected override void HandleAppClose()
    {
        emailContentsView.gameObject.SetActive(false);
    }

}
