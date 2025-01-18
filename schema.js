const { gql } = require('apollo-server-lambda');
const { makeExecutableSchema } = require('@graphql-tools/schema');

const typeDefs = gql`
  type Query {
    foo: String
  }
`;

const resolvers = {
  Query: {
    foo: () => {
      return 'Hello, foo world!';
    },
  },
};

const schema = makeExecutableSchema({ typeDefs, resolvers });

module.exports = schema;