// local apollo graphQL server
import express from "express";

import { server } from "./server";

const app = express();
server.applyMiddleware({ app });

const port = 3000; // Choose your desired port

app.listen(port, () => {
  console.log(`GraphQL server listening on port ${port}`);
});
