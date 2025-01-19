import { gql } from "apollo-server-lambda";
import { makeExecutableSchema } from "@graphql-tools/schema";

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
          resolve("Hello, foo 2 world!");
        }, 1000);
      });

      return res;
    },
  },
};

export const schema = makeExecutableSchema({ typeDefs, resolvers });
