using Newtonsoft.Json;
using projet23_Station_météo_WPF.code;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Text.RegularExpressions;

namespace projet23_Station_météo_WPF.UserControls
{
    public class Http
    {
        static HttpClient client = new HttpClient();
        public Http()
        {
            //client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<Dictionary<string, string>>> get(string sql)
        {
            // requete avec sql dans le Header
            List<Dictionary<string, string>> jsonData = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getValeurs");
            requestMessage.Headers.Add("sql", sql);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(requestMessage);
                // conversion de string to Json
                if (response.IsSuccessStatusCode)
                {
                    var totalBytes = response.Content.Headers.ContentLength;
                    var bytesRead = 0L;

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[4096];
                        var read = 0;

                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            bytesRead += read;
                            var progress = (int)((double)bytesRead / (double)totalBytes * 100.0);
                            //Console.WriteLine($"Progress: {progress}%");
                        }

                        string stringData = await response.Content.ReadAsStringAsync();
                        jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
                    }
                }
                else
                {
                    return jsonData;
                }
            } catch(Exception ex) {
                return jsonData;
            }


            
            // conversions
            foreach (Dictionary<string, string> data in jsonData)
            {
                Dictionary<string, Int32> values = new Dictionary<string, Int32>();
                values.Add("Temperature", Convert.ToInt32(convertToIntString(data["Temperature"])));
                values.Add("Hygrometrie", Convert.ToInt32(convertToIntString(data["Hygrometrie"])));
                values.Add("VitesseVent", Convert.ToInt32(convertToIntString(data["VitesseVent"])));
                values.Add("PressionAtmospherique", Convert.ToInt32(convertToIntString(data["PressionAtmospherique"])));
                values.Add("Pluviometre", Convert.ToInt32(convertToIntString(data["Pluviometre"])));
                values.Add("RayonnementSolaire", Convert.ToInt32(convertToIntString(data["RayonnementSolaire"])));


                data["Temperature"] = new unitConversion().Conversion(values["Temperature"], (string)App.Current.Properties["unitTemp"], values).ToString();
                data["Hygrometrie"] = new unitConversion().Conversion(values["Hygrometrie"], (string)App.Current.Properties["unitHygro"], values).ToString();
                data["VitesseVent"] = new unitConversion().Conversion(values["VitesseVent"], (string)App.Current.Properties["unitVvent"], values).ToString();
                data["PressionAtmospherique"] = new unitConversion().Conversion(values["PressionAtmospherique"], (string)App.Current.Properties["unitPresAtmo"], values).ToString();
                data["Pluviometre"] = new unitConversion().Conversion(values["Pluviometre"], (string)App.Current.Properties["unitPluv"], values).ToString();
                data["RayonnementSolaire"] = new unitConversion().Conversion(values["RayonnementSolaire"], (string)App.Current.Properties["unitRaySol"], values).ToString();
            }

            return jsonData;
        }
        private string convertToIntString(string text)
        {
            string pattern = @"^(.*?)\.";

            Match match = Regex.Match(text, pattern);

            if (match.Success)
            {
                text = match.Groups[1].Value;
            }

            return text;
        }
        public async Task<string> getDate(string minOrMax)
        {
            //in progress
            List<Dictionary<string, string>> jsonData = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getDate");
            requestMessage.Headers.Add("minOrMax", minOrMax);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                return "";
            }

            if (response.IsSuccessStatusCode)
            {
                string stringData = await response.Content.ReadAsStringAsync();
                jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
            }

            try
            {
                return jsonData[0]["0"];
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public async Task<Dictionary<string, string>> getCompte(string login, string password)
        {
            Dictionary<string, string> jsonData = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/connexion");
            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage); if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringData);
                }
                return jsonData;
            } catch (Exception ex) { return jsonData; }
        }
        public async Task<List<Dictionary<string, string>>> getAllCompte(string login, string password)
        {
            List<Dictionary<string, string>> jsonData = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getComptes");
            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);
            HttpResponseMessage response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                string stringData = await response.Content.ReadAsStringAsync();
                jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
            }
            return jsonData;
        }
        public async Task<bool> editCompte(string login, string password, Dictionary<string, string> profil)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/editCompte");
            
            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);
            requestMessage.Headers.Add("ID", profil["ID"]);
            requestMessage.Headers.Add("Nom", profil["Nom"]);
            requestMessage.Headers.Add("Prenom", profil["Prenom"]);
            requestMessage.Headers.Add("Identifiant", profil["Identifiant"]);
            if (profil["Mdp"] != null) requestMessage.Headers.Add("Mdp", profil["Mdp"]);
            requestMessage.Headers.Add("Tel", profil["Tel"]);
            requestMessage.Headers.Add("Fonction", profil["Fonction"]);
            requestMessage.Headers.Add("Droit", profil["Droit"]);

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = bool.Parse(stringData);
                }
                return saved;
            } catch (Exception ex) { return saved; }
        }
        public async Task<bool> newCompte(string login, string password, Dictionary<string, string> profil)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/newCompte");

            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);
            requestMessage.Headers.Add("Nom", profil["Nom"]);
            requestMessage.Headers.Add("Prenom", profil["Prenom"]);
            requestMessage.Headers.Add("Identifiant", profil["Identifiant"]);
            requestMessage.Headers.Add("Mdp", profil["Mdp"]);
            requestMessage.Headers.Add("Tel", profil["Tel"]);
            requestMessage.Headers.Add("Fonction", profil["Fonction"]);
            requestMessage.Headers.Add("Droit", profil["Droit"]);

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = bool.Parse(stringData);
                }
                return saved;
            } catch (Exception ex) { return saved; }
        }
        public async Task<bool> deleteCompte(string login, string password, string id)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/deleteCompte");

            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);
            requestMessage.Headers.Add("ID", id);

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = bool.Parse(stringData);
                }
                return saved;
            } catch(Exception ex) { return saved; }
        }
        public async Task<bool> editSeuil(string login, string password, string niv, string target, int var)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/editSeuil");

            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);
            requestMessage.Headers.Add("niv", niv);
            requestMessage.Headers.Add("target", target);
            requestMessage.Headers.Add("var", var.ToString());

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = bool.Parse(stringData);
                }
                return saved;
            } catch(Exception ex) { return saved; }
        }
        public async Task<List<string>> getSeuils()
        {
            List<string> saved = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getSeuils");

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = JsonConvert.DeserializeObject<List<string>>(stringData); ;
                }
                return saved;
            }
            catch (Exception ex) { return saved; }
        }

        public async Task<List<Dictionary<string, string>>> getPrevisions()
        {
            List<Dictionary<string, string>> saved = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getPrevisions");

            try
            {
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    string stringData = await response.Content.ReadAsStringAsync();
                    saved = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData); ;
                }
                return saved;
            }
            catch (Exception ex) { return saved; }
        }
    }
}