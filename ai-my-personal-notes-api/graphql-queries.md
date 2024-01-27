

mutation getTokenMutation {
  token(
    email: "hrleader@example.com", 
    password: "nevermind, this backend doesn't care for password"
  )
}

query authorize {
  unauthorized
}

query getRestuarants {
  restuarants (input: {
    batchSize: 10,
    filterKey: "cuisine",
    filterValue: "American",
  }) {
    body
    statusCode
  }
}

mutation addNote {
  addNote (input: {
    note: {
      inputData: {
        value: "some note 2",
        childInputs: [],
        tags: []
      },
      tags: ["t1", "t2"],
      title: "this is a test note",
      date: "2023-08-24T00:00:00"
    }
  }) {
    statusCode
    body
  }
}

query getNote {
  notes (input: {
    batchSize: 100,
  }) {
    statusCode
    body
  }
}

mutation addAuthor {
  addAuthor(input: {name: "Schiller"}) {
    record {
      id
      name
    }
  }
}

mutation addBook {
  addBook(input: {
    author: "7cbfd65e-3e57-4e1c-b7f1-e09b49a50ae0",
    title: "An die freude"
  }) {
    record {
      id
      title
      author {
        name
      }
    }
  }
}



# query
query books {
  books {
    id
    author {
      name
    }
  } 
}

query author {
  author(input: {
    authorId: "5944c03d-0f4d-4be7-8f6c-35cc7103c12f"
  }) {
    id
    name
  }
}