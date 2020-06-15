using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common
{
    public static class ExtensionMethods
    {
        /// Partition a collection into parts by number of items in each part.
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            var partition = new List<T>(size);
            var counter = 0;

            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    partition.Add(enumerator.Current);
                    counter++;
                    if (counter % size == 0)
                    {
                        yield return partition.ToList();
                        partition.Clear();
                        counter = 0;
                    }
                }

                if (counter != 0)
                    yield return partition;
            }
        }

        /// <summary>
        /// Converts a Cosmos DB Document to a class. This is useful when retrieving Cosmos DB
        /// documents from a CosmosDBTrigger in an Azure Function since it only supports returning
        /// an IReadOnlyList of Documents.
        /// </summary>
        /// <typeparam name="T">The Type (POCO) to which the Document should be converted.</typeparam>
        /// <param name="document">The Cosmos DB Document to convert.</param>
        /// <returns></returns>
        public static async Task<T> ReadAsAsync<T>(this Document document)
        {
            using (var ms = new MemoryStream())
            using (var reader = new StreamReader(ms))
            {
                document.SaveTo(ms);
                ms.Position = 0;
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
            }
        }
    }
}
