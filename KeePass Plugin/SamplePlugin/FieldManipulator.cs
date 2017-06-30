using KeePass.Plugins;
using KeePass.Forms;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePlugin
{
    class FieldManipulator
    {
        public FieldManipulator()
        {

        }

        public void writeEntry(IPluginHost m_host,string title, string username, string password)
        {
            PwEntry entry = new PwEntry(true, true);
            entry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, title));
            entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, username));
            entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(false, password));

            m_host.Database.RootGroup.AddEntry(entry, true);
            m_host.MainWindow.UpdateUI(false, null, true, m_host.Database.RootGroup, true, null, true);

        }

       

    }
}
