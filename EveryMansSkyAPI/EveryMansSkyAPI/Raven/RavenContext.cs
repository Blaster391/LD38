using System.Collections.Generic;
using EveryMansSkyAPI.Raven.Indexes;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace EveryMansSkyAPI.Raven
{
    public static class RavenContext
    {
        public const int PageSize = 20;
        private const int BulkInsertSize = 1024;

        private static readonly BulkInsertOptions _bulkInsertOptions;

        static RavenContext()
        {
            Store = new DocumentStore
            {
                Url = "http://localhost:8080/",
                DefaultDatabase = "EveryMansSky"
            };

            _bulkInsertOptions = new BulkInsertOptions()
            {
                BatchSize = BulkInsertSize,
                OverwriteExisting = true
            };

            Store.Initialize();
            DatabaseName = "EveryMansSky";

            IndexCreation.CreateIndexes(typeof(PlayerUsername).Assembly(), Store);
        }


        public static string DatabaseName { get; }

        public static IDocumentStore Store { get; }

        public static void Save(IEnumerable<object> values)
        {
            using (var bulkInsert = Store.BulkInsert(options: _bulkInsertOptions))
            {
                // ReSharper disable once AccessToDisposedClosure
                values.ForEach(value => bulkInsert.Store(value));
            }
        }

        public static void Save(object value)
        {
            using (var session = Store.OpenSession())
            {
                session.Store(value);
                session.SaveChanges();
            }
        }

        public static T LoadById<T>(string id)
        {
            using (var session = Store.OpenSession())
            {
                return session.Load<T>(id);
            }
        }
    }
}