using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Services;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace TestCore
{
    public class TestHelpers
    {
        [SetUp]
        public void Setup() { }

        // Happy flow
        [Test]
        public void VerifyPassword_WithValidHash_ReturnsTrue()
        {
            const string password = "user3";
            const string passwordHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08=")]
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        public void VerifyPassword_WithValidHashes_ReturnsTrue(string password, string passwordHash)
        {
            Assert.IsTrue(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        // Unhappy flow
        [Test]
        public void VerifyPassword_WithWrongPassword_ReturnsFalse()
        {
            const string wrongPassword = "userX";
            const string validHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsFalse(PasswordHelper.VerifyPassword(wrongPassword, validHash));
        }

        // Invalid hashes (tampered or truncated)
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbn")] // tampered
        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08")] // truncated missing '='
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA")] // truncated missing '='
        public void VerifyPassword_InvalidHashes_ReturnsFalse(string password, string passwordHash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [Test]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            const string emptyPassword = "";
            const string validHash = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=";
            Assert.IsFalse(PasswordHelper.VerifyPassword(emptyPassword, validHash));
        }

        // UC13: Klanten tonen per product (kort)
        [Test]
        public void BoughtProductsService_Product_ShowsDistinctClientsAndLists()
        {
            var product = new Product(10, "Brood", 5);
            var client1 = new Client(1, "Anna", "a@a", "x");
            var clientAdmin = new Client(3, "Admin", "admin@a", "x", Role.Admin);
            var gl1 = new GroceryList(100, "Lijst Anna", new DateOnly(2025,1,10), "#1", client1.Id);
            var gl2 = new GroceryList(101, "Lijst Admin", new DateOnly(2025,1,12), "#2", clientAdmin.Id);

            var items = new []
            {
                new GroceryListItem(1, gl1.Id, product.Id, 2),
                new GroceryListItem(2, gl2.Id, product.Id, 1),
                new GroceryListItem(3, gl2.Id, product.Id, 3) // zelfde lijst nog eens -> geen extra entry
            };

            var gliRepo = new Mock<IGroceryListItemsRepository>();
            gliRepo.Setup(r => r.GetAll()).Returns(items.ToList());
            var glRepo = new Mock<IGroceryListRepository>();
            glRepo.Setup(r => r.Get(gl1.Id)).Returns(gl1);
            glRepo.Setup(r => r.Get(gl2.Id)).Returns(gl2);
            var clientRepo = new Mock<IClientRepository>();
            clientRepo.Setup(r => r.Get(client1.Id)).Returns(client1);
            clientRepo.Setup(r => r.Get(clientAdmin.Id)).Returns(clientAdmin);
            var prodRepo = new Mock<IProductRepository>();
            prodRepo.Setup(r => r.Get(product.Id)).Returns(product);

            var service = new BoughtProductsService(gliRepo.Object, glRepo.Object, clientRepo.Object, prodRepo.Object);

            var result = service.Get(product.Id);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Exists(r => r.Client.Id == client1.Id));
            Assert.That(result.Exists(r => r.Client.Id == clientAdmin.Id && r.Client.Role == Role.Admin));
        }
    }
}