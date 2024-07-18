// Define a type for the category of a book
type Category = {
    id: number;
    name: string;
    displayOrder: number;
};

// Define a type for the product
type Product = {
    id: number;
    title: string;
    description: string;
    isbn: string;
    author: string;
    listPrice: number;
    price: number;
    price50: number;
    price100: number;
    categoryId: number;
    category: Category;
    productImages: null | string[];  // Assuming productImages can be either null or array of strings (image URLs)
};

// Array of Products
const Products: Product[] = [
    {
        id: 1,
        title: "Fortune of Time",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "SWD9999001",
        author: "Billy Spark",
        listPrice: 99,
        price: 90,
        price50: 85,
        price100: 80,
        categoryId: 1,
        category: {
          id: 1,
          name: "Action",
          displayOrder: 1
        },
        productImages: null
      },
      {
        id: 2,
        title: "Dark Skies",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "CAW777777701",
        author: "Nancy Hoover",
        listPrice: 40,
        price: 30,
        price50: 25,
        price100: 20,
        categoryId: 1,
        category: {
          id: 1,
          name: "Action",
          displayOrder: 1
        },
        productImages: null
      },
      {
        id: 3,
        title: "Vanish in the Sunset",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "RITO5555501",
        author: "Julian Button",
        listPrice: 55,
        price: 50,
        price50: 40,
        price100: 35,
        categoryId: 1,
        category: {
          id: 1,
          name: "Action",
          displayOrder: 1
        },
        productImages: null
      },
      {
        id: 4,
        title: "Cotton Candy",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "WS3333333301",
        author: "Abby Muscles",
        listPrice: 70,
        price: 65,
        price50: 60,
        price100: 55,
        categoryId: 2,
        category: {
          id: 2,
          name: "SciFi",
          displayOrder: 2
        },
        productImages: null
      },
      {
        id: 5,
        title: "Rock in the Ocean",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "SOTJ1111111101",
        author: "Ron Parker",
        listPrice: 30,
        price: 27,
        price50: 25,
        price100: 20,
        categoryId: 2,
        category: {
          id: 2,
          name: "SciFi",
          displayOrder: 2
        },
        productImages: null
      },
      {
        id: 6,
        title: "Leaves and Wonders",
        description: "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
        isbn: "FOT000000001",
        author: "Laura Phantom",
        listPrice: 25,
        price: 23,
        price50: 22,
        price100: 20,
        categoryId: 3,
        category: {
          id: 3,
          name: "History",
          displayOrder: 3
        },
        productImages: null
      }
];

export default Products;
