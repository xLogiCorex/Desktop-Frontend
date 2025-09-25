using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace WPF_Admin_Front
{
    public class ServerConnection
    {
        private HttpClient client = new HttpClient();
        private string baseURL = "http://127.1.1.1:3000";
        private string jwtToken;
        public ServerConnection(string url)
        {
            baseURL = url;
        }

        public async Task<object> connect(string urlString, string methodType, string jsonString = null)
        {
            string url = baseURL + urlString;
            SetAuthHeader();
            string valaszText = ""; ;
            if ((methodType.ToLower() == "delete" || methodType.ToLower() == "get") && jsonString != null)
            {
                MessageBox.Show("Get és delete esetén nem küldhető adat");
                return null;
            }
            try
            {
                HttpResponseMessage valasz = new HttpResponseMessage();
                if (jsonString != null)
                {
                    StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "Application/JSON");
                    if (methodType.ToLower() == "post")
                    {
                        valasz = await client.PostAsync(url, sendThis);
                    }
                    else if (methodType.ToLower() == "put")
                    {
                        valasz = await client.PutAsync(url, sendThis);
                    }

                }
                else
                {
                    if (methodType.ToLower() == "get")
                    {
                        valasz = await client.GetAsync(url);
                    }
                    else if (methodType.ToLower() == "delete")
                    {
                        valasz = await client.DeleteAsync(url);
                    }
                }
                // minden method type elküldte a kérést a szervernek, utána...
                valaszText = await valasz.Content.ReadAsStringAsync();  // kiolvassuk a hibaüzenetet
                valasz.EnsureSuccessStatusCode();   // és utána kiolvassuk a státusz kódot. Ha 4xx vagy 5xx, akkor ugrik a catch-be.
                return valaszText;
            }
            catch (Exception e)
            {
                Message message = JsonConvert.DeserializeObject<Message>(valaszText);
                MessageBox.Show(message.message);
                return null;
            }
            return null;
        }

        // ---------------------------------------LOGIN-----------------------------------------------

        public async Task<User> Login(string email, string password)
        {
            User oneUser = new User() { email = null };
            try
            {
                var jsonData = new
                {
                    newEmail = email,
                    newPassword = password,
                };

                string data = JsonConvert.SerializeObject(jsonData);
                object something = await connect("/login", "post", data);

                if (something is null)
                {
                    // nem volt sikeres a bejelentkezés
                    return oneUser;
                }
                loginData response = JsonConvert.DeserializeObject<loginData>(something as string);

                //token
                jwtToken = response.token;

                oneUser.email = email;
                oneUser.password = password;
                oneUser.role = response.role;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return oneUser;
        }

        // Token hitelesítés
        private void SetAuthHeader()
        {
            //client.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(jwtToken) && client.DefaultRequestHeaders.Authorization == null)      // ha a tokenem tartalmaz adatot és a client nem tartalmaz semmit, tehát null.
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }


        // Logout

        public void Logout()
        {
            jwtToken = null;
            client.DefaultRequestHeaders.Authorization = null;
        }


        // ---------------------------------------TERMÉKEK-----------------------------------------------


        //Get Products
        public async Task<List<Product>> GetProduct()
        {
            List<Product> productList = new List<Product>();
            string url = "/products";
            try
            {
                object response = await connect(url, "get");
                if (response is null)
                {
                    return productList;     // ez egy üres product list, mert hibás volt a lekérés        
                }
                productList = JsonConvert.DeserializeObject<List<Product>>(response as string);
            }
            catch (Exception e) {MessageBox.Show(e.Message); }
            return productList;
        }

        //Post Products
        public async Task<bool> PostProduct(Product oneProduct)
        {
            SetAuthHeader();
            string url = "/products";
            try
            {
                var jsonData = new
                {
                    newSku = oneProduct.sku,
                    newName = oneProduct.name,
                    newCategoryId = oneProduct.categoryId,
                    newSubcategoryId = oneProduct.subcategoryId,
                    newUnit = oneProduct.unit,
                    newPrice = oneProduct.price,
                    newStockQuantity = oneProduct.stockQuantity,
                    newMinStockLevel = oneProduct.minStockLevel,
                    newIsActive = oneProduct.isActive
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);
                object response = await connect(url, "post", jsonString);
                if (response is null)
                {
                    return false;
                }
                Message message = JsonConvert.DeserializeObject<Message>(response as string);
                MessageBox.Show(message.message);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }


        // ---------------------------------------Felhasználók-----------------------------------------------


        //Get Users
        public async Task<List<User>> GetUsers()
        {
            SetAuthHeader();
            List<User> userList = new List<User>();
            string url = baseURL + "/users";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                userList = JsonConvert.DeserializeObject<List<User>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return userList;
        }

        //Post Users
        public async Task<bool> PostUser(User oneUser)
        {
            SetAuthHeader();
            string url = baseURL + "/register";
            try
            {
                var jsonData = new
                {
                    newName = oneUser.name,
                    newEmail = oneUser.email,
                    newPassword = oneUser.password,
                    newRole = oneUser.role,
                    newIsActive = oneUser.isActive
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Message errorUser = JsonConvert.DeserializeObject<Message>(errorMessage);
                    MessageBox.Show(errorUser.message, "Hiba a felhasználó létrehozásakor.");
                    return false;
                }
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Message successUser = JsonConvert.DeserializeObject<Message>(responseString);
                MessageBox.Show(successUser.message, "Felhasználó sikeresen létrehozva.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        // user isActive frissítése
        public async Task<bool> UpdateUserStatus(Guid userId, bool isActive)
        {
            SetAuthHeader();
            string url = baseURL + $"/users/{userId}/status";
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { isActive }),
                    Encoding.UTF8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PutAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a státusz módosításakor: {ex.Message}");
                return false;
            }
        }


        // ---------------------------------------Partnerek-----------------------------------------------


        //Get Partners
        public async Task<List<Partner>> GetPartners()
        {
            SetAuthHeader();
            List<Partner> partnerList = new List<Partner>();
            string url = baseURL + "/partners";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                partnerList = JsonConvert.DeserializeObject<List<Partner>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return partnerList;
        }

        //Post Partners
        public async Task<bool> PostPartner(Partner onePartner)
        {
            SetAuthHeader();
            string url = baseURL + "/partners";
            try
            {
                var jsonData = new
                {
                    newName = onePartner.name,
                    newTaxNumber = onePartner.taxNumber,
                    newAddress = onePartner.address,
                    newContactPerson = onePartner.contactPerson,
                    newEmail = onePartner.email,
                    newPhone = onePartner.phone,
                    newIsActive = onePartner.isActive
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Message errorPartner = JsonConvert.DeserializeObject<Message>(errorMessage);
                    MessageBox.Show(errorPartner.message, "Hiba a partner létrehozásakor.");
                    return false;
                }
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Message successPartner = JsonConvert.DeserializeObject<Message>(responseString);
                MessageBox.Show(successPartner.message, "Partner sikeresen létrehozva.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        
        // ---------------------------------------Kategóriák-----------------------------------------------


        //Get Kategória
        public async Task<List<Category>> GetCategories()
        {
            SetAuthHeader();
            List<Category> categoryList = new List<Category>();
            string url = baseURL + "/categories";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<Category>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return categoryList;
        }

        public async Task<bool> PostCategory(Category oneCategory)
        {
            SetAuthHeader();
            string url = baseURL + "/categories";
            try
            {
                var jsonData = new
                {
                    newName = oneCategory.name,
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Message errorCategory = JsonConvert.DeserializeObject<Message>(errorMessage);
                    MessageBox.Show(errorCategory.message, "Hiba a kategória létrehozásakor.");
                    return false;
                }
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Message successCategory = JsonConvert.DeserializeObject<Message>(responseString);
                MessageBox.Show(successCategory.message, "Partner sikeresen létrehozva.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }

        // ---------------------------------------Alkategóriák-----------------------------------------------


        //Get Alkategória
        public async Task<List<Subcategory>> GetSubcategories()
        {
            SetAuthHeader();
            List<Subcategory> subCategoryList = new List<Subcategory>();
            string url = baseURL + "/subcategories";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                subCategoryList = JsonConvert.DeserializeObject<List<Subcategory>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return subCategoryList;
        }

        public async Task<bool> PostSubcategory(Subcategory oneSubcategory)
        {
            SetAuthHeader();
            string url = baseURL + "/subcategories";
            try
            {
                var jsonData = new
                {
                    newName = oneSubcategory.name,
                    newCategoryId = oneSubcategory.categoryId
                };
                string jsonString = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Message errorSubcategory = JsonConvert.DeserializeObject<Message>(errorMessage);
                    MessageBox.Show(errorSubcategory.message, "Hiba a kategória létrehozásakor.");
                    return false;
                }
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Message successSubcategory = JsonConvert.DeserializeObject<Message>(responseString);
                MessageBox.Show(successSubcategory.message, "Alkategória sikeresen létrehozva.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }

        // ---------------------------------------Rendelések-----------------------------------------------

        //Get Rendelések
        public async Task<List<Orders>> GetOrders()
        {
            SetAuthHeader();
            List<Orders> orderList = new List<Orders>();
            string url = baseURL + "/orders";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                orderList = JsonConvert.DeserializeObject<List<Orders>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return orderList;
        }
        // ---------------------------------------Rendelés részletei-----------------------------------------------

        //Get Rendelések
        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            SetAuthHeader();
            List<OrderItem> itemsList = new List<OrderItem>();
            string url = baseURL + $"/orders/{orderId}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseString);
                var items = json["items"].ToString();
                itemsList = JsonConvert.DeserializeObject<List<OrderItem>>(items);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return itemsList;
        }
        // ---------------------------------------Rendelés státusz-----------------------------------------------

        public async Task OrderAsCompletedAsync(int orderId)
        {
            SetAuthHeader();
            string url = baseURL + $"/orders/{orderId}/status";
            var requestBody = new
            {
                newStatus = "completed"
            };
            string json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                string errorText = await response.Content.ReadAsStringAsync();
                throw new Exception($"Hiba a rendelés frissítésekor: {errorText}");
            }
        }
        // ---------------------------------------Számlák-----------------------------------------------

        // Invoices Get 
        public async Task<List<Invoices>> GetInvoices()
        {
            SetAuthHeader();
            List<Invoices> invoicesList = new List<Invoices>();
            string url = baseURL + $"/invoices";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                invoicesList = JsonConvert.DeserializeObject<List<Invoices>>(responseInString);

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return invoicesList;
        }
        // ---------------------------------------Naplózás---------------------------------------------

        // logs Get
        public async Task<List<Logs>> GetLogs()
        {
            SetAuthHeader();
            List<Logs> logsList = new List<Logs>();
            string url = baseURL + $"/logs";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                logsList = JsonConvert.DeserializeObject<List<Logs>>(responseInString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return logsList;
        }

    }
}
