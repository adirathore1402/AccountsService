using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using PartitionKey = Microsoft.Azure.Cosmos.PartitionKey;

namespace AccountsService.Repository
{
    public class Repo
    {
        private readonly CosmosClient client;
        private readonly Container container;

        public Repo()
        {
   // endpoint should be extracted
            client = new CosmosClient(
                accountEndpoint: "https://yasagrawalcosmosacc.documents.azure.com:443/",
                tokenCredential: new DefaultAzureCredential());
    // ids should be extracted
            var database = client.GetDatabase(id: "aurora-platform-db");
            container = database.GetContainer(id: "Accounts-<ReplaceWithYourName>");
        }

        public async Task<Account> Find(string name, string password)
        {
            var queryable = container.GetItemLinqQueryable<Account>();

            var queryableMatches = queryable
                .Where(a => a.Name == name)
                .Where(a => a.Password == password);

            using FeedIterator<Account> feedIterator = queryableMatches.ToFeedIterator();

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Account> response = await feedIterator.ReadNextAsync();

                foreach (Account item in response)
                {
                    return item;
                }
            }
            // 
            return null;
        }

        public async Task<Account> Get(string id)
        {
            return await container.ReadItemAsync<Account>(id, new PartitionKey(id.ToString()));
        }

        public async Task<Account> Update(Account account)
        {
            return await container.UpsertItemAsync(account, new PartitionKey(account.id.ToString()));
        }

        public async Task<Account> Create(string name, string password)
        {
            var account = new Account()
            {
                Name = name,
                Password = password,
                id = Guid.NewGuid(),
                Balance = 0.0,
                OpeningDate = DateTime.Now,
                Transactions = new List<Transaction>()
            };

            return await container.CreateItemAsync(
                        item: account,
                        partitionKey: new PartitionKey(account.id.ToString()));
        }
    }
}
