import path from "path";
import dotenv from "dotenv";
import webpack from "webpack";
import { fileURLToPath } from "url";
import Module from "node:module";
import { BundleAnalyzerPlugin } from "webpack-bundle-analyzer";
import TerserPlugin from "terser-webpack-plugin";
import ESLintPlugin from "eslint-webpack-plugin";

console.info(
  "Current Specified Enviornment: " + (process.env.NODE_ENV || null),
);

let webpackMode = "";
let needSourceMaps = false;
let needBundleAnalyser = false;
let skipTypeCheck = false;
// while the HMR is active certain precached modules
// throw error while recompiling which takes away
// the advantage of HMR, so while development
// include them by setting excludePrecachedLibs
let excludePrecachedLibs = false;

const NODE_ENV = process.env.NODE_ENV || "dev";
switch (NODE_ENV) {
  case "production":
    // TODO: later once the build is stable, change "needSourceMaps" to false
    needSourceMaps = true;
    skipTypeCheck = true;
    excludePrecachedLibs = true;
    webpackMode = "production";
    break;

  case "development":
    needSourceMaps = true;
    needBundleAnalyser = true; // TODO: set needBundleAnalyser
    webpackMode = "development";
    break;

  default:
    needSourceMaps = true;
    skipTypeCheck = true;
    excludePrecachedLibs = true;
    webpackMode = "development";
    break;
}

console.info("Current Actual Enviornment: " + NODE_ENV);

// inline-source-map - Full source map inlined in the bundle (the code is as it is, without the .map files)
// hidden-source-map
//   - Use hidden-source-map or nosources-source-map to avoid exposing sensitive source code.
//   - Serve .map files selectively or restrict access to them.
console.info(
  "Generate Source Maps: " +
    (needSourceMaps ? "inline-source-map" : "hidden-source-map"),
);

console.info("Skip Type Checking: " + skipTypeCheck);

console.info("Bundle Analyser: " + needBundleAnalyser);
console.info("Cache Prebuild large lib binaries: " + excludePrecachedLibs);

const __filename = fileURLToPath(import.meta.url); // get the resolved path to the file
const __dirname = path.dirname(__filename); // get the name of the directory
const require = Module.createRequire(import.meta.url);

const envFilePath = path.resolve(__dirname, `.env.${NODE_ENV}`);
dotenv.config({ path: envFilePath, safe: true });

export default {
  mode: webpackMode,

  target: "node",

  entry: {
    app: "./src/index.ts", // Entry point
  },

  output: {
    path: path.resolve(__dirname, "build"),
    filename: "[name].[contenthash].bundle.js",
    chunkFilename: "[name].[contenthash].chunk.js", // Output for dynamically loaded chunks
    publicPath: "/", // Ensure the chunks are resolved from the correct path
    clean: true, // Ensures the output folder is cleared before building, so old files don’t linger.
  },

  optimization: Object.assign(
    {},
    webpackMode === "production" && {
      splitChunks: {
        chunks: "all", // enable auto code chunks for all code splits
        maxSize: 200000, // Split files larger than 200 KB
        cacheGroups: {
          vendor: {
            test: /[\\/]node_modules[\\/]/,
            name: "vendors",
            chunks: "all",
          },
        },
      },
      minimizer: [
        new TerserPlugin({
          terserOptions: {
            compress: {
              booleans: true, // Optimize boolean expressions (e.g., `!!a` becomes `a`)
              dead_code: true, // Remove unreachable code
              unused: true, // Drop unused variables and functions
              drop_console: true, // Remove all console.* calls
              drop_debugger: true, // Remove debugger statements
              conditionals: true, // Optimize if-else and ternary expressions
              sequences: true, // Combine consecutive statements using the comma operator
              ecma: 2015, // Optimize for ES6
              passes: 3, // Apply compression passes multiple times for better results
            },
            mangle: true, // Shorten variable names
            output: {
              comments: false, // Remove comments
              ascii_only: true, // Prevents encoding issues
            },
            sourceMap: needSourceMaps,
          },
          parallel: true, // Enable multi-threading for faster builds
        }),
      ],
    },
  ),

  // global resolver, without include or exclude path
  // for finer alias management use the module -> rules -> alias
  resolve: {
    extensions: [".js", ".ts"],
    alias: {},
  },

  module: {
    rules: [
      {
        oneOf: [
          // Process the application source code for typescript
          {
            test: /\.ts$/,
            exclude: /node_modules/,
            use: [
              {
                loader: require.resolve("ts-loader"),
                options: {
                  getCustomTransformers: () => ({}),
                  transpileOnly: skipTypeCheck,
                  compilerOptions: {
                    inlineSourceMap: needSourceMaps,
                  },
                },
              },
            ],
            resolve: {
              alias: {},
            },
          },

          // "file" loader makes sure those assets get served by WebpackDevServer.
          // When you `import` an asset, you get its (virtual) filename.
          // In production, they would get copied to the `build` folder.
          // This loader doesn't use a "test" so it will catch all modules
          // that fall through the other loaders.
          {
            loader: require.resolve("file-loader"),
            // Exclude `js` files to keep "css" loader working as it injects
            // its runtime that would otherwise be processed through "file" loader.
            // Also exclude `html` and `json` extensions so they get processed
            // by webpacks internal loaders.
            exclude: [/\.(js|mjs|jsx|ts|tsx)$/, /\.html$/, /\.json$/],
            options: {
              name: "static/media/[name].[hash:8].[ext]",
            },
          },
          // ** STOP ** Are you adding a new loader?
          // Make sure to add the new loader(s) before the "file" loader.
        ],
      },
    ],
  },

  plugins: [
    new webpack.ProgressPlugin(),
    needBundleAnalyser &&
      new BundleAnalyzerPlugin({
        analyzerMode: "static",
      }),
    new ESLintPlugin({
      extensions: ["js", "jsx", "ts", "tsx"], // Check JavaScript and TypeScript files
      failOnError: false, // Set to true to fail the build on linting errors
      emitWarning: true, // Emit warnings instead of errors
      overrideConfigFile: path.resolve(__dirname, ".eslintrc.js"), // Custom ESLint config
    }),
  ].filter(Boolean),

  devServer: {
    static: {
      directory: path.join(__dirname, "public"), // Serve files from the "dist" directory
    },
    port: process.env.PORT || 3000,
    open: true,
    hot: true,
    client: {
      progress: true,
      overlay: false,
    },
    historyApiFallback: true, // Redirect all 404s to index.html
  },

  // inline-source-map - Full source map inlined in the bundle (the code is as it is, without the .map files)
  // hidden-source-map
  //   - Use hidden-source-map or nosources-source-map to avoid exposing sensitive source code.
  //   - Serve .map files selectively or restrict access to them.
  devtool: needSourceMaps ? "inline-source-map" : "hidden-source-map",
};
