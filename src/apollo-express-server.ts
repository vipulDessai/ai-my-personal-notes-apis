// local apollo graphQL server
import express from "express";
import { ApolloServer } from "apollo-server-express";

import { serverConfig } from "./server";

const port = 8081; // Choose your desired port

const run = async () => {
  const server = new ApolloServer(serverConfig);

  await server.start();

  const app = express();
  server.applyMiddleware({ app });

  app.listen(port, () => {
    console.log(`GraphQL server listening on port ${port}`);
  });
};

run();
