using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Deployment.Application;
using Microsoft.Win32;
using System.Net.NetworkInformation;

public struct User
{
    public string adeli, limite;
    public string name, prenom, pwd;
}

namespace orthoStereogram
{
    public partial class mainForm : Form
    {
        public User user = new User();

        public mainForm()
        {
            InitializeComponent();

            //Check for user auth
            CheckRegistryEntry();
            //Get assembly version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            //Adapt form title
            this.Text = "orthoStéréogram v" + CurrentVersion + " - " + user.name + " " + user.prenom;

            
        }

        public string CurrentVersion
        {
            get
            {
                return ApplicationDeployment.IsNetworkDeployed
                       ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString()
                       : Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public void CheckRegistryEntry()
        {
            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\orthoStereogram");  
              
            //if it does exist, retrieve the stored values  
            if (key != null)  
            {  
                user.limite     = key.GetValue("limite").ToString();  
                user.name       = key.GetValue("name").ToString();
                user.prenom     = key.GetValue("prenom").ToString();
                user.pwd        = key.GetValue("pwd").ToString();
                key.Close();  
            }
            else
            {
                user.name   = "DEMO";
                user.prenom = "";
                this.connexionServeurToolStripMenuItem.Enabled = true;
            }
        }

        public string GetMacAddress()
        {
            string addr = "";
            foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (n.OperationalStatus == OperationalStatus.Up)
                {
                    addr += n.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return addr;
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Connect to serveur to check auths
        private void connexionServeurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DBConnect fmaillet = new DBConnect();

            user = fmaillet.CheckAdeli("319203121", "12345");
            Console.WriteLine(user.name);
            Console.WriteLine(user.prenom);
            Console.WriteLine(user.adeli);
            Console.WriteLine(user.pwd);
            Console.WriteLine(user.limite);

            DateTime lDate = Convert.ToDateTime(user.limite);
            DateTime oDate = DateTime.Now;

            Console.WriteLine();
            Console.WriteLine(oDate.ToString());
            Console.WriteLine(lDate.ToString());

            if (DateTime.Compare(oDate, lDate) > 0)
            {
                return;
            }

            //Write registry
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\orthoStereogram");

            //storing the values  
            key.SetValue("limite", user.limite);
            key.SetValue("name", user.name);
            key.SetValue("prenom", user.prenom);
            key.SetValue("pwd", user.pwd);
            key.SetValue("adeli", user.adeli);
            key.Close();

            this.connexionServeurToolStripMenuItem.Enabled = false;
            //Adapt form title
            this.Text = "orthoStéréogram v" + CurrentVersion + " - " + user.name + " " + user.prenom;
            return;
        }

        private void fichiersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void InstallUpdateSyncWithInfo()
        {
            UpdateCheckInfo info = null;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                try
                {
                    info = ad.CheckForDetailedUpdate();

                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("Impossible d'effectuer la mise à jour. \n\nVérifiez votre connexion internet, ou réessayez plus tard. Erreur: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Impossible de vérifier la mise à jour. ClickOnce deployment incorrect. Merci de réinstaller l'application. Erreur: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }

                if (info.UpdateAvailable)
                {
                    Boolean doUpdate = true;

                    if (!info.IsUpdateRequired)
                    {
                        DialogResult dr = MessageBox.Show("Une mise à jour est disponible. Désirez-vous installer cette mise à jour maintenant ?", "Mise à jour disponible", MessageBoxButtons.OKCancel);
                        if (!(DialogResult.OK == dr))
                        {
                            doUpdate = false;
                        }
                    }
                    else
                    {
                        // Display a message that the app MUST reboot. Display the minimum required version.
                        MessageBox.Show("Une mise à jour importante est disponible : " +
                            "version " + info.MinimumRequiredVersion.ToString() +
                            ". Cette mise à jour va s'installer et le programem va redémarrer.",
                            "Mise à jour disponible", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    if (doUpdate)
                    {
                        try
                        {
                            ad.Update();
                            MessageBox.Show("Le programme a été mis à jour, et va redémarrer.");
                            Application.Restart();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            MessageBox.Show("Impossible d'effectuer la mise à jour. \n\nVérifiez votre connexion internet, ou réessayez plus tard. Erreur: " + dde);
                            return;
                        }
                    }
                }
            }
        }

        private void miseÀJourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InstallUpdateSyncWithInfo();
        }

        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }
    }

    
}
