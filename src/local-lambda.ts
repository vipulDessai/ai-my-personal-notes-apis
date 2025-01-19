// NPM local lambda library debug single lambda function
// use `npm run invoke:local`
// ts-node src/local-lambda.ts
import { ApolloServer } from "apollo-server-lambda";

import { serverConfig } from "./server";

const getHandler = (event: any, context: any, callback: any) => {
  const server = new ApolloServer(serverConfig);

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
