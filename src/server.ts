import { schema } from "./schema";

import { ApolloServerPluginLandingPageGraphQLPlayground } from "apollo-server-core";

export const serverConfig = {
  schema,
  introspection: true,
  debug: true,
  plugins: [ApolloServerPluginLandingPageGraphQLPlayground()],
};
