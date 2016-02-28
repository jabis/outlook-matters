﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.Core;

namespace OutlookMatters
{
    [ComVisible(true)]
    public class MailItemContextMenuEntry : Office.IRibbonExtensibility
    {
        private readonly IMailExplorer _explorer;
        private readonly IMattermost _mattermost;
        private readonly ISettingsProvider _settingsProvider;

        private ISession _session;
        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = _mattermost.LoginByUsername(
                                _settingsProvider.Url,
                                _settingsProvider.TeamId,
                                _settingsProvider.Username,
                                _settingsProvider.Password);
                }
                return _session;
            }
        }

        public MailItemContextMenuEntry(IMailExplorer explorer, IMattermost mattermost, ISettingsProvider settingsProvider)
        {
            _explorer = explorer;
            _mattermost = mattermost;
            _settingsProvider = settingsProvider;
        }

        public string GetCustomUI(string ribbonId)
        {
            switch (ribbonId)
            {
                case "Microsoft.Outlook.Explorer":
                    return GetResourceText("OutlookMatters.MailItemContextMenuEntry.xml");
                default:
                    return string.Empty;
            }
        }

        public void OnSettingsClick(Office.IRibbonControl control)
        {
            var window = new View.SettingsWindow();
            window.ShowDialog();
        }

        public void OnPostClick(Office.IRibbonControl control)
        {
            var channelId = _settingsProvider.ChannelId;
            var mailbody = _explorer.GetSelectedMailBody();

            Session.CreatePost(channelId, mailbody);
        }
        
        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}