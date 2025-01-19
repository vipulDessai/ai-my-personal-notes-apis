import tsEslintPlugin from "@typescript-eslint/eslint-plugin";

export default [
  {
    files: ["**/*.ts"],
    rules: {
      "@typescript-eslint/no-unused-vars": ["warn"],
      "@typescript-eslint/no-require-imports": "off",
      "@typescript-eslint/no-explicit-any": "off",
      "@typescript-eslint/ban-ts-comment": "off",
    },
    languageOptions: {
      parser: "@typescript-eslint/parser", // Use TypeScript parser
      parserOptions: {
        project: ["./tsconfig.json"], // Point to your tsconfig.json
      },
    },
    plugins: {
      "@typescript-eslint": tsEslintPlugin, // Define the plugin
    },
  },
];
