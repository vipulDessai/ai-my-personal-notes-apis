const { gql } = require("apollo-server-lambda");
const { makeExecutableSchema } = require("@graphql-tools/schema");

const typeDefs = gql`
  type Query {
    foo: String
  }
`;

const resolvers = {
  Query: {
    foo: async () => {
      const res = await new Promise((resolve) => {
        setTimeout(() => {
          resolve("Hello, foo world!");
        }, 1000);
      });

      return res;
    },
  },
};

const schema = makeExecutableSchema({ typeDefs, resolvers });

module.exports = schema;
