module.exports = {
  root: true,
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    '@typescript-eslint/recommended',
    'plugin:react-hooks/recommended',
  ],
  ignorePatterns: ['dist', '.eslintrc.cjs'],
  parser: '@typescript-eslint/parser',
  plugins: ['react-refresh'],
  rules: {
    'react-refresh/only-export-components': [
      'warn',
      { allowConstantExport: true },
    ],
    // Clean Code Rules - Allow some flexibility for development
    '@typescript-eslint/no-unused-vars': ['warn', { 
      'argsIgnorePattern': '^_',
      'varsIgnorePattern': '^_',
      'ignoreRestSiblings': true 
    }],
    '@typescript-eslint/no-explicit-any': 'warn', // Changed from error to warn
    'no-empty': ['error', { 'allowEmptyCatch': true }], // Allow empty catch blocks with comment
    'react-hooks/exhaustive-deps': 'warn', // Changed from error to warn
    'no-dupe-else-if': 'error',
    'prefer-const': 'warn'
  },
}