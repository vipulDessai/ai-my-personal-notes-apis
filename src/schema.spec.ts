import { schema } from './schema'; // Assuming your schema is in schema.js
import { graphql } from 'graphql';

describe('GraphQL Schema', () => {
  it('should resolve the "foo" query correctly', async () => {
    const query = `
      query {
        foo
      }
    `;

    const result = await graphql({ schema, source: query });

    expect(result.data?.foo).toBe('Hello, foo world!');
    expect(result.errors).toBeUndefined();
  });

  it('should handle errors gracefully', async () => {
    const query = `
      query {
        bar
      }
    `;

    const result = await graphql({ schema, source: query });

    expect(result.data?.bar).toBeUndefined();
    expect(result.errors).toBeDefined();
    expect(result.errors?.[0].message).toContain('Cannot query field "bar" on type "Query".');
  });
});