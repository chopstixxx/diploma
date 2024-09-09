using Moq;
using MySqlConnector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using testmobile;
using testmobile.Classes;
using Xamarin.Essentials; 

namespace Unit_tests
{
    public class Tests
    {
        [TestFixture]
        public class DBTests
        {
            private const string connectionString = "Server = db4free.net; Port = 3306; Database = mobile_test; Uid = mobile; Pwd = mobile12; Allow User Variables=True;";

            private DB db;

            [SetUp]
            public void Setup()
            {
                db = new DB();
            }

            [TearDown]
            public void TearDown()
            {
                db.Dispose();
            }

            [Test]
            public void OpenConn_WhenCalled_ConnectionOpened()
            {
                // Act
                db.openConn();

                // Assert
                Assert.AreEqual(ConnectionState.Open, db.getConn().State);
            }

            [Test]
            public void CloseConn_WhenCalled_ConnectionClosed()
            {
                // Arrange
                db.openConn();

                // Act
                db.closeConn();

                // Assert
                Assert.AreEqual(ConnectionState.Closed, db.getConn().State);
            }

            [Test]
            public void GetUserId_ValidLogin_ReturnsId()
            {
                // Arrange
                const string validLogin = "Cok123";
                const int expectedId = 21;

                // Act
                var id = db.get_user_id(validLogin);

                // Assert
                Assert.AreEqual(expectedId, id);
            }

            [Test]
            public void GetUserId_InvalidLogin_ReturnsZero()
            {
                // Arrange
                const string invalidLogin = "nonexistentuser";
                const int expectedId = 0;

                // Act
                var id = db.get_user_id(invalidLogin);

                // Assert
                Assert.AreEqual(expectedId, id);
            }
        }
        [TestFixture]
        public class HashingTests
        {
            private hashing hashingInstance;

            [SetUp]
            public void Setup()
            {
                hashingInstance = new hashing();
            }

            [TearDown]
            public void TearDown()
            {
                hashingInstance = null;
            }

            [Test]
            public void HashPassword_ValidInput_ReturnsHashedValue()
            {
                // Arrange
                string input = "password123";
                string expectedHash = CalculateMD5Hash(input);

                // Act
                string actualHash = hashingInstance.Hash_password(input);

                // Assert
                Assert.AreEqual(expectedHash, actualHash);
            }

           

            [Test]
            public void HashPassword_NullInput_ThrowsException()
            {
                // Arrange
                string input = null;

                // Act & Assert
                Assert.Throws<System.ArgumentNullException>(() => hashingInstance.Hash_password(input));
            }

            // Helper method to calculate MD5 hash for comparison
            private string CalculateMD5Hash(string input)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(input);
                    byte[] hash = md5.ComputeHash(bytes);

                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash)
                    {
                        sb.Append(b.ToString("X2"));
                    }
                    return sb.ToString();
                }
            }

            [TestFixture]
            public class UserTests
            {
                [Test]
                public async Task LoadUserDataFromDatabase_Client_Success()
                {
                    // Arrange
                    var user = new User();
                    user.Login = "Cok123"; // Устанавливаем логин

                    // Act
                    await user.LoadUserDataFromDatabase(IsClient: true);

                    // Assert
                    Assert.NotNull(user.Name);
                    Assert.NotNull(user.Date_of_birth);
                    Assert.NotNull(user.Phone_number);
                    // Add more assertions if necessary
                }

                [Test]
                public async Task LoadUserDataFromDatabase_Guide_Success()
                {
                    // Arrange
                    var user = new User();
                    user.Login = "Testgid17"; // Устанавливаем логин

                    // Act
                    await user.LoadUserDataFromDatabase(IsClient: false);

                    // Assert
                    Assert.NotNull(user.Name);
                    Assert.NotNull(user.Date_of_birth);
                    Assert.NotNull(user.Phone_number);
                    // Add more assertions if necessary
                }
            }
            [TestFixture]
            public class CatalogViewModelTests
            {
                [Test]
                public async Task LoadDataAsync_ProductsNotEmpty_Success()
                {
                    
                    var viewModel = new CatalogViewModel();

                    
                    await viewModel.LoadDataAsync();

                    
                    Assert.IsNotNull(viewModel.Products);
                    Assert.IsNotEmpty(viewModel.Products);
                }

                [Test]
                public async Task GetPlacesByGuideId_WithGuideId_ReturnsMatchingProducts()
                {
                    
                    var viewModel = new CatalogViewModel();
                    await viewModel.LoadDataAsync(); 
                    int guideId = 123;

                    
                    var places = viewModel.GetPlacesByGuideId(guideId);

                    
                    Assert.IsNotNull(places);
                    Assert.IsInstanceOf(typeof(ObservableCollection<Product>), places);
                    foreach (var place in places)
                    {
                        Assert.AreEqual(guideId, place.Guide_Id);
                    }
                }
                [TestFixture]
                public class OrdersViewModelTests
                {
                    [Test]
                    public async Task LoadOrdersData_OrdersNotEmpty_Success()
                    {
                       
                        var viewModel = new OrdersViewModel();

                      
                        await viewModel.LoadOrdersData();

                       
                        Assert.IsNotNull(viewModel.Orders);
                        Assert.IsNotEmpty(viewModel.Orders);
                    }

                    [Test]
                    public async Task GetOrdersByClientId_WithClientId_ReturnsMatchingOrders()
                    {
                        
                        var viewModel = new OrdersViewModel();
                        await viewModel.LoadOrdersData(); 
                        int clientId = 9;

                        
                        var clientOrders = viewModel.GetOrdersByClientId(clientId);

                       
                        Assert.IsNotNull(clientOrders);
                        Assert.IsInstanceOf(typeof(ObservableCollection<Order>), clientOrders);
                        foreach (var order in clientOrders)
                        {
                            Assert.AreEqual(clientId, order.Client_id);
                        }
                    }

                    [Test]
                    public async Task GetOrdersByGuideId_WithGuideId_ReturnsMatchingOrders()
                    {
                      
                        var viewModel = new OrdersViewModel();
                        await viewModel.LoadOrdersData(); 
                        int guideId = 3;

                       
                        var guideOrders = viewModel.GetOrdersByGuideId(guideId);

                       
                        Assert.IsNotNull(guideOrders);
                        Assert.IsInstanceOf(typeof(ObservableCollection<Order>), guideOrders);
                        foreach (var order in guideOrders)
                        {
                            Assert.AreEqual(guideId, order.Guide_id);
                        }
                    }
                }
            }
        }
    }
}