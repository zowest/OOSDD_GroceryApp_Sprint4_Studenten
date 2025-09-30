using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class BoughtProductsService : IBoughtProductsService
    {
        private readonly IGroceryListItemsRepository _groceryListItemsRepository;
        private readonly IGroceryListRepository _groceryListRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IProductRepository _productRepository;

        public BoughtProductsService(
            IGroceryListItemsRepository groceryListItemsRepository,
            IGroceryListRepository groceryListRepository,
            IClientRepository clientRepository,
            IProductRepository productRepository)
        {
            _groceryListItemsRepository = groceryListItemsRepository;
            _groceryListRepository = groceryListRepository;
            _clientRepository = clientRepository;
            _productRepository = productRepository;
        }

        public List<BoughtProducts> Get(int? productId)
        {
            if (productId is null)
                return new List<BoughtProducts>();

            // Haal product op (als het niet bestaat: lege lijst)
            Product? product = _productRepository.Get(productId.Value);
            if (product is null)
                return new List<BoughtProducts>();

            // Alle items die dit product bevatten en effectief gekocht (Amount > 0)
            var listItems = _groceryListItemsRepository
                .GetAll()
                .Where(li => li.ProductId == productId && li.Amount > 0)
                .ToList();

            if (listItems.Count == 0)
                return new List<BoughtProducts>();

            // Unieke boodschappenlijsten ophalen
            var groceryListIds = listItems
                .Select(li => li.GroceryListId)
                .Distinct()
                .ToList();

            List<BoughtProducts> result = new();

            foreach (int glId in groceryListIds)
            {
                var groceryList = _groceryListRepository.Get(glId);
                if (groceryList is null) continue;

                var client = _clientRepository.Get(groceryList.ClientId);
                if (client is null) continue;

                result.Add(new BoughtProducts(client, groceryList, product));
            }

            // Optioneel sorteren (bijv. op Client naam, dan datum)
            result = result
                .OrderBy(r => r.Client.Name)
                .ThenBy(r => r.GroceryList.Date)
                .ToList();

            return result;
        }
    }
}
