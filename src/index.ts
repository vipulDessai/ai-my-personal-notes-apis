// this file will be used by SAM CLI to deploy to AWS Lambda
import { server } from "./server";

const graphqlHandler = server.createHandler();

exports.handler = graphqlHandler;
