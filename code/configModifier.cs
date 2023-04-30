// Importations de namespaces et classes externes
using projet23_Station_météo_WPF.ressources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.Design;
using System.Xml;

namespace projet23_Station_météo_WPF.code
{
    internal class configModifier
    {
        string resxFilePath = @"./ressources/Configs.resx";

        public async Task<bool> edit(string resName, string newValue)
        {
            // Démarre une nouvelle thread pour éditer le fichier .resx
            new Thread(async () => {
                // Attend que le fichier soit accessible en écriture
                if (await waitToWriteFile(resxFilePath) == true)
                {
                    try
                    {
                        // Ouvre le fichier .resx en mode lecture/écriture
                        using (var stream = new FileStream(this.resxFilePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            // Tente d'acquérir un verrou sur le fichier
                            stream.Lock(0, stream.Length);

                            // Charge le fichier .resx dans un objet XmlDocument
                            var resxDoc = new XmlDocument();
                            resxDoc.Load(stream);

                            // Recherche la ressource dans le document XML
                            XmlNode node = resxDoc.SelectSingleNode("//data[@name='" + resName + "']/value");

                            if (node != null)
                            {
                                // Met à jour la valeur de la ressource
                                node.InnerText = newValue;

                                // Enregistre le fichier .resx
                                stream.Seek(0, SeekOrigin.Begin);
                                resxDoc.Save(stream);

                                // Réduit la longueur du fichier
                                stream.SetLength(stream.Position);

                                // Libération du verrou sur le fichier
                                stream.Unlock(0, stream.Length);
                            }
                        }
                    }
                    catch (Exception e) { };
                }
            }).Start();

            // Met à jour la valeur de la propriété correspondante dans les propriétés de l'application
            App.Current.Properties[resName] = newValue;
            return true;
        }

        // return un booléen qui indique si l'écriture dans le fichier est possible (true) ou non (false)
        private async Task<bool> waitToWriteFile(string filePath, int timeoutMs = 5000)
        {
            try
            {
                // Ouverture d'un flux en lecture du fichier
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // Vérification de la possibilité de lire le fichier
                    if (!stream.CanRead)
                    {

                        throw new IOException("Cannot read file: " + filePath);
                    }

                    // Tente d'acquérir un verrou sur le fichier
                    stream.Lock(0, stream.Length);

                    // Lecture du contenu du fichier à l'aide d'un StreamReader
                    using (var reader = new StreamReader(stream))
                    {
                        var contents = await reader.ReadToEndAsync();

                        // Libération du verrou sur le fichier
                        stream.Unlock(0, stream.Length);

                        return true;
                    }
                }
            }
            catch (IOException)
            {
                // Si la tentative d'acquisition du verrou a échoué, on attend un peu et on réessaie
                await Task.Delay(100);

                if (timeoutMs > 0)
                {
                    return await waitToWriteFile(filePath, timeoutMs - 100);
                }
                else
                {
                    return false;
                }
            }
        }

        // Cette méthode est utilisée pour charger les propriétés de l'application à partir d'un fichier resx
        public async void load()
        {

            using (ResXResourceSet resxSet = new ResXResourceSet(resxFilePath))
            {
                App.Current.Properties["colorSeuils"] = resxSet.GetString("colorSeuils");
                App.Current.Properties["saveIdentifiant"] = resxSet.GetString("saveIdentifiant");
                App.Current.Properties["saveMdp"] = resxSet.GetString("saveMdp");
                App.Current.Properties["refreshTimer"] = resxSet.GetString("refreshTimer");
                App.Current.Properties["serverIp"] = resxSet.GetString("serverIp");
                App.Current.Properties["seuilLvl1"] = resxSet.GetString("seuilLvl1");
                App.Current.Properties["seuilLvl2"] = resxSet.GetString("seuilLvl2");
                App.Current.Properties["unitDvent"] = resxSet.GetString("unitDvent");
                App.Current.Properties["unitHygro"] = resxSet.GetString("unitHygro");
                App.Current.Properties["unitPluv"] = resxSet.GetString("unitPluv");
                App.Current.Properties["unitPresAtmo"] = resxSet.GetString("unitPresAtmo");
                App.Current.Properties["unitRaySol"] = resxSet.GetString("unitRaySol");
                App.Current.Properties["unitTemp"] = resxSet.GetString("unitTemp");
                App.Current.Properties["unitVvent"] = resxSet.GetString("unitVvent");
            }
        }
    }
}
