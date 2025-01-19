"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.schema = void 0;
const apollo_server_lambda_1 = require("apollo-server-lambda");
const schema_1 = require("@graphql-tools/schema");
const typeDefs = (0, apollo_server_lambda_1.gql)`
  type Query {
    foo: String
  }
`;
const resolvers = {
  Query: {
    foo: async () => {
      const res = await new Promise((resolve) => {
        setTimeout(() => {
          resolve("Hello, foo 2 world!");
        }, 1000);
      });
      return res;
    },
  },
};
exports.schema = (0, schema_1.makeExecutableSchema)({ typeDefs, resolvers });
