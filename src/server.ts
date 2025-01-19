import { ApolloServer } from "apollo-server-lambda";
import { schema } from "./schema";

import { ApolloServerPluginLandingPageGraphQLPlayground } from "apollo-server-core";

export const server = new ApolloServer({
  schema,
  introspection: true,
  debug: true,
  plugins: [ApolloServerPluginLandingPageGraphQLPlayground()],
});
