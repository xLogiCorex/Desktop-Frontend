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

namespace Desktop
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
        // ---------------------------------------LOGIN-----------------------------------------------

        public async Task<User> Login(string email, string password)
        {
            string url = baseURL + "/login";
            User oneUser = new User() { email = null };
            try
            {
                var jsonData = new
                {
                    newEmail = email,
                    newPassword = password,
                };

                string StringJSON = JsonConvert.SerializeObject(jsonData);
                StringContent sendThis = new StringContent(StringJSON, Encoding.UTF8, "Application/JSON");

                HttpResponseMessage valasz = await client.PostAsync(url, sendThis);
                valasz.EnsureSuccessStatusCode();
                string valaszText = await valasz.Content.ReadAsStringAsync();

                //token
                dynamic response = JsonConvert.DeserializeObject(valaszText);
                jwtToken = response.token;

                oneUser.email = email;
                oneUser.password = password;
                oneUser.role = response.role;

                return oneUser;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return oneUser;
            }
        }

        // Token hitelesítés
        private void SetAuthHeader()
        {
            client.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(jwtToken))
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
            SetAuthHeader();
            List<Product> productList = new List<Product>();
            string url = baseURL + "/products";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseInString = await response.Content.ReadAsStringAsync();
                productList = JsonConvert.DeserializeObject<List<Product>>(responseInString);
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
            return productList;
        }

        //Post Products
        public async Task<bool> PostProduct(Product oneProduct)
        {
            SetAuthHeader();
            string url = baseURL + "/products";
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
                StringContent sendThis = new StringContent(jsonString, Encoding.UTF8, "Application/JSON");
                HttpResponseMessage response = await client.PostAsync(url, sendThis);
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Message errorProduct = JsonConvert.DeserializeObject<Message>(errorMessage);
                    MessageBox.Show(errorProduct.message, "Hiba a termék létrehozásakor.");
                    return false;
                }
                response.EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                Message successProduct = JsonConvert.DeserializeObject<Message>(responseString);
                MessageBox.Show(successProduct.message, "Termék sikeresen létrehozva.");
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

    }
}