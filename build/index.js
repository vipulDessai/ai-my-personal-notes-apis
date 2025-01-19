"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// this file will be used by SAM CLI to deploy to AWS Lambda
const apollo_server_lambda_1 = require("apollo-server-lambda");
const server_1 = require("./server");
const server = new apollo_server_lambda_1.ApolloServer(server_1.serverConfig);
const graphqlHandler = server.createHandler();
exports.handler = graphqlHandler;
