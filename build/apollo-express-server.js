"use strict";
var __importDefault =
  (this && this.__importDefault) ||
  function (mod) {
    return mod && mod.__esModule ? mod : { default: mod };
  };
Object.defineProperty(exports, "__esModule", { value: true });
// local apollo graphQL server
const express_1 = __importDefault(require("express"));
const apollo_server_express_1 = require("apollo-server-express");
const server_1 = require("./server");
const port = 3000; // Choose your desired port
const run = async () => {
  const server = new apollo_server_express_1.ApolloServer(
    server_1.serverConfig,
  );
  await server.start();
  const app = (0, express_1.default)();
  server.applyMiddleware({ app });
  app.listen(port, () => {
    console.log(`GraphQL server listening on port ${port}`);
  });
};
run();
