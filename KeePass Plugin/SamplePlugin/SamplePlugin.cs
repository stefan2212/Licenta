
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using KeePass.Plugins;
using KeePass.Forms;
using KeePass.Resources;

using KeePassLib;
using KeePassLib.Security;
using System.Threading;

namespace SamplePlugin
{

    public sealed class SamplePluginExt : Plugin
    {
        private IPluginHost m_host = null;
        private ToolStripSeparator m_tsSeparator = null;
        private ToolStripMenuItem m_tsmiPopup = null;
        private ToolStripMenuItem m_account = null;
        private ToolStripMenuItem m_register = null;
        private ToolStripTextBox Username = null;
        private ToolStripTextBox Password = null;
        private ToolStripTextBox Username_register = null;
        private ToolStripTextBox Password_register = null;
        private ToolStripTextBox Password2_register = null;
        private ToolStripButton submit = null;
        private ToolStripButton register = null;
        private ToolStripMenuItem m_logout = null;
        private ToolStripMenuItem export = null;
        private ToolStripMenuItem import = null;
        private MessageQueue msg = new MessageQueue();
        private ToolStripItemCollection tsMenu = null;


        public override bool Initialize(IPluginHost host)
        {
            Debug.Assert(host != null);
            if (host == null) return false;
            m_host = host;


            // add a reference to the tools menu in item cointainer
            tsMenu = m_host.MainWindow.ToolsMenu.DropDownItems;


            m_tsSeparator = new ToolStripSeparator();
            tsMenu.Add(m_tsSeparator);

            // add popup menu item

            m_tsmiPopup = new ToolStripMenuItem();
            m_tsmiPopup.Text = "Account";
            tsMenu.Add(m_tsmiPopup);

            // Add  acount menu item
            m_account = new ToolStripMenuItem();
            m_account.Text = "Login";
            m_tsmiPopup.DropDownItems.Add(m_account);

            // Add a register menu item
            m_register = new ToolStripMenuItem();
            m_register.Text = "Register";
            m_register.Click += OnMenuRegister;
            m_tsmiPopup.DropDownItems.Add(m_register);

            submit = new ToolStripButton();
            submit.Text = "Submit";

            register = new ToolStripButton();
            register.Text = "Register";

            DisplayLoging();
            DisplayRegister();

            m_logout = new ToolStripMenuItem();
            m_logout.Text = "Log Out";

            m_logout.Click += OnMenuLogout;

            // Adding items on login

            return true;
        }
        private void DisplayLoging()
        {
            Username = new ToolStripTextBox();
            Password = new ToolStripTextBox();
            Password.TextBox.UseSystemPasswordChar = true;
            
            addLoginItems();
            submit.Click += OnMenuLogin;
        }

        private void DisplayRegister()
        {
            Username_register = new ToolStripTextBox();
            Password_register = new ToolStripTextBox();
            Password2_register = new ToolStripTextBox();
            
            Password_register.TextBox.UseSystemPasswordChar = true;
            Password2_register.TextBox.UseSystemPasswordChar = true;
            
            addRegisterItems();
            register.Click += OnMenuRegister;


        }

        private void OnMenuLogin(object sender, EventArgs e)
        {
            if (!m_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database");
                //m_host.MainWindow.ContextMenu;

            }
            else
            {
                
                if (Username.Text == ""|| Password.Text=="")
                {
                    MessageBox.Show("Please enter credentials");
                }
                else
                {
                    msg.connectToSever();
                    msg.SendMessage("LoginWithUser");
                    export = new ToolStripMenuItem();
                    import = new ToolStripMenuItem();
                    if (LogivValidate() == true) { 
                        MessageBox.Show("You have logged in succesfuly");
                        export.Text = "Export Passwords";
                        tsMenu.Add(export);
                        import.Text = "Import Passwords";
                        tsMenu.Add(import);
                        export.Click += OnMenuExport;
                        import.Click += OnMenuImport;
                        removeLoginItems();
                        m_tsmiPopup.DropDownItems.Add(m_logout);
                    }
                    else
                    {
                        MessageBox.Show("Your username/password are incorrect");
                    }
                   
                }           
            }
        }

        private void OnMenuRegister(object sender, EventArgs e)
        {
            if (!m_host.Database.IsOpen)
            {
                MessageBox.Show("You first need to open a database");
                return;
            }
            else
            {
                msg.connectToSever();
                msg.SendMessage("RegisterWithUser");
                if (RegisterValidate()==true)
                {
                    
                    msg.SendMessage(Username_register.Text);
                    msg.SendMessage(Password_register.Text);
                    MessageBox.Show("You have registered succesfuly");
                }


            }
        }

