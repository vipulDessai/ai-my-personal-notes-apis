const { ApolloServer } = require("apollo-server-lambda");
const schema = require("./schema");

const getHandler = (event, context) => {
  const server = new ApolloServer({
    schema,
    playground: true,
    introspection: true,
    debug: true,
  });
  const graphqlHandler = server.createHandler();
  if (!event.requestContext) {
    event.requestContext = context;
  }
  return graphqlHandler(event, context);
};

exports.handler = getHandler;
