const { ApolloServer } = require('apollo-server-lambda');
const schema = require('./schema');

const server = new ApolloServer({
  schema,
  playground: true, 
  introspection: true,
});

exports.handler = server.createHandler();