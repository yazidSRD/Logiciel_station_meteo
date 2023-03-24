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
            new Thread(async () => {
                if (await waitToWriteFile(resxFilePath) == true)
                {
                    try
                    {
                        using (var stream = new FileStream(this.resxFilePath, FileMode.Open, FileAccess.ReadWrite))
                        {
                            // Tente d'acquérir un verrou sur le fichier
                            stream.Lock(0, stream.Length);

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
            App.Current.Properties[resName] = newValue;
            return true;
        }
        private async Task<bool> waitToWriteFile(string filePath, int timeoutMs = 5000)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (!stream.CanRead)
                    {
                        throw new IOException("Cannot read file: " + filePath);
                    }

                    // Tente d'acquérir un verrou sur le fichier
                    stream.Lock(0, stream.Length);

                    using (var reader = new StreamReader(stream))
                    {
                        // Lecture du contenu du fichier
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
