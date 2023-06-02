// Importations de namespaces et classes externes
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

        // Constructeur de la classe
        public Http()
        {
        }

        // Cette méthode récupère des données à partir d'une API en utilisant une requête HTTP GET et envoie une requête SQL dans l'en-tête de la demande
        public async Task<List<Dictionary<string, string>>> get(string sql)
        {
            // Initialisation d'une variable qui stocke les données JSON récupérées de l'API
            List<Dictionary<string, string>> jsonData = null;

            // Création d'un objet HttpRequestMessage avec la méthode HTTP GET et l'URL de l'API cible
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getValeurs");

            // Ajout de la requête SQL dans l'en-tête de la demande
            requestMessage.Headers.Add("sql", sql);

            HttpResponseMessage response;
            try
            {
                // Envoie de la demande et réception de la réponse
                response = await client.SendAsync(requestMessage);

                // Vérification si la réponse a été réussie
                if (response.IsSuccessStatusCode)
                {

                    // Récupération de la taille totale des octets de la réponse
                    var totalBytes = response.Content.Headers.ContentLength;

                    // Initialisation d'une variable qui stocke le nombre d'octets lus
                    var bytesRead = 0L;

                    // Lecture de la réponse de manière asynchrone
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[4096];
                        var read = 0;

                        // Boucle de lecture de la réponse
                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            bytesRead += read;
                            // Calcul du pourcentage de progression de la lecture de la réponse
                            var progress = (int)((double)bytesRead / (double)totalBytes * 100.0);
                        }

                        // Conversion de la réponse de JSON en une liste de dictionnaires clé-valeur
                        string stringData = await response.Content.ReadAsStringAsync();
                        jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
                    }
                }
                else
                {
                    // Si la réponse n'est pas réussie, retourne la variable jsonData (qui est null)
                    return jsonData;
                }
            } catch(Exception ex) {
                // En cas d'erreur, retourne la variable jsonData (qui est null)
                return jsonData;
            }

            int x = 0;
            int c = jsonData.Count;
            int last = 0;
            // Conversion des unités de mesure des données récupérées
            foreach (Dictionary<string, string> data in jsonData)
            {
                if (x++ * 100 / c - last > 4)
                {
                    last = x++ * 100 / c;
                    loadingBar.refresh(last);
                    Console.WriteLine(last);
                }
                Dictionary<string, Int32> values = new Dictionary<string, Int32>();
                values.Add("Temperature", Convert.ToInt32(convertToIntString(data["Temperature"])));
                values.Add("Hygrometrie", Convert.ToInt32(convertToIntString(data["Hygrometrie"])));
                values.Add("VitesseVent", Convert.ToInt32(convertToIntString(data["VitesseVent"])));
                values.Add("PressionAtmospherique", Convert.ToInt32(convertToIntString(data["PressionAtmospherique"])));
                values.Add("Pluviometre", Convert.ToInt32(convertToIntString(data["Pluviometre"])));
                values.Add("RayonnementSolaire", Convert.ToInt32(convertToIntString(data["RayonnementSolaire"])));

                // Conversion des unités de mesure pour chaque type de donnée
                data["Temperature"] = new unitConversion().Conversion(values["Temperature"], (string)App.Current.Properties["unitTemp"], values).ToString();
                data["Hygrometrie"] = new unitConversion().Conversion(values["Hygrometrie"], (string)App.Current.Properties["unitHygro"], values).ToString();
                data["VitesseVent"] = new unitConversion().Conversion(values["VitesseVent"], (string)App.Current.Properties["unitVvent"], values).ToString();
                data["PressionAtmospherique"] = new unitConversion().Conversion(values["PressionAtmospherique"], (string)App.Current.Properties["unitPresAtmo"], values).ToString();
                data["Pluviometre"] = new unitConversion().Conversion(values["Pluviometre"], (string)App.Current.Properties["unitPluv"], values).ToString();
                data["RayonnementSolaire"] = new unitConversion().Conversion(values["RayonnementSolaire"], (string)App.Current.Properties["unitRaySol"], values).ToString();
            }

            // Retourne la variable jsonData
            return jsonData;
        }

        // Cette fonction est utilisée pour extraire la partie entière d'une chaîne qui peut contenir un point décimal
        private string convertToIntString(string text)
        {
            // Modèle de chaîne pour extraire la partie entière
            string pattern = @"^(.*?)\.";

            // Effectue la correspondance avec le modèle
            Match match = Regex.Match(text, pattern);

            if (match.Success)
            {
                // Récupère la partie entière correspondant au modèle
                text = match.Groups[1].Value;
            }

            // Retourne la partie entière de la chaîne
            return text;
        }

        // Cette méthode renvoie une date sous forme de chaîne de caractères
        public async Task<string> getDate(string minOrMax)
        {
            // Initialisation d'une liste de dictionnaires
            List<Dictionary<string, string>> jsonData = null;

            // Création d'un nouvel objet HttpRequestMessage avec la méthode HTTP GET et une URL
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getDate");

            // Ajout du paramètre minOrMax à la collection Headers de l'objet requestMessage
            requestMessage.Headers.Add("minOrMax", minOrMax);

            HttpResponseMessage response;
            try
            {
                // Envoi de la requête HTTP asynchrone au serveur et attente d'une réponse
                response = await client.SendAsync(requestMessage);
            }
            catch (Exception ex)
            {
                // Si une exception est levée, renvoie une chaîne vide
                return "";
            }

            // Si le serveur a renvoyé une réponse avec succès
            if (response.IsSuccessStatusCode)
            {
                // Lecture du contenu de la réponse et désérialisation des données JSON en une liste de dictionnaires
                string stringData = await response.Content.ReadAsStringAsync();
                jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
            }

            try
            {
                // Récupération de la première valeur associée à la clé "0" du premier dictionnaire de la liste jsonData
                return jsonData[0]["0"];
            }
            catch (Exception ex)
            {
                // Si une exception est levée, renvoie une chaîne vide
                return "";
            }
        }

        // Récupérer un compte en fonction du login et du mot de passe
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
                    // Si la requête réussit, on récupère les données de la réponse
                    string stringData = await response.Content.ReadAsStringAsync();
                    jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringData);
                }
                return jsonData;

                // Si une erreur se produit, on renvoie null
            } catch (Exception ex) { return jsonData; }
        }

        // Récupérer tous les comptes
        public async Task<List<Dictionary<string, string>>> getAllCompte(string login, string password)
        {
            List<Dictionary<string, string>> jsonData = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getComptes");
            requestMessage.Headers.Add("login", login);
            requestMessage.Headers.Add("password", password);

            // Envoyer la requête et récupérer la réponse
            HttpResponseMessage response = await client.SendAsync(requestMessage);

            // Si la requête réussit, on récupère les données de la réponse
            if (response.IsSuccessStatusCode)
            {
                string stringData = await response.Content.ReadAsStringAsync();
                jsonData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData);
            }
            return jsonData;
        }

        // Méthode pour éditer un compte existant
        public async Task<bool> editCompte(string login, string password, Dictionary<string, string> profil)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/editCompte");

            // Ajout des headers dans la requête pour l'authentification et les informations de profil
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

        // Méthode pour créer un nouveau compte
        public async Task<bool> newCompte(string login, string password, Dictionary<string, string> profil)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/newCompte");

            // Ajout des headers dans la requête pour l'authentification et les informations de profil
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

        // Cette méthode supprime un compte
        public async Task<bool> deleteCompte(string login, string password, string id)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/deleteCompte");

            // Ajoute le login de l'utilisateur à la requête
            requestMessage.Headers.Add("login", login);
            // Ajoute le mot de passe de l'utilisateur à la requête
            requestMessage.Headers.Add("password", password);
            // Ajoute l'identifiant du compte à supprimer à la requête
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

        // Cette méthode permet de modifier les seuils
        public async Task<bool> editSeuil(string login, string password, string niv, string target, int var)
        {
            bool saved = false;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/editSeuil");

            // Ajoute le login de l'utilisateur
            requestMessage.Headers.Add("login", login);
            // Ajoute le mot de passe de l'utilisateur
            requestMessage.Headers.Add("password", password);
            // Ajoute le niveau du seuil à modifier
            requestMessage.Headers.Add("niv", niv);
            // Ajoute la cible du seuil à modifier
            requestMessage.Headers.Add("target", target);
            // Ajoute la nouvelle valeur du seuil à modifier
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

        // Cette méthode permet de récupérer les seuils de l'API
        public async Task<List<string>> getSeuils()
        {
            List<string> saved = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getSeuils");

            try
            {
                // On envoie la requête à l'API
                HttpResponseMessage response = await client.SendAsync(requestMessage);
                
                // Si la réponse est valide
                if (response.IsSuccessStatusCode)
                {
                    // On récupère la réponse en format JSON
                    string stringData = await response.Content.ReadAsStringAsync();
                    
                    // On convertit la réponse en liste de string
                    saved = JsonConvert.DeserializeObject<List<string>>(stringData);
                }
                return saved;
            }
            catch (Exception ex) { return saved; }
        }

        // Cette méthode permet de récupérer les prévisions météorologiques de l'API
        public async Task<List<Dictionary<string, string>>> getPrevisions()
        {
            List<Dictionary<string, string>> saved = null;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://{((string)App.Current.Properties["serverIp"])}/projet2023stationmeteo/api/getPrevisions");

            try
            {
                // On envoie la requête à l'API
                HttpResponseMessage response = await client.SendAsync(requestMessage);

                // Si la réponse est valide
                if (response.IsSuccessStatusCode)
                {
                    // On récupère la réponse en format JSON
                    string stringData = await response.Content.ReadAsStringAsync();

                    // On convertit la réponse en liste de dictionnaires
                    saved = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(stringData); ;
                }
                return saved;
            }
            catch (Exception ex) { return saved; }
        }
    }
}