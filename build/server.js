"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.serverConfig = void 0;
const schema_1 = require("./schema");
const apollo_server_core_1 = require("apollo-server-core");
exports.serverConfig = {
  schema: schema_1.schema,
  introspection: true,
  debug: true,
  plugins: [
    (0, apollo_server_core_1.ApolloServerPluginLandingPageGraphQLPlayground)(),
  ],
};
