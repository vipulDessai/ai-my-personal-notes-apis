// this file will be used by SAM CLI to deploy to AWS Lambda
import { ApolloServer } from "apollo-server-lambda";

import { serverConfig } from "./server";

const server = new ApolloServer(serverConfig);

const graphqlHandler = server.createHandler();

exports.handler = graphqlHandler;
