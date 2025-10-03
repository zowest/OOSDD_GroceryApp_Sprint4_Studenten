using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;

namespace Grocery.Core.Data.Repositories
{
    // Eenvoudige in-memory repository 
    public class ClientRepository : IClientRepository
    {
        private readonly List<Client> clientList;

        public ClientRepository()
        {
            clientList =
            [
                new Client(1, "M.J. Curie", "user1@mail.com", "IunRhDKa+fWo8+4/Qfj7Pg==.kDxZnUQHCZun6gLIE6d9oeULLRIuRmxmH2QKJv2IM08="),
                new Client(2, "H.H. Hermans", "user2@mail.com", "dOk+X+wt+MA9uIniRGKDFg==.QLvy72hdG8nWj1FyL75KoKeu4DUgu5B/HAHqTD2UFLU="),
                new Client(3, "A.J. Kwak", "user3@mail.com", "sxnIcZdYt8wC8MYWcQVQjQ==.FKd5Z/jwxPv3a63lX+uvQ0+P7EuNYZybvkmdhbnkIHA=", Role.Admin)
            ];
        }

        // Zoek client op e-mail
        public Client? Get(string email) =>
            clientList.FirstOrDefault(c => c.EmailAddress.Equals(email));

        // Zoek client op Id
        public Client? Get(int id) =>
            clientList.FirstOrDefault(c => c.Id == id);

        // Geef volledige lijst terug
        public List<Client> GetAll() => clientList;
    }
}