        private void OnMenuExport(object sender, EventArgs e)
        {
            msg.SendMessage("ExportMyPasswords");
            PwEntry [] pwEntryes = m_host.MainWindow.GetSelectedEntries();
            for(int i=0;i<pwEntryes.Length;i++)
            {
                string title = pwEntryes[i].Strings.Get(PwDefs.TitleField).ReadString();
                string username = pwEntryes[i].Strings.Get(PwDefs.UserNameField).ReadString(); 
                ProtectedString protectedString = pwEntryes[i].Strings.GetSafe(PwDefs.PasswordField);
                byte[] bytes = protectedString.ReadUtf8();
                UTF8Encoding encoding = new UTF8Encoding();
                string passwords = encoding.GetString(bytes);

                msg.SendMessage(title);
                msg.SendMessage(username);
                msg.SendMessage(passwords);
            }
        }

        private void OnMenuImport(object sender, EventArgs e)
        {
            msg.SendMessage("ImportMyPasswords");
            FieldManipulator field = new FieldManipulator();
            string answer = msg.ReciveMessage();
            int length = Int32.Parse(answer);
            List<string> titles = new List<string>();
            List<string> usernames = new List<string>();
            List<string> passwords = new List<string>();
            for (int i = 0; i < length; i++)
            {
                string title = msg.ReciveMessage();
                string username = msg.ReciveMessage();
                string password = msg.ReciveMessage();
                titles.Add(title);
                usernames.Add(username);
                passwords.Add(password);
            }
            for(int i = 0; i < length; i++)
            {
                field.writeEntry(m_host, titles[i], usernames[i], passwords[i]);
                Thread.Sleep(2000);
            }
            msg.closeClient();
            return;
        }

        private void OnMenuLogout(object sender, EventArgs e)
        {
            m_tsmiPopup.DropDownItems.Remove(m_logout);
            Username.Text = "";
            Password.Text = "";
            Username_register.Text = "";
            Password_register.Text = "";
            Password2_register.Text = "";

            m_tsmiPopup.DropDownItems.Add(m_account);
            m_tsmiPopup.DropDownItems.Add(m_register);
            tsMenu.Remove(export);
            tsMenu.Remove(import);
            //msg.closeClient();
            addLoginItems();
            addRegisterItems();
            msg.closeClient();


        }

        public override void Terminate()
        {
            MessageBox.Show("Goodbye");
            msg.closeClient();
        }



        private void m_UrlsRdpConnectMenuItem_Click(object sender, EventArgs e)
        {
            KeePassLib.PwEntry entry = m_host.MainWindow.GetSelectedEntry(false);
            KeePassLib.Security.ProtectedString ps_Username = entry.Strings.Get("UserName");
            KeePassLib.Security.ProtectedString ps_Password = entry.Strings.Get("Password");
            KeePassLib.Security.ProtectedString ps_URL = entry.Strings.Get("URL");
        }

        private void addLoginItems()
        {
            m_account.DropDownItems.Add(Username);
            m_account.DropDownItems.Add(Password);
            m_account.DropDownItems.Add(submit);
        }

        private void addRegisterItems()
        {
            m_register.DropDownItems.Add(Username_register);
            m_register.DropDownItems.Add(Password_register);
            m_register.DropDownItems.Add(Password2_register);
            m_register.DropDownItems.Add(register);
        }

        private void removeLoginItems() {
            m_account.DropDownItems.Remove(Username);
            m_account.DropDownItems.Remove(Password);
            m_account.DropDownItems.Remove(submit);
            m_register.DropDownItems.Remove(Username_register);
            m_register.DropDownItems.Remove(Password_register);
            m_register.DropDownItems.Remove(Password2_register);
            m_register.DropDownItems.Remove(register);
            m_tsmiPopup.DropDownItems.Remove(m_account);
            m_tsmiPopup.DropDownItems.Remove(m_register);

        }

        private bool LogivValidate()
        {
            msg.SendMessage(Username.Text);
            msg.SendMessage(Password.Text);
            string answer = msg.ReciveMessage();
            if("1".CompareTo(answer)==0)
                return true;
            return false;
        }

        private bool RegisterValidate()
        {
            if (Password_register.Text.CompareTo(Password2_register.Text) != 0)
            {
                MessageBox.Show("Passwords do no match");
                return false;
            }
            return true;
        }


    }
}
