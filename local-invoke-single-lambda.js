const lambdaLocal = require("lambda-local");
const path = require("path");

const event = require("./context/foo-query.json");
const clientContext = require("./context/client-context.json");

lambdaLocal.execute({
  event,
  lambdaPath: path.join(__dirname, "build", "local-lambda.js"),
  profilePath: "~/.aws/credentials",
  profileName: "default",
  timeoutMs: 120000,
  callback: function (err, data) {
    if (err) {
      console.log(err);
    } else {
      console.log(data);
    }
  },
  clientContext,
});
