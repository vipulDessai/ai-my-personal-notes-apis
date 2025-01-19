"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// NPM local lambda library debug single lambda function
// use `npm run invoke:local`
// ts-node src/local-lambda.ts
const apollo_server_lambda_1 = require("apollo-server-lambda");
const server_1 = require("./server");
const getHandler = (event, context, callback) => {
  const server = new apollo_server_lambda_1.ApolloServer(server_1.serverConfig);
  const graphqlHandler = server.createHandler();
  // This is a workaround to make the handler work with the
  // lambda-local library. The library event object does not
  // have a requestContext property, which is required by the
  // ApolloServer handler.
  if (!event.requestContext) {
    event.requestContext = context;
  }
  return graphqlHandler(event, context, callback);
};
exports.handler = getHandler;
