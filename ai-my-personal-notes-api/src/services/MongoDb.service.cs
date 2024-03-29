﻿using MongoDB.Driver;

namespace ai_my_personal_notes_api.services
{
    public class MongoDbServer
    {
        public MongoClient client;

        public MongoDbServer()
        {
            var mongoClientSettings = MongoClientSettings.FromConnectionString(
                $"{Environment.GetEnvironmentVariable("MONGODB_URI")}"
            );
            mongoClientSettings.ServerApi = new ServerApi(ServerApiVersion.V1, strict: true);
            client = new MongoClient(mongoClientSettings);
        }
    }
}
