using Grocery.Core.Helpers;

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

        [Test]
        public void VerifyPassword_WithTamperedHash_ReturnsFalse()
        {
            const string password = "user3";
            // Hash gemanipuleerd (laatste chars weg)
            const string tampered = "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbn";
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, tampered));
        }

        [TestCase("user1", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08")] // ontbreekt '=' op einde
        [TestCase("user3", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA")] // ontbreekt '='
        public void VerifyPassword_WithTruncatedHash_ReturnsFalse(string password, string passwordHash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }

        [TestCase("", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=")]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse(string password, string passwordHash)
        {
            Assert.IsFalse(PasswordHelper.VerifyPassword(password, passwordHash));
        }
    }
}