module.exports = {
  env: {
    es2021: true,
  },
  settings: {},
  extends: ["eslint:recommended", "plugin:@typescript-eslint/recommended"],
  parserOptions: {
    ecmaFeatures: {
      jsx: true,
    },
    ecmaVersion: 12,
    sourceType: "module",
  },
  plugins: ["@typescript-eslint"],
  rules: {
    // here the rules added will impact both the js and ts files
    // ADD WITH CAUTION
  },
  overrides: [
    // Apply only to typescript files
    {
      files: ["*.ts"],
      rules: {
        "@typescript-eslint/no-unused-vars": ["warn"],
        "@typescript-eslint/no-require-imports": "off",
        "@typescript-eslint/no-explicit-any": "off",
        "@typescript-eslint/ban-ts-comment": "off",
      },
    },
    // Rule for plain javascript files
    {
      files: ["*.js"],
      rules: {
        "no-prototype-builtins": "warn",
        "no-unreachable": "warn",
        "no-empty": "warn",
        "no-empty-pattern": "off",
        "no-unused-vars": "off",
        "@typescript-eslint/no-unused-vars": "off",
        "no-case-declarations": "off",
        "@typescript-eslint/no-unused-expressions": "off",
        "@typescript-eslint/no-require-imports": "off",
        "@typescript-eslint/no-array-constructor": "off",
      },
    },
  ],
  globals: {
    // Declare global variables here
    process: "writable",
    module: "writable",
    require: "writable",
  },
};
